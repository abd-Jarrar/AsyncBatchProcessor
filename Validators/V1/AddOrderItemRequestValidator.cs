using AsyncBatchProcessor.Requests.V1.OrderItemRequests;
using FluentValidation;

namespace AsyncBatchProcessor.Validators.V1
{
    public class AddOrderItemRequestValidator: AbstractValidator<AddOrderItemRequest>
    {
        public AddOrderItemRequestValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Order Item quantity must be positive");
        }
    }
}
