using AsyncBatchProcessor.Requests.V2.OrderItemRequests;
using FluentValidation;

namespace AsyncBatchProcessor.Validators.V2
{
    public class CreateOrderItemRequestValidator: AbstractValidator<CreateOrderItemRequest>
    {
        public CreateOrderItemRequestValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Order item quantity must be positive");
        }
    }
}
