using Asp.Versioning;
using AsyncBatchProcessor.Data;
using AsyncBatchProcessor.Exceptions;
using AsyncBatchProcessor.Factories;
using AsyncBatchProcessor.Handler.ProductValidation;
using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Repositories;
using AsyncBatchProcessor.Services;
using AsyncBatchProcessor.Services.V2;
using AsyncBatchProcessor.Strategies;
using AsyncBatchProcessor.ThridPartyService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.InstanceName = "App1";
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CachedProductService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<NotificationFactory>();
builder.Services.AddScoped<SmsNotification>();
builder.Services.AddScoped<EmailNotification>();
builder.Services.AddScoped<PushNotification>();
builder.Services.AddScoped<PricingStrategyFactory>();
builder.Services.AddScoped<NormalPricingStrategy>();
builder.Services.AddScoped<VipPricingStrategy>();
builder.Services.AddScoped<BlackFridayPricingStrategy>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddSingleton<SemaphoreSlim>(new SemaphoreSlim(5, 5));
builder.Services.AddScoped<ProductActiveHandler>();
builder.Services.AddScoped<ProductQuantityHandler>();
builder.Services.AddScoped<IProductValidationHandler>(sp =>
{
    var activeHandler =
        sp.GetRequiredService<ProductActiveHandler>();

    var stockHandler =
        sp.GetRequiredService<ProductQuantityHandler>();

    activeHandler.SetNextHandler(stockHandler);

    return activeHandler;
});
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddMvc();
builder.Services.AddDbContext<AppDbContext>((options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddSingleton<ThirdPartyPayment>();
builder.Services.AddScoped<IPaymentService, PaymentAdapterService>();
var app = builder.Build();
app.MapControllers();
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseExceptionHandler();
app.MapControllers();

app.Run();
