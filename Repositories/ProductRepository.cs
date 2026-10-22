using AsyncBatchProcessor.Data;
using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace AsyncBatchProcessor.Repositories
{
    public class ProductRepository(AppDbContext context) : IProductRepository
    {
        public Task<Product?> GetByIdAsync(Guid productId, CancellationToken ct) =>
            context.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);

        public Task<Product?> GetByNameAsync(string productName, CancellationToken ct) =>
            context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Name == productName, ct);

        public Task<Product?> GetMaxPriceAsync(CancellationToken ct) =>
            context.Products.AsNoTracking().OrderByDescending(p => p.Price).FirstOrDefaultAsync(ct);

        public Task<Product?> GetMinPriceAsync(CancellationToken ct) =>
            context.Products.AsNoTracking().OrderBy(p => p.Price).FirstOrDefaultAsync(ct);

        public Task<List<Product>> GetAllAsync(CancellationToken ct) =>
            context.Products.AsNoTracking().ToListAsync(ct);

        public Task<List<Product>> GetWithStockAsync(int minStock, CancellationToken ct) =>
            context.Products.AsNoTracking().Where(p => p.StockQuantity >= minStock).ToListAsync(ct);

        public Task<List<Product>> GetActiveAsync(CancellationToken ct) =>
            context.Products.AsNoTracking().Where(p => p.IsActive).ToListAsync(ct);

        public Task<bool> SkuExistsAsync(string sku, CancellationToken ct) =>
            context.Products.AnyAsync(p => p.SKU == sku, ct);

        public async Task AddAsync(Product product, CancellationToken ct) =>
            await context.Products.AddAsync(product, ct);

        public void Remove(Product product) =>
            context.Products.Remove(product);

        public Task SaveChangesAsync(CancellationToken ct) =>
            context.SaveChangesAsync(ct);
    }
}

