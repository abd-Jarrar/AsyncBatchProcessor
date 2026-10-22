namespace AsyncBatchProcessor.Interfaces
{
    public interface INotification
    {
        Task SendAsync(string message);

    }
}
