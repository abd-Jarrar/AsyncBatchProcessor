namespace AsyncBatchProcessor.ThridPartyService
{
    public class ThirdPartyPayment
    {
        public string ExecuteTransaction(double total)
        {
            Console.WriteLine($"Charging ${total}");

            return "SUCCESS";
        }
    }
}
