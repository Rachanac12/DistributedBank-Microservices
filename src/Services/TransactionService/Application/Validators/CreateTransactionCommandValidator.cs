using FluentValidation;
using TransactionService.Application.Commands;

namespace TransactionService.Application.Validators
{
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {
            RuleFor(x => x.FromAccountId)
                .NotEmpty()
                .WithMessage("FromAccountId is required");

            RuleFor(x => x.ToAccountId)
                .NotEmpty()
                .WithMessage("ToAccountId is required")
                .NotEqual(x => x.FromAccountId)
                .WithMessage("FromAccountId and ToAccountId must be different");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0");
        }
    }
}
