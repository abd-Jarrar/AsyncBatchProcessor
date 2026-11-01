using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct);
        Task<List<Order>> GetAllAsync(CancellationToken ct);
        Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct);
        Task<List<Order>> GetCreatedSinceAsync(DateTime since, CancellationToken ct);
        Task<List<Product>> GetTop5SellingProductsAsync(CancellationToken ct);

        Task AddAsync(Order order, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}