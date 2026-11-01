using AsyncBatchProcessor.Interfaces;

namespace AsyncBatchProcessor.Strategies
{
    public class PricingStrategyFactory(IServiceProvider serviceProvider)
    {

        public IPricingStrategy GetStrategy(string strategy)
        {
            return strategy.ToLowerInvariant() switch
            {
                "normal" => serviceProvider
                    .GetRequiredService<NormalPricingStrategy>(),

                "vip" => serviceProvider
                    .GetRequiredService<VipPricingStrategy>(),

                "blackfriday" => serviceProvider
                    .GetRequiredService<BlackFridayPricingStrategy>(),

                _ => throw new ArgumentException(
                    "Invalid pricing strategy.")
            };
        }
    }
}
