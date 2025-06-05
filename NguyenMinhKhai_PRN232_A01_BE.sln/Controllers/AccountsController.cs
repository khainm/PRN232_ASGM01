using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Authorization;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;
using NguyenMinhKhai_PRN232_A01_BE.sln.Repositories;
using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;
using AutoMapper;
using NguyenMinhKhai_PRN232_A01_BE.sln.Services;
using System.Security.Claims;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController : ODataController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountRepository accountRepository, IMapper mapper, IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _accountService = accountService;
            _logger = logger;
        }

        // OData Endpoints
        [EnableQuery]
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet]
        public IQueryable<AccountDTO> Get()
        {
            _logger.LogInformation("Getting all accounts");
            var accounts = _accountRepository.GetAll()
                .Select(a => new AccountDTO
                {
                    AccountId = a.AccountId,
                    Email = a.Email,
                    FullName = a.FullName,
                    Role = a.Role,
                    Status = a.Status,
                    NewsCount = a.NewsArticles.Count
                });
            return accounts;
        }

        // RESTful Endpoints
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<AccountDTO>> GetById(int id)
        {
            _logger.LogInformation($"Getting account with ID: {id}");
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
                return NotFound();

            var accountDto = new AccountDTO
            {
                AccountId = account.AccountId,
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role,
                Status = account.Status,
                NewsCount = account.NewsArticles.Count
            };

            return Ok(accountDto);
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<AccountDTO>> Create([FromBody] CreateAccountDTO accountDto)
        {
            _logger.LogInformation("Creating new account");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var account = _mapper.Map<Account>(accountDto);
            var createdAccount = await _accountRepository.CreateAsync(account);
            
            var resultDto = new AccountDTO
            {
                AccountId = createdAccount.AccountId,
                Email = createdAccount.Email,
                FullName = createdAccount.FullName,
                Role = createdAccount.Role,
                Status = createdAccount.Status,
                NewsCount = 0 // New account has no news
            };

            return CreatedAtAction(nameof(GetById), new { id = createdAccount.AccountId }, resultDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<AccountDTO>> Update(int id, [FromBody] UpdateAccountDTO accountDto)
        {
            _logger.LogInformation($"Updating account with ID: {id}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingAccount = await _accountRepository.GetByIdAsync(id);
            if (existingAccount == null)
                return NotFound();

            _mapper.Map(accountDto, existingAccount);
            var updatedAccount = await _accountRepository.UpdateAsync(existingAccount);

            var resultDto = new AccountDTO
            {
                AccountId = updatedAccount.AccountId,
                Email = updatedAccount.Email,
                FullName = updatedAccount.FullName,
                Role = updatedAccount.Role,
                Status = updatedAccount.Status,
                NewsCount = updatedAccount.NewsArticles.Count
            };

            return Ok(resultDto);
        }

        [HttpPut("admin/{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<AccountDTO>> AdminUpdateAccount(int id, [FromBody] AdminUpdateAccountDTO accountDto)
        {
            _logger.LogInformation($"Admin updating account with ID: {id}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingAccount = await _accountRepository.GetByIdAsync(id);
            if (existingAccount == null)
                return NotFound();

            _mapper.Map(accountDto, existingAccount);

            var updatedAccount = await _accountRepository.UpdateAsync(existingAccount);

            var resultDto = _mapper.Map<AccountDTO>(updatedAccount);

            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation($"Deleting account with ID: {id}");
            var hasNews = await _accountRepository.HasNewsArticlesAsync(id);
            if (hasNews)
                return BadRequest("Cannot delete account that has associated news articles.");

            var result = await _accountRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<AccountDTO>> GetProfile()
        {
            var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation($"Getting profile for account ID: {accountId}");
            var account = await _accountService.GetByIdAsync(accountId);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<AccountDTO>> UpdateProfile([FromBody] UpdateProfileDTO profile)
        {
            var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation($"Updating profile for account ID: {accountId}");
            var account = await _accountService.UpdateProfileAsync(accountId, profile);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }
    }
}