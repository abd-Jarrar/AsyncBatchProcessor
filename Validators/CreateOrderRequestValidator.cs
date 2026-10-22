using AsyncBatchProcessor.OrderRequests;
using FluentValidation;
namespace AsyncBatchProcessor.Validators
{
    public class CreateOrderRequestValidator: AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.Items).NotEmpty().WithMessage("the Order must contain at least one item");
            RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemRequestValidator());
        }
    }
}
