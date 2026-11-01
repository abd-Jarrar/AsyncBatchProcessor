using AsyncBatchProcessor.Common;
using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;
using AsyncBatchProcessor.Responses.V2;

namespace AsyncBatchProcessor.Services.V2
{
    public class CustomerService(ICustomerRepository customerRepository) 
    {
        public async Task<Result<Guid>> CreateCustomerAsync(
            string customerName, string customerEmail, string phone, CancellationToken ct)
        {

            if (await customerRepository.EmailExistsAsync(customerEmail, ct))
                return Result<Guid>.Failure($"A customer with email '{customerEmail}' already exists.",StatusCodes.Status409Conflict);

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
            return Result<Guid>.Success(customer.Id);
        }

        public async Task<Result<bool>> DeleteCustomerByIdAsync(Guid customerId, CancellationToken ct)
        {
            var customer = await customerRepository.GetByIdAsync(customerId, ct);
            if (customer is null)
                return Result<bool>.Failure($"customer with the id: {customerId} was not found", StatusCodes.Status404NotFound);

            customerRepository.Remove(customer);
            await customerRepository.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<CustomerResponse>> GetCustomerByIdAsync(Guid customerId, CancellationToken ct) {
        
            var customer=await customerRepository.GetByIdAsync(customerId, ct);
            if(customer is null)
            {
                return Result<CustomerResponse>.Failure($"Customer with the Id: {customerId} was not found", StatusCodes.Status404NotFound);
            }
            else
            {
                return Result<CustomerResponse>.Success(customer.ToResponse());
            }
        }

        public async Task<List<CustomerResponse>> GetAllCustomersAsync(CancellationToken ct) {
            var customers=await customerRepository.GetAllAsync(ct);
            var customersDto = customers.Select(c => c.ToResponse()).ToList();
            return customersDto;
        }

        public async Task<List<CustomerResponse>> GetCustomersWithOrdersAsync(CancellationToken ct) {
        
           var customers=await  customerRepository.GetWithOrdersAsync(ct);
           var customersDto = customers.Select(c => c.ToResponse()).ToList();
           return customersDto;
        
        }
    }
}