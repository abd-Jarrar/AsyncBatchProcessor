using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Responses.V1
{
    public record OrderResponse(Guid Id, Guid CustomerId, OrderStatus Status, decimal TotalAmount, DateTime CreatedAt);
    
}
