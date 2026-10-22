using AsyncBatchProcessor.Interfaces;

namespace AsyncBatchProcessor.Services.V2
{
    public class SmsNotification :INotification
    {
        public Task SendAsync(string message)
        {
            Console.WriteLine($"Sending SMS: {message}");
            return Task.CompletedTask;
        }
    }
}
