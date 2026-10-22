using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Requests.V2.OrderItemRequests;

namespace AsyncBatchProcessor.Requests.V2.OrderRequests
{
    public record CreateOrderRequest(Guid CustomerId, List<CreateOrderItemRequest> Items, string PricingStrategy);
}
