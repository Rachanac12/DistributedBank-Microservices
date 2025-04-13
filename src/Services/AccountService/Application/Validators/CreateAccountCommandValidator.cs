using AccountService.Application.Commands;
using FluentValidation;

namespace AccountService.Application.Validators
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId is required");

            RuleFor(x => x.Balance)
                .GreaterThanOrEqualTo(0).WithMessage("Balance must be non-negative");

            RuleFor(x => x.AccountType)
                .NotEmpty().WithMessage("Account type is required");
        }
    }
}
