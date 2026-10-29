using AsyncBatchProcessor.Common;
using AsyncBatchProcessor.Controllers.V2;
using AsyncBatchProcessor.Data;
using AsyncBatchProcessor.Factories;
using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;
using AsyncBatchProcessor.Requests.V2.OrderItemRequests;
using AsyncBatchProcessor.Requests.V2.OrderRequests;
using AsyncBatchProcessor.Responses.V2;
using AsyncBatchProcessor.Specifications.Products;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

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
        NotificationFactory notificationFactory,
        IProductValidationHandler productValidationHandler,
        ActiveProductSpecification activeProductSpecification,
        IPaymentService paymentService)
    {
        public async Task<Result<Guid>> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct)
        {
            await semaphoreSlim.WaitAsync(ct);
            try
            {
                
                Customer?  customer = await customers.GetByIdAsync(request.CustomerId, ct);


                if (customer is null)
                    return Result<Guid>.Failure($"Customer with the id {request.CustomerId} was not found.", StatusCodes.Status404NotFound);

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
                        return Result<Guid>.Failure($"Product with the id {item.ProductId} was not found.", StatusCodes.Status404NotFound);

                    productValidationHandler.Handle(product,item.Quantity);

                    product.StockQuantity -= item.Quantity;

                    order.OrderItems.Add(new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    });
                    await paymentService.PayAsync(item.Quantity*product.Price);
                }

                await orders.AddAsync(order, ct);
                await unitOfWork.SaveChangesAsync(ct);
                await unitOfWork.CommitAsync(ct);
                
                logger.LogInformation("Order with the id: {OrderId} was created", order.Id);
                var notificationStrategy=notificationFactory.Create(NotificationType.Sms);
                notificationStrategy = new LoggingDecorator(notificationStrategy);
                await notificationStrategy.SendAsync($"the order with the id: {order.Id} was created successfully");
                return Result<Guid>.Success(order.Id);
              
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

        public async Task<Result<bool>> AddItemAsync(Guid orderId, AddOrderItemRequest request, CancellationToken ct)
        {
            
            var order = await orders.GetByIdAsync(orderId, ct);
            if (order is null)
                return Result<bool>.Failure($"Order with the id {orderId} was not found.", StatusCodes.Status404NotFound);

            if (order.Status != OrderStatus.Pending)
                return Result<bool>.Failure("Items can only be added to pending orders.", StatusCodes.Status422UnprocessableEntity);


            var product = await products.GetByIdAsync(request.ProductId, ct);
            if (product is null)
                return Result<bool>.Failure($"Product with the id {request.ProductId} was not found.",StatusCodes.Status404NotFound);

            productValidationHandler.Handle(product,request.Quantity);

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
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> RemoveItemAsync(Guid orderId, Guid productId, CancellationToken ct)
        {
            var order = await orders.GetByIdAsync(orderId, ct);
            if (order is null)
                return Result<bool>.Failure($"Order with the id {orderId} was not found.", StatusCodes.Status404NotFound);

            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);
            if (orderItem is null)
                return Result<bool>.Failure($"Product with the id {productId} was not found in the order.", StatusCodes.Status404NotFound);

            var product = await products.GetByIdAsync(orderItem.ProductId, ct);
            if (product is null || !activeProductSpecification.IsSatisfiedBy(product))
                return Result<bool>.Failure("Failed to remove the item because the product is unavailable.", StatusCodes.Status404NotFound);


            product.StockQuantity += orderItem.Quantity;
            order.OrderItems.Remove(orderItem);

            await orders.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> CancelOrderAsync(Guid orderId, CancellationToken ct)
        {
            var order = await orders.GetByIdAsync(orderId, ct);
            if (order is null)
                return Result<bool>.Failure($"Order with the id {orderId} was not found.", StatusCodes.Status404NotFound);


            if (order.Status == OrderStatus.Cancelled)
                return Result<bool>.Failure("You can't cancel a cancelled order.",StatusCodes.Status409Conflict);


            if (order.Status == OrderStatus.Completed)
                return Result<bool>.Failure("You can't cancel a completed order.",StatusCodes.Status422UnprocessableEntity);


            foreach (var orderItem in order.OrderItems)
            {
                var product = await products.GetByIdAsync(orderItem.ProductId, ct);
                if (product is null || !activeProductSpecification.IsSatisfiedBy(product))
                    return Result<bool>.Failure("Failed to cancel the order because it contains unavailable products.",StatusCodes.Status404NotFound);


                product.StockQuantity += orderItem.Quantity;
            }

            order.Status = OrderStatus.Cancelled;
            await orders.SaveChangesAsync(ct);
            logger.LogInformation("The order with id: {OrderId} was cancelled", orderId);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdateStatus(Guid orderId, OrderStatus newStatus, CancellationToken ct)
        {
            var order = await orders.GetByIdAsync(orderId, ct);
            if (order is null)
                return Result<bool>.Failure($"Order with the id {orderId} was not found.", StatusCodes.Status404NotFound);

            var isValidTransition =
                (order.Status == OrderStatus.Pending && newStatus == OrderStatus.Shipped) ||
                (order.Status == OrderStatus.Shipped && newStatus == OrderStatus.Completed);

            if (!isValidTransition)
                return Result<bool>.Failure($"Cannot change the order status from {order.Status} to {newStatus}.", StatusCodes.Status422UnprocessableEntity);

            order.Status = newStatus;
            await orders.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }

        public async Task<Order?> GetOrderByIdForUpdateAsync(Guid orderId, CancellationToken ct)
        {
            var order=await orders.GetByIdAsync(orderId, ct);
            return order;
        }

        public async Task<Result<OrderResponse>> GetOrderByIdForReadAsync(Guid orderId, CancellationToken ct)
        {
            var order = await orders.GetByIdAsync(orderId, ct);
            if(order is null)
                return Result<OrderResponse>.Failure($"Order with the id {orderId} was not found.", StatusCodes.Status404NotFound);
            else 
                return Result<OrderResponse>.Success(order.ToResponse());
        }

        public async Task<List<OrderResponse>> GetAllOrdersAsync(CancellationToken ct)
        {
            var result=await orders.GetAllAsync(ct);
            return result.Select(o => o.ToResponse()).ToList();
        }

        public async Task<List<OrderResponse>> GetOrdersCreatedInTheLast30DaysAsync(CancellationToken ct)
        {
            var result=await orders.GetCreatedSinceAsync(DateTime.UtcNow.AddDays(-30), ct);
            return result.Select(o => o.ToResponse()).ToList();
        }
        public async Task<List<ProductResponse>> GetTop5SellingProductsAsync(CancellationToken ct)
        {

           var result=await orders.GetTop5SellingProductsAsync(ct);
           return result.Select(o => o.ToResponse()).ToList();
        }

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