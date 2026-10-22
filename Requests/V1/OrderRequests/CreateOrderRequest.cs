using AsyncBatchProcessor.Requests.V1.OrderItemRequests;

namespace AsyncBatchProcessor.Requests.V1.OrderRequests
{
    public record CreateOrderRequest(Guid CustomerId, List<CreateOrderItemRequest> Items);
}
