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
    public class ProductController(CachedProductService productService):ControllerBase
    {
        [HttpGet]
        [Route("productId:guid")]
        public async Task<IActionResult> GetProductById(Guid productId,CancellationToken ct)
        {
            var result = await productService.GetProductByIdAsync(productId, ct);
            if (result.IsFailure)
                return NotFound(new ProblemDetails
                {
                    Title = "product was not found",
                    Detail = result.Error
                });
            else
            {
                return Ok(result.Value);
            }

        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetProductByName(string name, CancellationToken ct)
        {
            var result = await productService
                .GetProductByNameAsync(name, ct);

            if (result.IsFailure)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Product was not found",
                    Detail = result.Error
                });
            }

            return Ok(result.Value);
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
                return Ok(products);
               
            }

            
        }

        [HttpGet("max-price")]
        public async Task<IActionResult> GetProductWithMaximumPrice(CancellationToken ct)
        {
            var result = await productService
                .GetProductWithMaximumPriceAsync(ct);

            if (result.IsFailure)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "There are no products",
                    Detail = result.Error
                });
            }

            return Ok(result.Value);
        }

        [HttpDelete]
        [Route("{productId:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid productId, CancellationToken ct)
        {
            var result = await productService.DeleteProductByIdAsync(productId, ct);
            if (result.IsFailure)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "prouct was not found",
                    Detail = result.Error
                });
            }
            else
            {
                return NoContent();
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddProduct(CreateProductRequest request,CancellationToken ct)
        {
            var result = await productService.CreateProductAsync(
    request.Name,
    request.SKU,
    request.Price,
    request.stockQuantity,
    ct);
            if (result.IsSuccess)
                return CreatedAtAction(
                    nameof(GetProductById),
                    new { productId = result.Value },
                    new { result.Value });
            else
                return Conflict();
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
