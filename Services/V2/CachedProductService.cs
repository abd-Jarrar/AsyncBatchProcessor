using AsyncBatchProcessor.Common;
using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;
using AsyncBatchProcessor.Responses.V2;

namespace AsyncBatchProcessor.Services.V2
{
    public class CachedProductService(ProductService productService) : IProductService
    {
        public async Task<Result<Guid>> CreateProductAsync(string productName, string sku, decimal productPrice, int? stockQuantity, CancellationToken ct)
        {
            return await productService.CreateProductAsync(productName,sku,productPrice,stockQuantity,ct);
        }

        public async Task<Result<bool>> DeleteProductByIdAsync(Guid productId, CancellationToken ct)
        {
            return  await productService.DeleteProductByIdAsync(productId, ct);
        }

        public async Task<List<ProductResponse>> GetActiveProductsAsync(CancellationToken ct)
        {
            return await productService.GetActiveProductsAsync(ct);
        }

        public async Task<List<ProductResponse>> GetAllProductsAsync(CancellationToken ct)
        {
            Console.WriteLine("caching using the decorator ....");
            return await productService.GetAllProductsAsync(ct);
            
        }

        public async Task<Result<ProductResponse>> GetProductByIdAsync(Guid productId, CancellationToken ct)
        {
            //Console.WriteLine("caching using the decorator ....");

            return await productService.GetProductByIdAsync(productId, ct);
        }

        public async Task<Result<ProductResponse>> GetProductByNameAsync(string productName, CancellationToken ct)
        {
            return await productService.GetProductByNameAsync(productName, ct);
        }

        public async Task<Result<ProductResponse>> GetProductWithMaximumPriceAsync(CancellationToken ct)
        {
            return await productService.GetProductWithMaximumPriceAsync(ct);
        }

        public async Task<Result<ProductResponse>> GetProductWithMinimumPriceAsync(CancellationToken ct)
        {
            return await productService.GetProductWithMinimumPriceAsync(ct);

        }

        public async Task<List<ProductResponse>> GetProductWithStockAsync(int stockQuantity, CancellationToken ct)
        {
            return await productService.GetProductWithStockAsync(stockQuantity,ct);
        }
    }
}
