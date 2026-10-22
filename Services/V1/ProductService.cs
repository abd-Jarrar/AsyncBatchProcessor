using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;
using System.ComponentModel.DataAnnotations;

namespace AsyncBatchProcessor.Services.V1
{
    public class ProductService(IProductRepository repo)
    {
        public async Task<Guid> CreateProductAsync(
            string productName, string sku, decimal productPrice, int? stockQuantity, CancellationToken ct)
        {
            // Business rules
            if (productPrice < 0)
                throw new ValidationException("Price cannot be negative.");

            if (stockQuantity is < 0)
                throw new ValidationException("Stock cannot be negative.");

            if (await repo.SkuExistsAsync(sku, ct))
                throw new InvalidOperationException($"A product with SKU '{sku}' already exists.");

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
            return product.Id;
        }

        public async Task<bool> DeleteProductByIdAsync(Guid productId, CancellationToken ct)
        {
            var product = await repo.GetByIdAsync(productId, ct);
            if (product is null)
                return false;

            repo.Remove(product);
            await repo.SaveChangesAsync(ct);
            return true;
        }

        public Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken ct) =>
            repo.GetByIdAsync(productId, ct);

        public Task<Product?> GetProductByNameAsync(string productName, CancellationToken ct) =>
            repo.GetByNameAsync(productName, ct);

        public Task<Product?> GetProductWithMaximumPriceAsync(CancellationToken ct) =>
            repo.GetMaxPriceAsync(ct);

        public Task<Product?> GetProductWithMinimumPriceAsync(CancellationToken ct) =>
            repo.GetMinPriceAsync(ct);

        public Task<List<Product>> GetAllProductsAsync(CancellationToken ct) =>
            repo.GetAllAsync(ct);

        public Task<List<Product>> GetProductWithStockAsync(int stockQuantity, CancellationToken ct) =>
            repo.GetWithStockAsync(stockQuantity, ct);

        public Task<List<Product>> GetActiveProductsAsync(CancellationToken ct) =>
            repo.GetActiveAsync(ct);
    }
}