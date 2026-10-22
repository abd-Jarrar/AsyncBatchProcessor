using Asp.Versioning;
using AsyncBatchProcessor.Models;
using AsyncBatchProcessor.Requests.V2.ProductRequests;
using AsyncBatchProcessor.Services.V2;
using Microsoft.AspNetCore.Mvc;

namespace AsyncBatchProcessor.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/V{version:apiVersion}/products")]
    public class ProductController(ProductService productService):ControllerBase
    {
        [HttpGet]
        [Route("productId:guid")]
        public async Task<IActionResult> GetProductById(Guid productId,CancellationToken ct)
        {
            var product = await productService.GetProductByIdAsync(productId, ct);
            if (product is null)
                return NotFound(new ProblemDetails
                {
                    Title = "product was not found",
                    Detail = $"product with the id {productId} was not found"
                });
            else
            {
                return Ok(product.ToResponse());
            }

        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetProductByName(string name, CancellationToken ct)
        {
            var product = await productService
                .GetProductByNameAsync(name, ct);

            if (product is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Product was not found",
                    Detail = $"No product with the name '{name}' was found."
                });
            }

            return Ok(product.ToResponse());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts( CancellationToken ct)
        {
          var products= await productService .GetAllProductsAsync(ct);
            if (products.Count == 0)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "There are no products",
                    Detail = "There are no products right now."
                });
            }
            else
            {
                return Ok(products.Select(p => p.ToResponse()).ToList());
               
            }

            
        }

        [HttpGet("max-price")]
        public async Task<IActionResult> GetProductWithMaximumPrice(CancellationToken ct)
        {
            var product = await productService
                .GetProductWithMaximumPriceAsync(ct);

            if (product is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "There are no products",
                    Detail = "There are no products right now."
                });
            }

            return Ok(product.ToResponse());
        }

        [HttpDelete]
        [Route("{productId:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid productId, CancellationToken ct)
        {
            var isDeleted = await productService.DeleteProductByIdAsync(productId, ct);

            if (isDeleted)
                return NoContent();
            else
            {
                return NotFound(new ProblemDetails
                {
                    Title = "prouct was not found",
                    Detail = $"Product with the id: {productId} was not found"
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddProduct(CreateProductRequest request,CancellationToken ct)
        {
            var id = await productService.CreateProductAsync(
    request.Name,
    request.SKU,
    request.Price,
    request.stockQuantity,
    ct);

            return CreatedAtAction(
                nameof(GetProductById),
                new { productId = id },
                new { id });
        }


        [HttpGet]
        [Route("stock-belowOrEqual-5")]
        public async Task<IActionResult> GetProductsWithStockBelow5(CancellationToken ct)
        {
            var products = await productService.GetProductWithStockAsync(5, ct);
            if (products.Count == 0)
                return NotFound(new ProblemDetails
                {
                    Title = "there is no products",
                    Detail = "there is no products with stock less than 5"
                });
            else
                return Ok(products);
        }


    }
}
