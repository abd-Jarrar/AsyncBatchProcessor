using AsyncBatchProcessor.Interfaces;

namespace AsyncBatchProcessor.Services.V2
{
    public class LoggingDecorator(INotification notificationType) : INotification
    {
        
        public async Task SendAsync(string message)
        {
            Console.WriteLine("logging using the decorator started....");
            await notificationType.SendAsync(message);
            Console.WriteLine("logging using the decorator finished....");

        }
    }
}
