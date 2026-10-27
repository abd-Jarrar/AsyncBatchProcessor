using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Specifications.Products
{
    public class ProductInPriceRangeSpecification(decimal minPrice, decimal maxPrice)
    : ISpecificataion<Product>
    {
        public bool IsSatisfiedBy(Product entity)
        {
            return (entity.Price >= minPrice && entity.Price <= maxPrice);
        }
    }
}
