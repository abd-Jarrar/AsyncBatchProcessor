using AsyncBatchProcessor.Interfaces;

namespace AsyncBatchProcessor.Strategies
{
    public class NormalPricingStrategy : IPricingStrategy
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine("paid by NormalPricing Strategy");

        }
    }
}
