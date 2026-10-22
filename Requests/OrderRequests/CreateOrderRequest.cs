

using AsyncBatchProcessor.OrderItemRequests;

namespace AsyncBatchProcessor.OrderRequests
{
    public record CreateOrderRequest(Guid CustomerId, List<CreateOrderItemRequest> Items);
}
