namespace AsyncBatchProcessor.Responses
{
    public record ProductResponse(string Name, string SKU, decimal Price, int stockQuantity, bool IsActive);
    
}
