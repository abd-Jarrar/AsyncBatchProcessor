namespace AsyncBatchProcessor.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> PayAsync(decimal amount);
    }
}
