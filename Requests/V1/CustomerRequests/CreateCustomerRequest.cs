namespace AsyncBatchProcessor.Requests.V1.CustomerRequests
{
    public record CreateCustomerRequest(string Name, string Email, string Phone);
}
