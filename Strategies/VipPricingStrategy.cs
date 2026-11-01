using AsyncBatchProcessor.Interfaces;

namespace AsyncBatchProcessor.Strategies
{
    public class VipPricingStrategy : IPricingStrategy
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine("paid by VipPricing Strategy");

        }
    }
}
