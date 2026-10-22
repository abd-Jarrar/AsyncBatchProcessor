using Asp.Versioning;
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
            var order = await orderService.GetOrderByIdAsync(orderId, ct);
            if (order is null)
                return NotFound(new ProblemDetails
                {
                    Title = "no order was found",
                    Detail = $"order with the id {orderId} was not found"
                });
            return Ok(order.ToResponse());
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
            return Ok(orders.Select(o=>o.ToResponse()).ToList());
        }

        [HttpPut]
        [Route("{orderId}/{status}")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, OrderStatus status, CancellationToken ct)
        {
            await orderService.UpdateStatus(orderId, status, ct);
            return NoContent();
        }

        [HttpPost("{orderId:guid}/items")]
        public async Task<IActionResult> AddOrderItem(Guid orderId, [FromBody] AddOrderItemRequest request, CancellationToken ct)
        {
            await orderService.AddItemAsync(orderId, request, ct);

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request, CancellationToken ct)
        {
            var orderId = await orderService.CreateOrderAsync(request, ct);
            var strategy = pricingStrategyFactory.GetStrategy(request.PricingStrategy);
            return Created(
                $"/api/orders/{orderId}",
                new { orderId,PricingStrategy=(strategy.GetType().Name) });
        }

        [HttpDelete]
        [Route("{orderId:guid}/items/{itemId:guid}")]
        public async Task<IActionResult> RemoveItem(Guid orderId, Guid itemId, CancellationToken ct)
        {
            await orderService.RemoveItemAsync(orderId, itemId, ct);
            return NoContent();
        }

        [HttpPost]
        [Route("cancel/{orderId:guid}")]
        public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken ct)
        {

            bool isDeleted = await orderService.CancelOrderAsync(orderId, ct);
            if (isDeleted)
                return NoContent();
            else
            {
                return NotFound(new ProblemDetails
                {
                    Title = "cancelling failed",
                    Detail = "order with the id {orderId} was not found"
                });
            }

        }

        [HttpPost]
        [Route("createOrders")]
        public async Task<IActionResult> ProcessOrdersAsync(List<CreateOrderRequest> requests, CancellationToken ct)
        {
            List<Task<Guid>> tasks = new List<Task<Guid>>();


             tasks = requests.Select(async request =>
            {
                await using var scope = factory.CreateAsyncScope();

                var orderService =
                    scope.ServiceProvider.GetRequiredService<OrderService>();

                return await orderService.CreateOrderAsync(request, ct);
            }).ToList();

            
            //foreach(var request in requests)
            //{
            //    var scopedOrderService = scope.ServiceProvider.GetRequiredService<OrderService>();
            //     tasks.Add(scopedOrderService.CreateOrderAsync(request, ct));

            //}
            try
            {
                await Task.WhenAll(tasks);
                //await Task.WhenAll(requests.Select(async  (r)=>  {
                //    using var scope = factory.CreateAsyncScope();
                //    var scopedOrderService = scope.ServiceProvider.GetRequiredService<OrderService>();
                //    return await scopedOrderService.CreateOrderAsync(r, ct);
                //    }));
            }
            catch(Exception ex)
            {
            
            }
            var tasksWithIndexes=tasks.Select((t,i)=>new {task=t,index=i,isSuccesfull=t.IsCompletedSuccessfully}).ToList();
            return Ok(new BatchResult
            {
                TotalOrders = tasks.Count,
                FailedOrders = tasksWithIndexes.Where(t => t.isSuccesfull == false).Select(t => new FailedOrderResult (t.index,t.task.Exception?.Message??"operation was cancelled")).ToList(),
                SuccessfulOrders = tasksWithIndexes.Where(t => t.isSuccesfull == true).Select(t => new OrderResult(t.task.Result)).ToList(),
                
            });

            
        }


    }
}
