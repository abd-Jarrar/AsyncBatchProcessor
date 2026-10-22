namespace AsyncBatchProcessor.Responses.V1
{
    public record ProductResponse(string Name, string SKU, decimal Price, int stockQuantity, bool IsActive);
    
}
