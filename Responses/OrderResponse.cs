using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Responses
{
    public record OrderResponse(Guid Id, Guid CustomerId, OrderStatus Status, decimal TotalAmount, DateTime CreatedAt);
    
}
