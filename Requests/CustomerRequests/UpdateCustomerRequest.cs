namespace AsyncBatchProcessor.CustomerRequests
{
    public record UpdateCustomerRequest
    {
        public string? Name { get; init; }
        public string? Email { get; init; }
    }
}
