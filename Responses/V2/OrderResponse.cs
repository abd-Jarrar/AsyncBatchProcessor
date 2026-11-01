using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Responses.V2
{
    public record OrderResponse(Guid Id, Guid CustomerId, OrderStatus Status, decimal TotalAmount, DateTime CreatedAt);
    
}
