using NguyenMinhKhai_PRN232_A01_BE.sln.Models;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Repositories
{
    public interface IAccountRepository
    {
        IQueryable<Account> GetAll();
        IQueryable<Account> SearchAccounts(string searchTerm);
        Task<Account?> GetByIdAsync(int id);
        Task<Account?> GetByEmailAsync(string email);
        Task<Account> CreateAsync(Account account);
        Task<Account> UpdateAsync(Account account);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasNewsArticlesAsync(int id);
    }
} 