using AsyncBatchProcessor.Interfaces;

namespace AsyncBatchProcessor.Strategies
{
    public class BlackFridayPricingStrategy : IPricingStrategy
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine("paid by BlackFridayPricing Strategy");
        }
    }
}
