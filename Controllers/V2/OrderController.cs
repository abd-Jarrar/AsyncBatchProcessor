using Asp.Versioning;
using AsyncBatchProcessor.Common;
using AsyncBatchProcessor.Models;
using AsyncBatchProcessor.Requests.V2.OrderItemRequests;
using AsyncBatchProcessor.Requests.V2.OrderRequests;
using AsyncBatchProcessor.Responses.V2.BatchResponse;
using AsyncBatchProcessor.Services.V2;
using AsyncBatchProcessor.Strategies;

using Microsoft.AspNetCore.Mvc;

namespace AsyncBatchProcessor.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/V{version:apiVersion}/orders")]
    public class OrderController(OrderService orderService, IServiceScopeFactory  factory,PricingStrategyFactory pricingStrategyFactory) :ControllerBase
    {
        [HttpGet]
        [Route("{orderId:guid}")]
        public async Task<IActionResult> GetOrderById(Guid orderId, CancellationToken ct)
        {
            var result = await orderService.GetOrderByIdForReadAsync(orderId, ct);
            if (result.IsFailure)
                return NotFound(new ProblemDetails
                {
                    Title = "no order was found",
                    Detail = result.Error
                });
            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(CancellationToken ct)
        {
            var orders = await orderService.GetAllOrdersAsync(ct);
            if (orders.Count() == 0)
                return NotFound(new ProblemDetails
                {
                    Title = "no orders found",
                    Detail = "there is no orders right now"
                });
            return Ok(orders);
        }

        [HttpPut]
        [Route("{orderId}/{status}")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, OrderStatus status, CancellationToken ct)
        {
            var result=await orderService.UpdateStatus(orderId, status, ct);
            if(result.IsSuccess)
                return NoContent();
            else
                return Problem(
                    detail: result.Error,
                    statusCode: result.ErrorStatusCode,
                    title: "Failed to update order status"
                    );
        }

        [HttpPost("{orderId:guid}/items")]
        public async Task<IActionResult> AddOrderItem(Guid orderId, [FromBody] AddOrderItemRequest request, CancellationToken ct)
        {
            var result=await orderService.AddItemAsync(orderId, request, ct);
            if( result.IsSuccess)
                return NoContent();

            else
                return Problem(
                    detail: result.Error,
                    statusCode: result.ErrorStatusCode,
                    title: "Failed to add the item to the order "
                    );
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request, CancellationToken ct)
        {
            var result = await orderService.CreateOrderAsync(request, ct);
            if (result.IsSuccess)
            {
                var strategy = pricingStrategyFactory.GetStrategy(request.PricingStrategy);
                return Created(
                    $"/api/orders/{result.Value}",
                    new { result.Value, PricingStrategy = (strategy.GetType().Name) });
            }
            else
                   return Problem(
                    detail: result.Error,
                    statusCode: result.ErrorStatusCode,
                    title: "Failed to Create the Order "
                    );
        }

        [HttpDelete]
        [Route("{orderId:guid}/items/{itemId:guid}")]
        public async Task<IActionResult> RemoveItem(Guid orderId, Guid itemId, CancellationToken ct)
        {
            var result=await orderService.RemoveItemAsync(orderId, itemId, ct);
            if(result.IsSuccess)
                return NoContent();
            else
            {
                return Problem(
                    detail: result.Error,
                    statusCode: result.ErrorStatusCode,
                    title: "Failed to to remove the item from the order "
                    );
            }
        }

        [HttpPost]
        [Route("cancel/{orderId:guid}")]
        public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken ct)
        {

            var result= await orderService.CancelOrderAsync(orderId, ct);
            if (result.IsSuccess)
                return NoContent();
            else
            {
                return Problem(
                   detail: result.Error,
                   statusCode: result.ErrorStatusCode,
                   title: "Failed to to cancel the order "
                   );
            }

        }

        [HttpPost]
        [Route("createOrders")]
        public async Task<IActionResult> ProcessOrdersAsync(List<CreateOrderRequest> requests, CancellationToken ct)
        {
            List<Task<Result<Guid>>> tasks = new List<Task<Result<Guid>>>();


             tasks = requests.Select(async request =>
            {
                await using var scope = factory.CreateAsyncScope();

                var orderService =
                    scope.ServiceProvider.GetRequiredService<OrderService>();

                return await orderService.CreateOrderAsync(request, ct);
            }).ToList();


            try
            {
                await Task.WhenAll(tasks);

            }
            catch(Exception ex)
            {
                
            }
            var tasksWithIndexes=tasks.Select((t,i)=>new {task=t,index=i,isSuccesfull=t.IsCompletedSuccessfully}).ToList();
            return Ok(new BatchResult
            {
                TotalOrders = tasks.Count,
                FailedOrders = tasksWithIndexes.Where(t => t.isSuccesfull == false).Select(t => new FailedOrderResult (t.index,t.task.Exception?.Message??"operation was cancelled")).ToList(),
                SuccessfulOrders = tasksWithIndexes.Where(t => t.isSuccesfull == true).Select(t => new OrderResult(t.task.Result.Value)).ToList(),
                
            });

            
        }


    }
}
