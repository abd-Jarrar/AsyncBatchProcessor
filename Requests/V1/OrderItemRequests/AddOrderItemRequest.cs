namespace AsyncBatchProcessor.Requests.V1.OrderItemRequests
{
    public record AddOrderItemRequest(Guid ProductId, int Quantity);
}
