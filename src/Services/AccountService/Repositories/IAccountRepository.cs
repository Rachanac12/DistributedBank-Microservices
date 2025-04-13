using AccountService.Models;

namespace AccountService.Repositories
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(Guid id);
        Task<Account> CreateAsync(Account account);
    }
}
