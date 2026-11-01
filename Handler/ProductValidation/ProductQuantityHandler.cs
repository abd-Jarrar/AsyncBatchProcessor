using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Handler.ProductValidation
{
    public class ProductQuantityHandler : ProductValidationHandler
    {
        protected override void Validate(Product product, int quantity)
        {
            if (product.StockQuantity < quantity)
            {
                throw new InvalidOperationException($"There is not enough stock for product {product.Id}.");
            }
        }
    }
}
