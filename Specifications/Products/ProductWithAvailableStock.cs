using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Specifications.Products
{
    public class ProductWithAvailableStock(int quantity) : ISpecificataion<Product>
    {
        public bool IsSatisfiedBy(Product entity)
        {
            return entity.StockQuantity>=quantity;
        }
    }
}
