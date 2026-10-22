namespace AsyncBatchProcessor.OrderItemRequests
{

    public record CreateOrderItemRequest(Guid ProductId, int Quantity);
}
