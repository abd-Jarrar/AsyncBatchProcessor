using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;
using AsyncBatchProcessor.Services.V2;

namespace AsyncBatchProcessor.Factories
{
    public class NotificationFactory(IServiceProvider serviceProvider)
    {
        public INotification Create(NotificationType type)
        {
            return type switch
            {
                NotificationType.Sms =>
                serviceProvider.GetRequiredService<SmsNotification>(),

                NotificationType.Email =>
                    serviceProvider.GetRequiredService<EmailNotification>(),

                NotificationType.Push =>
                    serviceProvider.GetRequiredService<PushNotification>(),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(type),
                    type,
                    "Unsupported notification type")
            };
        }
    }
}
