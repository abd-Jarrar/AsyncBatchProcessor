namespace AsyncBatchProcessor.Responses.V2
{
    public record ProductResponse(string Name, string SKU, decimal Price, int stockQuantity, bool IsActive);
    
}
