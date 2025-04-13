using AccountService.DTOs;
using MediatR;

namespace AccountService.Application.Commands
{
    public class CreateAccountCommand : IRequest<AccountDto>
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public Guid CustomerId { get; set; }
    }
}
