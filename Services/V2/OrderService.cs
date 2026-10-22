using AsyncBatchProcessor.Data;
using AsyncBatchProcessor.Factories;
using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;
using AsyncBatchProcessor.Requests.V2.OrderItemRequests;
using AsyncBatchProcessor.Requests.V2.OrderRequests;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AsyncBatchProcessor.Services.V2
{
    public class OrderService(
        IOrderRepository orders,
        IProductRepository products,
        ICustomerRepository customers,
        ILogger<OrderService> logger,
        IUnitOfWork unitOfWork,
        SemaphoreSlim semaphoreSlim
        ,AppDbContext context,
        NotificationFactory notificationFactory)
    {
        public async Task<Guid> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct)
        {
            await semaphoreSlim.WaitAsync(ct);
            try
            {
                
                Customer?  customer = await customers.GetByIdAsync(request.CustomerId, ct);

                
                if (customer is null)
                    throw new KeyNotFoundException($"Customer with the id {request.CustomerId} was not found.");

                await unitOfWork.BeginTransactionAsync(ct);
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    Status = OrderStatus.Pending,
                    OrderItems = new List<OrderItem>()
                };

                foreach (var item in request.Items.OrderBy(i=>i.ProductId))
                {

                    var product =  await context.Products.FromSqlInterpolated($"SELECT * FROM Products WITH (UPDLOCK, ROWLOCK) WHERE Id = {item.ProductId}").FirstOrDefaultAsync(ct);

                    if (product is null)
                        throw new KeyNotFoundException($"Product with the id {item.ProductId} was not found.");

                    if (!product.IsActive)
                        throw new InvalidOperationException($"Product with the id {item.ProductId} is inactive.");

                    if (item.Quantity > product.StockQuantity)
                    {
                        logger.LogWarning("There is not enough stock for product {ProductId}", product.Id);
                        throw new InvalidOperationException($"There is not enough stock for product {product.Id}.");
                    }

                    product.StockQuantity -= item.Quantity;

                    order.OrderItems.Add(new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    });
                }

                await orders.AddAsync(order, ct);
                await unitOfWork.SaveChangesAsync(ct);
                await unitOfWork.CommitAsync(ct);
                logger.LogInformation("Order with the id: {OrderId} was created", order.Id);
                var notificationStrategy=notificationFactory.Create(NotificationType.Sms);
                notificationStrategy = new LoggingDecorator(notificationStrategy);
                await notificationStrategy.SendAsync($"the order with the id: {order.Id} was created successfully");
                return order.Id;
              
            }
            catch 
            {
                await unitOfWork.RollbackAsync(ct);
                throw;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task AddItemAsync(Guid orderId, AddOrderItemRequest request, CancellationToken ct)
        {
            if (request.Quantity <= 0)
                throw new ValidationException("Item quantity must be greater than zero.");

            var order = await orders.GetByIdAsync(orderId, ct);
            if (order is null)
                throw new KeyNotFoundException($"Order with the id {orderId} was not found.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Items can only be added to pending orders.");

            var product = await products.GetByIdAsync(request.ProductId, ct);
            if (product is null)
                throw new KeyNotFoundException($"Product with the id {request.ProductId} was not found.");

            if (!product.IsActive)
                throw new InvalidOperationException("Cannot add an inactive product to an order.");

            if (product.StockQuantity < request.Quantity)
            {
                logger.LogWarning("There is not enough stock for product {ProductId}", product.Id);
                throw new InvalidOperationException("There is not enough quantity in the stock.");
            }

            product.StockQuantity -= request.Quantity;

            order.OrderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price
            });

            await orders.SaveChangesAsync(ct);
        }

        public async Task<bool> RemoveItemAsync(Guid orderId, Guid productId, CancellationToken ct)
        {
            var order = await orders.GetByIdAsync(orderId, ct);
            if (order is null)
                throw new KeyNotFoundException($"Order with the id {orderId} was not found.");

            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);
            if (orderItem is null)
                throw new KeyNotFoundException($"Product with the id {productId} was not found in the order.");

            var product = await products.GetByIdAsync(orderItem.ProductId, ct);
            if (product is null || !product.IsActive)
                throw new InvalidOperationException("Failed to remove the item because the product is unavailable.");

            product.StockQuantity += orderItem.Quantity;
            order.OrderItems.Remove(orderItem);

            await orders.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> CancelOrderAsync(Guid orderId, CancellationToken ct)
        {
            var order = await orders.GetByIdAsync(orderId, ct);
            if (order is null)
                return false;

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("You can't cancel a cancelled order.");

            if (order.Status == OrderStatus.Completed)
                throw new InvalidOperationException("You can't cancel a completed order.");

            foreach (var orderItem in order.OrderItems)
            {
                var product = await products.GetByIdAsync(orderItem.ProductId, ct);
                if (product is null || !product.IsActive)
                    throw new InvalidOperationException("Failed to cancel the order because it contains unavailable products.");

                product.StockQuantity += orderItem.Quantity;
            }

            order.Status = OrderStatus.Cancelled;
            await orders.SaveChangesAsync(ct);
            logger.LogInformation("The order with id: {OrderId} was cancelled", orderId);
            return true;
        }

        public async Task UpdateStatus(Guid orderId, OrderStatus newStatus, CancellationToken ct)
        {
            var order = await orders.GetByIdAsync(orderId, ct);
            if (order is null)
                throw new KeyNotFoundException($"Order with the id {orderId} was not found.");

            var isValidTransition =
                (order.Status == OrderStatus.Pending && newStatus == OrderStatus.Shipped) ||
                (order.Status == OrderStatus.Shipped && newStatus == OrderStatus.Completed);

            if (!isValidTransition)
                throw new InvalidOperationException(
                    $"Cannot change the order status from {order.Status} to {newStatus}.");

            order.Status = newStatus;
            await orders.SaveChangesAsync(ct);
        }

        public Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken ct) =>
            orders.GetByIdAsync(orderId, ct);

        public Task<List<Order>> GetAllOrdersAsync(CancellationToken ct) =>
            orders.GetAllAsync(ct);

        public Task<List<Order>> GetOrdersCreatedInTheLast30DaysAsync(CancellationToken ct) =>
            orders.GetCreatedSinceAsync(DateTime.UtcNow.AddDays(-30), ct);

        public Task<List<Product>> GetTop5SellingProductsAsync(CancellationToken ct) =>
            orders.GetTop5SellingProductsAsync(ct);

        public async Task<decimal?> GetCustomerTotalSalesAsync(Guid customerId, CancellationToken ct)
        {
            var customer = await customers.GetByIdAsync(customerId, ct);
            if (customer is null)
                return null;

            var customerOrders = await orders.GetByCustomerIdAsync(customerId, ct);
            return customerOrders.Sum(o => o.TotalAmount);
        }
    }
}