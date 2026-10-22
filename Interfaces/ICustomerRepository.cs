using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken ct);
        Task<List<Customer>> GetAllAsync(CancellationToken ct);
        Task<List<Customer>> GetWithOrdersAsync(CancellationToken ct);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct);
        Task AddAsync(Customer customer, CancellationToken ct);
        void Remove(Customer customer);
        Task SaveChangesAsync(CancellationToken ct);   // new
    }
}
