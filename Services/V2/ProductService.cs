using AsyncBatchProcessor.Common;
using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;
using AsyncBatchProcessor.Responses.V2;
using Microsoft.Extensions.Caching.Hybrid;
using System.ComponentModel.DataAnnotations;

namespace AsyncBatchProcessor.Services.V2
{
    public class ProductService(IProductRepository repo,HybridCache hybridCache,ILogger<ProductService> logger)
    {
        public async Task<Result<Guid>> CreateProductAsync(
            string productName, string sku, decimal productPrice, int? stockQuantity, CancellationToken ct)
        {
            // Business rules
            if (await repo.SkuExistsAsync(sku, ct))
                return Result<Guid>.Failure($"A product with SKU '{sku}' already exists.",StatusCodes.Status409Conflict);

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = productName,
                SKU = sku,
                Price = productPrice,
                StockQuantity = stockQuantity ?? 0,
                IsActive = true
            };

            await repo.AddAsync(product, ct);
            await repo.SaveChangesAsync(ct);
            return Result<Guid>.Success(product.Id);
        }

        public async Task<Result<bool>> DeleteProductByIdAsync(Guid productId, CancellationToken ct)
        {
            var product = await repo.GetByIdAsync(productId, ct);
            if (product is null)
                return Result<bool>.Failure($"the product with the id: {productId} was not found", StatusCodes.Status404NotFound);

            repo.Remove(product);
            await repo.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<ProductResponse>> GetProductByIdAsync(Guid productId,CancellationToken ct)
        {
            var cacheKey = $"products:{productId}";
            Console.WriteLine("---------------------------------");
            var product = await hybridCache.GetOrCreateAsync(
                cacheKey,
                async (token) =>
                {
                    Console.WriteLine($"DATABASE VISITED for ID:{productId}");
                    return await repo.GetByIdAsync(productId, token);
                },
                cancellationToken: ct
            );
            Console.WriteLine($"Cache VISITED for ID:{productId}");

            if (product is null)
            {
                Console.WriteLine("here");
                return Result<ProductResponse>.Failure(
                    $"The product with the id: {productId} was not found",
                    StatusCodes.Status404NotFound);
            }

            return Result<ProductResponse>.Success(product.ToResponse());



        }

        public async Task<Result<ProductResponse>> GetProductByNameAsync(string productName, CancellationToken ct) {
        
            var product=await repo.GetByNameAsync(productName, ct);
            if (product is null)
                return Result<ProductResponse>.Failure($"the product with the name {productName} was not found",StatusCodes.Status404NotFound);
            else
                return Result<ProductResponse>.Success(product.ToResponse());
        }

        public async Task<Result<ProductResponse>> GetProductWithMaximumPriceAsync(CancellationToken ct)
        {
            var product = await repo.GetMaxPriceAsync(ct);
            if (product is null)
                return Result<ProductResponse>.Failure("there is no products right now!!!", StatusCodes.Status404NotFound);
            else
                return Result<ProductResponse>.Success(product.ToResponse());
        }

        public async Task<Result<ProductResponse>> GetProductWithMinimumPriceAsync(CancellationToken ct){
        
            var product=await repo.GetMinPriceAsync(ct);
            if (product is null)
                return Result<ProductResponse>.Failure("there is no products right now!!!", StatusCodes.Status404NotFound);
            else
                return Result<ProductResponse>.Success(product.ToResponse());
        }

        public async Task<List<ProductResponse>> GetAllProductsAsync(CancellationToken ct) {

            var products = await repo.GetAllAsync(ct);
            return products.Select(p => p.ToResponse()).ToList();
        }

        public async Task<List<ProductResponse>> GetProductWithStockAsync(int stockQuantity, CancellationToken ct)
        {
            var products = await repo.GetWithStockAsync(stockQuantity, ct);
            return products.Select(p => p.ToResponse()).ToList();
        }

        public async Task<List<ProductResponse>> GetActiveProductsAsync(CancellationToken ct)
        {
            var products=await repo.GetActiveAsync(ct);
            return products.Select(p => p.ToResponse()).ToList();
        }
    }
}