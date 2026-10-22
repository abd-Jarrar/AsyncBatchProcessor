namespace AsyncBatchProcessor.Requests.V1.ProductRequests
{
    public record UpdateProductRequest
    {
        public string? Name { get; set; } = null!;

        public string? SKU { get; set; } = null!;

        public decimal? Price { get; set; }

        public int? stockQuantity { get; set; }

        public bool? IsActive { get; set; }


    }
}
