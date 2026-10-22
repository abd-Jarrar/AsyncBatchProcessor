using AsyncBatchProcessor.Requests.V1.CustomerRequests;
using FluentValidation;

namespace AsyncBatchProcessor.Validators.V1
{
    public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Customer name is requied");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Custmoer Email is required");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Customer Phone is required")
                .Length(10, 13).WithMessage("Phone number should be between 10 - 13 number");

        }
    }
}
