using AsyncBatchProcessor.OrderItemRequests;
using FluentValidation;

namespace AsyncBatchProcessor.Validators
{
    public class CreateOrderItemRequestValidator: AbstractValidator<CreateOrderItemRequest>
    {
        public CreateOrderItemRequestValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Order item quantity must be positive");
        }
    }
}
