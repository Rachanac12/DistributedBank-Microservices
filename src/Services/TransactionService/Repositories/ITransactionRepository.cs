using TransactionService.Models;

namespace TransactionService.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(Guid id);
        Task<Transaction> CreateAsync(Transaction transaction);
    }
}
