using AccountService.Application.Commands;
using AccountService.Data;
using AccountService.DTOs;
using AccountService.Models;
using AutoMapper;
using MediatR;

namespace AccountService.Application.Handlers
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
    {
        private readonly AccountDbContext _context;
        private readonly IMapper _mapper;

        public CreateAccountCommandHandler(AccountDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = request.AccountNumber,
                AccountType = request.AccountType,
                Balance = request.Balance,
                CustomerId = request.CustomerId
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AccountDto>(account);
        }
    }
}
