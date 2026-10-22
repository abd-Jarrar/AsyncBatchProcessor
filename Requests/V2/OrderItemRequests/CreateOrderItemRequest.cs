namespace AsyncBatchProcessor.Requests.V2.OrderItemRequests
{

    public record CreateOrderItemRequest(Guid ProductId, int Quantity);
}
