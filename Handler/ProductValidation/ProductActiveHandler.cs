using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Handler.ProductValidation
{
    public class ProductActiveHandler : ProductValidationHandler
    {
        protected override void Validate(Product product, int quantity)
        {
            if (!product.IsActive)
            {
                throw new InvalidOperationException($"Product with the id {product.Id} is inactive.");
            }
            
        }
    }
}
