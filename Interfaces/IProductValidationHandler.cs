using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Interfaces
{
    public interface IProductValidationHandler
    {
        void SetNextHandler(IProductValidationHandler next);

        public void Handle(Product product, int quantity );

    }
}
