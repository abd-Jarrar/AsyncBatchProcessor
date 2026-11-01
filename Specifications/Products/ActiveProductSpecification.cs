using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Specifications.Products
{
    public class ActiveProductSpecification : ISpecificataion<Product>
    {
        public bool IsSatisfiedBy(Product entity)
        {
            return entity.IsActive;
        }
    }
}
