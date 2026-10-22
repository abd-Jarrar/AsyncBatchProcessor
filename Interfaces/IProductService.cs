using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Interfaces
{
    public interface IProductService
    {
        public Task<Guid> CreateProductAsync(
            string productName, string sku, decimal productPrice, int? stockQuantity, CancellationToken ct);

        public Task<bool> DeleteProductByIdAsync(Guid productId, CancellationToken ct);

        public Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken ct);

        public Task<Product?> GetProductByNameAsync(string productName, CancellationToken ct);

        public Task<Product?> GetProductWithMaximumPriceAsync(CancellationToken ct);

        public Task<Product?> GetProductWithMinimumPriceAsync(CancellationToken ct);

        public Task<List<Product>> GetAllProductsAsync(CancellationToken ct);

        public Task<List<Product>> GetProductWithStockAsync(int stockQuantity, CancellationToken ct);

        public Task<List<Product>> GetActiveProductsAsync(CancellationToken ct);
    }
}
