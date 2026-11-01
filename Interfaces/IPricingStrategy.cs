namespace AsyncBatchProcessor.Interfaces
{
    public interface IPricingStrategy
    {
        public void Pay(decimal amount);
    }
}
