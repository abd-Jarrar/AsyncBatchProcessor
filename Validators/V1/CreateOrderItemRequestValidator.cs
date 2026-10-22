using AsyncBatchProcessor.Requests.V1.OrderItemRequests;
using FluentValidation;

namespace AsyncBatchProcessor.Validators.V1
{
    public class CreateOrderItemRequestValidator: AbstractValidator<CreateOrderItemRequest>
    {
        public CreateOrderItemRequestValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Order item quantity must be positive");
        }
    }
}
