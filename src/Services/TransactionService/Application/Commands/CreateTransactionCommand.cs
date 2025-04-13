using MediatR;
using TransactionService.DTOs;

namespace TransactionService.Application.Commands
{
    public class CreateTransactionCommand : IRequest<TransactionDto>
    {
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
