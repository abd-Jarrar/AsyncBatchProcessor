using Asp.Versioning;
using AsyncBatchProcessor.Models;
using AsyncBatchProcessor.Requests.V2.CustomerRequests;
using AsyncBatchProcessor.Services.V2;
using Microsoft.AspNetCore.Mvc;

namespace AsyncBatchProcessor.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/V{version:apiVersion}/customers")]
    public class CustomerController(CustomerService customerService,OrderService orderService) : ControllerBase
    {
        [HttpGet]
        [Route("{customerId:guid}")]
        public async Task<IActionResult> GetCustomerById(Guid customerId, CancellationToken ct)
        {
            var result = await customerService.GetCustomerByIdAsync(customerId, ct);
            if (result.IsFailure)
            {
                return NotFound(result);
            }
            else
            {
                return Ok(result.Value);
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
                return Ok(customers);
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
            var result = await customerService.DeleteCustomerByIdAsync(customerId,ct);
            if (result.IsSuccess)
                return NoContent();
            else
            {
                return NotFound(result.Error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CreateCustomerRequest request,CancellationToken ct)
        {
            var result = await customerService.CreateCustomerAsync(
                request.Name,
                request.Email,
                request.Phone,
                ct);
            if (result.IsSuccess)
            {
                return CreatedAtAction(
               nameof(GetCustomerById),
               new { result.Value },
               new { id = result.Value });
            }
            else
            {
                return Conflict(result.Error);
            }
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
