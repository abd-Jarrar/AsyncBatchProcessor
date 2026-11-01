using AsyncBatchProcessor.Interfaces;

namespace AsyncBatchProcessor.Services.V2
{
    public class EmailNotification : INotification
    {
        public Task SendAsync(string message)
        {
            Console.WriteLine($"Sending Email: {message}");
            return Task.CompletedTask;
        }
    }
}
