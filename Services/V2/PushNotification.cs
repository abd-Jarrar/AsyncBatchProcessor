using AsyncBatchProcessor.Interfaces;

namespace AsyncBatchProcessor.Services.V2
{
    public class PushNotification: INotification
    {
        public Task SendAsync(string message)
        {
            Console.WriteLine($"Sending Push Notification: {message}");
            return Task.CompletedTask;
        }
    }
}
