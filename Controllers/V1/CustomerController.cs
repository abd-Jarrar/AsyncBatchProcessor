using Asp.Versioning;
using AsyncBatchProcessor.Models;
using AsyncBatchProcessor.Requests.V1.CustomerRequests;
using AsyncBatchProcessor.Services.V1;
using Microsoft.AspNetCore.Mvc;

namespace AsyncBatchProcessor.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/customers")]
    [Route("api/V{version:apiVersion}/customers")]
    public class CustomerController(CustomerService customerService,OrderService orderService) : ControllerBase
    {
        [HttpGet]
        [Route("{customerId:guid}")]
        public async Task<IActionResult> GetCustomerById(Guid customerId,CancellationToken ct)
        {
            var customer =  await customerService.GetCustomerByIdAsync(customerId, ct);
            if (customer is null)
                return NotFound(new ProblemDetails()
                {
                    Title="not found",
                    Detail=$"customer with the id {customerId} was not found"
                });
            else
            {
                return Ok(customer.ToResponse());

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCusomters(CancellationToken ct)
        {
            var customers = await customerService.GetAllCustomersAsync(ct);
            if (customers.Count==0)
                return NotFound(new ProblemDetails
                {
                    Title="no customers",
                    Detail="there is no customer right now "
                });
            else
            {
                
                return Ok(customers.Select(c=>c.ToResponse()).ToList());
            }
        }


        [HttpGet]
        [Route("total-sales/{customerId:guid}")]
        public async Task<IActionResult> GetCusomterSales(Guid customerId, CancellationToken ct)
        {

            var sales = await orderService.GetCustomerTotalSalesAsync(customerId, ct);
            if (sales is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "there is no customer",
                    Detail = $"there is no customer with the id {customerId}"
                });
            }
            return Ok(sales);


        }


        [HttpDelete]
        [Route("{customerId:guid}")]
        public async Task<IActionResult> DelteCustomer(Guid customerId,CancellationToken ct)
        {
            
            var isDeleted = await customerService.DeleteCustomerByIdAsync(customerId,ct);
            if (isDeleted)
                return NoContent();
            else
            {
                return NotFound(new ProblemDetails()
                {
                    Title= "customer  was not found",
                    Detail= "Customer with the id: {customerId} was not found"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CreateCustomerRequest request,CancellationToken ct)
        {
            var customerId = await customerService.CreateCustomerAsync(
                request.Name,
                request.Email,
                request.Phone,
                ct);

            return CreatedAtAction(
                nameof(GetCustomerById),
                new { customerId },
                new { id = customerId });
        }

        [HttpGet]
        [Route("with-orders")]
        public async Task<IActionResult>GetCustomersWithOrders(CancellationToken ct)
        {
            var customers=await customerService.GetCustomersWithOrdersAsync(ct);
            if (customers.Count == 0)
                return NotFound(new ProblemDetails()
                {
                    Title = "no customers",
                    Detail = "there is no customesr with orders right now "
                });
            else
            {
                return Ok(customers);
            }
        }
    }
}
