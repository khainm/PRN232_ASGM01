using Microsoft.EntityFrameworkCore;
using NguyenMinhKhai_PRN232_A01_BE.sln.Data;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Account> GetAll()
        {
            return _context.Accounts
                .Include(a => a.NewsArticles)
                .OrderByDescending(a => a.CreatedDate)  // Default ordering
                .AsQueryable();
        }

        public IQueryable<Account> SearchAccounts(string searchTerm)
        {
            return _context.Accounts
                .Include(a => a.NewsArticles)
                .Where(a => a.Email.Contains(searchTerm) || 
                           a.FullName.Contains(searchTerm))
                .AsQueryable();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _context.Accounts
                .Include(a => a.NewsArticles)
                .FirstOrDefaultAsync(a => a.AccountId == id);
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _context.Accounts
                .Include(a => a.NewsArticles)
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Account> CreateAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account> UpdateAsync(Account account)
        {
            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return false;

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasNewsArticlesAsync(int id)
        {
            return await _context.NewsArticles.AnyAsync(n => n.AccountId == id);
        }
    }
} 