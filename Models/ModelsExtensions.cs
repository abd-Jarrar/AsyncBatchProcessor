using AsyncBatchProcessor.Responses.V1;
using System.Reflection.Metadata.Ecma335;

namespace AsyncBatchProcessor.Models
{
    public static class ModelsExtensions
    {
        extension(Product product)
        {
            public ProductResponse ToResponse()
            {
                return new ProductResponse(
                     product.Name ,
                     product.SKU ,
                     product.Price ,
                     product.StockQuantity ,
                     product.IsActive 
                     );
            }
        }
        extension(Order order)
        {
            public OrderResponse ToResponse()
            {
                return new OrderResponse
                    (
                    order.Id,
                    order.CustomerId,
                    order.Status,
                    order.TotalAmount,
                    order.CreatedAt
                    );
            }
        }
        extension(Customer customer)
        {
            public CustomerResponse ToResponse()
            {
                return new CustomerResponse
                    (
                    customer.Name,
                    customer.Email,
                    customer.Phone
                    );
            }
        }
    }
}
