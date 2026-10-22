namespace AsyncBatchProcessor.OrderItemRequests
{
    public record AddOrderItemRequest(Guid ProductId, int Quantity);
}
