using AsyncBatchProcessor.Data;
using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace AsyncBatchProcessor.Repositories
{
    public class CustomerRepository(AppDbContext context) : ICustomerRepository
    {
        public async Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken ct) =>
        await context.Customers.FirstOrDefaultAsync(c=>c.Id==customerId,ct);

        public Task<List<Customer>> GetAllAsync(CancellationToken ct) =>
            context.Customers.AsNoTracking().ToListAsync(ct);

        public Task<List<Customer>> GetWithOrdersAsync(CancellationToken ct) =>
            context.Customers
                .AsNoTracking()
                .Where(c => context.Orders.Any(o => o.CustomerId == c.Id))
                .ToListAsync(ct);

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
            context.Customers.AnyAsync(c => c.Email == email, ct);

        public async Task AddAsync(Customer customer, CancellationToken ct) =>
            await context.Customers.AddAsync(customer, ct);

        public void Remove(Customer customer) =>
            context.Customers.Remove(customer);

        public Task SaveChangesAsync(CancellationToken ct) =>
    context.SaveChangesAsync(ct);
    }
}
