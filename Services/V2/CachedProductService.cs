using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Services.V2
{
    public class CachedProductService(ProductService productService) : IProductService
    {
        public async Task<Guid> CreateProductAsync(string productName, string sku, decimal productPrice, int? stockQuantity, CancellationToken ct)
        {
            return await productService.CreateProductAsync(productName,sku,productPrice,stockQuantity,ct);
        }

        public async Task<bool> DeleteProductByIdAsync(Guid productId, CancellationToken ct)
        {
            return  await productService.DeleteProductByIdAsync(productId, ct);
        }

        public async Task<List<Product>> GetActiveProductsAsync(CancellationToken ct)
        {
            return await productService.GetActiveProductsAsync(ct);
        }

        public async Task<List<Product>> GetAllProductsAsync(CancellationToken ct)
        {
            Console.WriteLine("caching using the decorator ....");
            return await productService.GetAllProductsAsync(ct);
            

        }

        public async Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken ct)
        {
            return await productService.GetProductByIdAsync(productId, ct);
        }

        public async Task<Product?> GetProductByNameAsync(string productName, CancellationToken ct)
        {
            return await productService.GetProductByNameAsync(productName, ct);
        }

        public async Task<Product?> GetProductWithMaximumPriceAsync(CancellationToken ct)
        {
            return await productService.GetProductWithMaximumPriceAsync(ct);
        }

        public async Task<Product?> GetProductWithMinimumPriceAsync(CancellationToken ct)
        {
            return await productService.GetProductWithMinimumPriceAsync(ct);

        }

        public async Task<List<Product>> GetProductWithStockAsync(int stockQuantity, CancellationToken ct)
        {
            return await productService.GetProductWithStockAsync(stockQuantity,ct);
        }
    }
}
