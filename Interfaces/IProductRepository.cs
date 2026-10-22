using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid productId, CancellationToken ct);
        Task<Product?> GetByNameAsync(string productName, CancellationToken ct);
        Task<Product?> GetMaxPriceAsync(CancellationToken ct);
        Task<Product?> GetMinPriceAsync(CancellationToken ct);
        Task<List<Product>> GetAllAsync(CancellationToken ct);
        Task<List<Product>> GetWithStockAsync(int minStock, CancellationToken ct);
        Task<List<Product>> GetActiveAsync(CancellationToken ct);
        Task<bool> SkuExistsAsync(string sku, CancellationToken ct);

        Task AddAsync(Product product, CancellationToken ct);
        void Remove(Product product);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
