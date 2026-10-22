namespace AsyncBatchProcessor.Requests.V2.OrderItemRequests
{
    public record AddOrderItemRequest(Guid ProductId, int Quantity);
}
