using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NguyenMinhKhai_PRN232_A01_BE.sln.Data;
using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;
// Add using for hashing library if using one (e.g., BCrypt.Net-Core)
// using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;

        public AccountService(ApplicationDbContext context, IMapper mapper, ILogger<AccountService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public IQueryable<AccountDTO> GetAll()
        {
            _logger.LogInformation("Getting all accounts");
            return _context.Accounts
                .Select(a => new AccountDTO
                {
                    AccountId = a.AccountId,
                    Email = a.Email,
                    FullName = a.FullName,
                    Role = a.Role,
                    Status = a.Status,
                    NewsCount = a.NewsArticles.Count
                });
        }

        public IQueryable<Account> SearchAccounts(string searchTerm)
        {
            _logger.LogInformation($"Searching accounts with term: {searchTerm}");
            return _context.Accounts.Where(a => 
                a.Email.Contains(searchTerm) || 
                a.FullName.Contains(searchTerm));
        }

        public async Task<AccountDTO?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting account with ID: {id}");
            var account = await _context.Accounts
                .Include(a => a.NewsArticles)
                .FirstOrDefaultAsync(a => a.AccountId == id);
            
            if (account == null)
                return null;

            return new AccountDTO
            {
                AccountId = account.AccountId,
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role,
                Status = account.Status,
                NewsCount = account.NewsArticles.Count
            };
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            _logger.LogInformation($"Getting account with email: {email}");
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<AccountDTO> CreateAsync(CreateAccountDTO accountDto)
        {
            try
            {
                _logger.LogInformation($"Creating new account with email: {accountDto.Email}");
                
                // Validate input
                if (string.IsNullOrEmpty(accountDto.Email) || string.IsNullOrEmpty(accountDto.Password) || string.IsNullOrEmpty(accountDto.FullName))
                {
                    throw new ArgumentException("Email, password, and full name are required");
                }

                // Check if email already exists
                var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == accountDto.Email);
                if (existingAccount != null)
                {
                    throw new InvalidOperationException($"Email {accountDto.Email} already exists");
                }

                var account = _mapper.Map<Account>(accountDto);
                _logger.LogInformation("Mapped DTO to Account model");

                // Set default values
                account.Status = 1; // Active
                account.CreatedDate = DateTime.UtcNow;
                _logger.LogInformation("Set default values");

                try
                {
                    _context.Accounts.Add(account);
                    _logger.LogInformation("Account added to context");

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Account saved to DB");

                    var createdAccountDto = _mapper.Map<AccountDTO>(account);
                    createdAccountDto.NewsCount = 0;

                    return createdAccountDto;
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Database error while creating account");
                    throw new InvalidOperationException("Error saving account to database", dbEx);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateAsync");
                throw;
            }
        }

        public async Task EnsureAdminAccountExists(IConfiguration configuration)
        {
            var adminEmail = configuration["AdminAccount:Email"];
            var adminPassword = configuration["AdminAccount:Password"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                _logger.LogWarning("Admin account configuration is missing in appsettings.json");
                return;
            }

            var existingAdmin = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == adminEmail);
            if (existingAdmin == null)
            {
                _logger.LogInformation("Creating admin account from appsettings.json");
                var adminAccount = new Account
                {
                    Email = adminEmail,
                    Password = adminPassword,
                    FullName = "System Administrator",
                    Role = 0, // Admin role
                    Status = 1, // Active
                    CreatedDate = DateTime.UtcNow
                };

                _context.Accounts.Add(adminAccount);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Admin account created successfully");
            }
            else
            {
                _logger.LogInformation("Admin account already exists");
            }
        }

        public async Task<AccountDTO?> UpdateProfileAsync(int id, UpdateProfileDTO accountDto)
        {
            _logger.LogInformation($"Updating account profile with ID: {id}");
            var account = await _context.Accounts
                .Include(a => a.NewsArticles)
                .FirstOrDefaultAsync(a => a.AccountId == id);
                
            if (account == null)
            {   
                _logger.LogWarning($"Account with ID {id} not found for profile update.");
                return null;
            }

            // Verify current password
            if (accountDto.CurrentPassword != account.Password)
            {
                _logger.LogWarning($"Invalid current password for account ID: {id}");
                return null;
            }

            // Update profile fields that are allowed to be updated via this DTO
            account.FullName = accountDto.FullName;

            // Update password if provided and confirmed
            if (!string.IsNullOrEmpty(accountDto.NewPassword))
            {   
                if (accountDto.NewPassword != accountDto.ConfirmPassword)
                {
                    _logger.LogWarning($"New password and confirmation do not match for account ID: {id}.");
                    return null;
                }
                account.Password = accountDto.NewPassword;
            }

            await _context.SaveChangesAsync();

            // Map the updated account model back to DTO
            var updatedAccountDto = _mapper.Map<AccountDTO>(account);
            updatedAccountDto.NewsCount = account.NewsArticles.Count;

            return updatedAccountDto;
        }

        // This method is likely for Admin to update account details including role and status
         public async Task<AccountDTO?> AdminUpdateAsync(int id, AdminUpdateAccountDTO accountDto)
        {
             _logger.LogInformation($"Admin updating account with ID: {id}");
            var account = await _context.Accounts
                .Include(a => a.NewsArticles)
                .FirstOrDefaultAsync(a => a.AccountId == id);
                
            if (account == null)
            {   
                _logger.LogWarning($"Account with ID {id} not found for admin update.");
                return null;
            }

            // Update profile fields
            account.FullName = accountDto.FullName;
            account.Role = accountDto.Role; // Admin can update role
            account.Status = accountDto.Status; // Admin can update status

             // Password is not updated via this DTO

            await _context.SaveChangesAsync();

            // Map the updated account model back to DTO
            var updatedAccountDto = _mapper.Map<AccountDTO>(account);
            updatedAccountDto.NewsCount = account.NewsArticles.Count;

            return updatedAccountDto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting account with ID: {id}");
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {   
                 _logger.LogWarning($"Account with ID {id} not found for deletion.");
                return false;
            }
            // Consider adding check if account has associated data (like NewsArticles) before deleting

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
             _logger.LogInformation($"Account with ID {id} deleted successfully.");
            return true;
        }

        public async Task<bool> HasNewsArticlesAsync(int id)
        {
            _logger.LogInformation($"Checking if account {id} has news articles");
            return await _context.NewsArticles.AnyAsync(n => n.AccountId == id);
        }

        // Placeholder for strong password hashing
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[salt.Length + passwordBytes.Length];
            Buffer.BlockCopy(salt, 0, saltedPassword, 0, salt.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, salt.Length, passwordBytes.Length);
            var hash = sha256.ComputeHash(saltedPassword);
            var result = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);
            return Convert.ToBase64String(result);
        }

         // Placeholder for password verification using hashing
        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            try
            {
                var storedBytes = Convert.FromBase64String(storedHashedPassword);
                var salt = new byte[16];
                Buffer.BlockCopy(storedBytes, 0, salt, 0, salt.Length);
                var passwordBytes = Encoding.UTF8.GetBytes(inputPassword);
                var saltedPassword = new byte[salt.Length + passwordBytes.Length];
                Buffer.BlockCopy(salt, 0, saltedPassword, 0, salt.Length);
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, salt.Length, passwordBytes.Length);
                using var sha256 = SHA256.Create();
                var hash = sha256.ComputeHash(saltedPassword);
                var result = new byte[salt.Length + hash.Length];
                Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);
                return Convert.ToBase64String(result) == storedHashedPassword;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> VerifyPasswordAsync(string password, Account account)
        {
            _logger.LogInformation($"Verifying password for account with ID: {account.AccountId}");
            return password == account.Password;
        }
    }
} 