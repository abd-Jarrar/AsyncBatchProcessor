namespace AsyncBatchProcessor.Requests.V1.OrderItemRequests
{

    public record CreateOrderItemRequest(Guid ProductId, int Quantity);
}
