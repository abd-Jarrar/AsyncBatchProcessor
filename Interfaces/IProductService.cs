using AsyncBatchProcessor.Common;
using AsyncBatchProcessor.Models;
using AsyncBatchProcessor.Responses.V2;

namespace AsyncBatchProcessor.Interfaces
{
    public interface IProductService
    {
        public Task<Result<Guid>> CreateProductAsync(
            string productName, string sku, decimal productPrice, int? stockQuantity, CancellationToken ct);

        public Task<Result<bool>> DeleteProductByIdAsync(Guid productId, CancellationToken ct);

        public Task<Result<ProductResponse>> GetProductByIdAsync(Guid productId, CancellationToken ct);

        public Task<Result<ProductResponse>> GetProductByNameAsync(string productName, CancellationToken ct);

        public Task<Result<ProductResponse>> GetProductWithMaximumPriceAsync(CancellationToken ct);

        public Task<Result<ProductResponse>> GetProductWithMinimumPriceAsync(CancellationToken ct);

        public Task<List<ProductResponse>> GetAllProductsAsync(CancellationToken ct);

        public Task<List<ProductResponse>> GetProductWithStockAsync(int stockQuantity, CancellationToken ct);

        public Task<List<ProductResponse>> GetActiveProductsAsync(CancellationToken ct);
    }
}
