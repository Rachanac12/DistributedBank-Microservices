using AutoMapper;
using MediatR;
using TransactionService.DTOs;
using TransactionService.Models;
using TransactionService.Repositories;

namespace TransactionService.Application.Commands
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;

        public CreateTransactionCommandHandler(ITransactionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                FromAccountId = request.FromAccountId,
                ToAccountId = request.ToAccountId,
                Amount = request.Amount
            };

            var created = await _repository.CreateAsync(transaction);
            return _mapper.Map<TransactionDto>(created);
        }
    }
}
