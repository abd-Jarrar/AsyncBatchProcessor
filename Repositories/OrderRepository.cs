using AsyncBatchProcessor.Data;
using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace AsyncBatchProcessor.Repositories
{
    public class OrderRepository(AppDbContext context) : IOrderRepository
    {
       
        public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct) =>
            context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId, ct);

        public Task<List<Order>> GetAllAsync(CancellationToken ct) =>
            context.Orders.AsNoTracking().Include(o => o.OrderItems).ToListAsync(ct);

        public Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct) =>
            context.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync(ct);

        public Task<List<Order>> GetCreatedSinceAsync(DateTime since, CancellationToken ct) =>
            context.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems)
                .Where(o => o.CreatedAt >= since)
                .ToListAsync(ct);

        public Task<List<Product>> GetTop5SellingProductsAsync(CancellationToken ct) =>
            context.Products
                .FromSqlRaw("SELECT TOP 5 p.* FROM Products p INNER JOIN OrderItems oi ON p.Id = oi.ProductId GROUP BY p.Id, p.Name, p.Price, p.SKU, p.StockQuantity, p.IsActive, p.CreatedAt ORDER BY SUM(oi.Quantity) DESC")
                .AsNoTracking()
                .ToListAsync(ct);

        public async Task AddAsync(Order order, CancellationToken ct) =>
            await context.Orders.AddAsync(order, ct);

        public Task SaveChangesAsync(CancellationToken ct) =>
            context.SaveChangesAsync(ct);
    }
}