namespace AsyncBatchProcessor.Responses.V2.BatchResponse
{
    public class BatchResult
    {
        public int TotalOrders { get; init; }

        public List<OrderResult> SuccessfulOrders { get; init; } = [];

        public List<FailedOrderResult> FailedOrders { get; init; } = [];

        public int SuccessCount => SuccessfulOrders.Count;

        public int FailureCount => FailedOrders.Count;
    }

}
