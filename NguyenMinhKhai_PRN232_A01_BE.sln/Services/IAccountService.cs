using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;
using Microsoft.Extensions.Configuration;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Services
{
    public interface IAccountService
    {
        IQueryable<AccountDTO> GetAll();
        IQueryable<Account> SearchAccounts(string searchTerm);
        Task<AccountDTO?> GetByIdAsync(int id);
        Task<Account?> GetByEmailAsync(string email);
        Task<AccountDTO> CreateAsync(CreateAccountDTO accountDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasNewsArticlesAsync(int id);
        Task<AccountDTO?> UpdateProfileAsync(int id, UpdateProfileDTO profile);
        Task<bool> VerifyPasswordAsync(string password, Account account);
        Task EnsureAdminAccountExists(IConfiguration configuration);
    }
} 