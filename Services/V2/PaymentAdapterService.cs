using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.ThridPartyService;

namespace AsyncBatchProcessor.Services.V2
{
    public class PaymentAdapterService(ThirdPartyPayment thirdPartyPaymentService) : IPaymentService
    {
        public Task<bool> PayAsync(decimal amount)
        {
            double total=(double)amount;
            string result = thirdPartyPaymentService.ExecuteTransaction(total);

            bool success = (result == "SUCCESS");

            return Task.FromResult(success);
        }
    }
}
