using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Services.V1
{
    public class CustomerService(ICustomerRepository customerRepository) 
    {
        public async Task<Guid> CreateCustomerAsync(
            string customerName, string customerEmail, string phone, CancellationToken ct)
        {
            
            if (await customerRepository.EmailExistsAsync(customerEmail, ct))
                throw new InvalidOperationException($"A customer with email '{customerEmail}' already exists.");

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = customerName,
                Email = customerEmail,
                Phone = phone,
                CreatedAt = DateTime.UtcNow
            };

            await customerRepository.AddAsync(customer, ct);
            await customerRepository.SaveChangesAsync(ct);
            return customer.Id;
        }

        public async Task<bool> DeleteCustomerByIdAsync(Guid customerId, CancellationToken ct)
        {
            var customer = await customerRepository.GetByIdAsync(customerId, ct);
            if (customer is null)
                return false;

            customerRepository.Remove(customer);
            await customerRepository.SaveChangesAsync(ct);
            return true;
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid customerId, CancellationToken ct) =>
            await customerRepository.GetByIdAsync(customerId, ct);

        public async Task<List<Customer>> GetAllCustomersAsync(CancellationToken ct) =>
            await customerRepository.GetAllAsync(ct);

        public async Task<List<Customer>> GetCustomersWithOrdersAsync(CancellationToken ct) =>
           await  customerRepository.GetWithOrdersAsync(ct);
    }
}