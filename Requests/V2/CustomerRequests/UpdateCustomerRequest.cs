namespace AsyncBatchProcessor.Requests.V2.CustomerRequests
{
    public record UpdateCustomerRequest
    {
        public string? Name { get; init; }
        public string? Email { get; init; }
    }
}
