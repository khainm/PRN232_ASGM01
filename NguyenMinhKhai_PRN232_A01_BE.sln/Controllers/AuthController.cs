using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;
using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;
using AutoMapper;
using NguyenMinhKhai_PRN232_A01_BE.sln.Services;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAccountService accountService, IConfiguration configuration, IMapper mapper, ILogger<AuthController> logger)
        {
            _accountService = accountService;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            try 
            {
                _logger.LogInformation("Register endpoint hit.");
                _logger.LogInformation($"Incoming registration data: Email={model.Email}, FullName={model.FullName}, Role={model.Role}");

                // Validate model
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state");
                    return BadRequest(ModelState);
                }

                // Validate email format
                if (!IsValidEmail(model.Email))
                {
                    _logger.LogWarning($"Invalid email format: {model.Email}");
                    return BadRequest(new { message = "Invalid email format" });
                }

                // Validate password strength
                if (!IsValidPassword(model.Password))
                {
                    _logger.LogWarning("Password does not meet requirements");
                    return BadRequest(new { message = "Password must contain at least 1 uppercase letter, 1 lowercase letter, and 1 number" });
                }

                var existingAccount = await _accountService.GetByEmailAsync(model.Email);
                if (existingAccount != null)
                {
                    _logger.LogWarning($"Registration failed: Email {model.Email} already exists.");
                    return BadRequest(new { message = "Email already exists" });
                }
                _logger.LogInformation($"Email {model.Email} is available.");

                CreateAccountDTO createAccountDto;
                try 
                {
                    _logger.LogInformation("Attempting to map RegisterDTO to CreateAccountDTO...");
                    createAccountDto = _mapper.Map<CreateAccountDTO>(model);
                    _logger.LogInformation($"Successfully mapped RegisterDTO to CreateAccountDTO: {System.Text.Json.JsonSerializer.Serialize(createAccountDto)}");
                }
                catch (Exception mapEx)
                {
                    _logger.LogError(mapEx, "Error during mapping: {Message}", mapEx.Message);
                    return StatusCode(500, new { message = "Error during data mapping" });
                }

                try
                {
                    _logger.LogInformation("Attempting to create account via AccountService...");
                    var createdAccountDto = await _accountService.CreateAsync(createAccountDto);
                    _logger.LogInformation($"Account created successfully via AccountService. AccountId: {createdAccountDto.AccountId}.");

                    _logger.LogInformation($"Fetching created account model with ID: {createdAccountDto.AccountId} for token generation.");
                    var createdAccountModel = await _accountService.GetByIdAsync(createdAccountDto.AccountId);
                    
                    if (createdAccountModel == null)
                    {
                        _logger.LogError($"Failed to retrieve created account model with ID: {createdAccountDto.AccountId} after creation.");
                        return StatusCode(500, new { message = "Error retrieving created account after creation" });
                    }
                    _logger.LogInformation($"Retrieved created account model. Generating token...");

                    var token = GenerateJwtToken(_mapper.Map<Account>(createdAccountModel));
                    _logger.LogInformation("JWT Token generated.");

                    _logger.LogInformation("Registration successful. Returning response.");
                    return Ok(new LoginResponseDTO
                    {
                        Token = token,
                        Account = createdAccountDto
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during account creation: {Message}", ex.Message);
                    return StatusCode(500, new { message = "Error during account creation", details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during registration: {Message}", ex.Message);
                return StatusCode(500, new { message = "An internal error occurred during registration.", details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            _logger.LogInformation("Login endpoint hit.");
            _logger.LogInformation($"Incoming login data: Email={model.Email}");

            var account = await _accountService.GetByEmailAsync(model.Email);
            
            if (account == null)
            {
                _logger.LogWarning($"Login failed: Account with email {model.Email} not found.");
                return Unauthorized(new { message = "Invalid email or password" });
            }
            _logger.LogInformation($"Account found: AccountId={account.AccountId}, Status={account.Status}");
            
            if (!await _accountService.VerifyPasswordAsync(model.Password, account))
            {
                _logger.LogWarning($"Login failed for account {account.AccountId}: Invalid password.");
                return Unauthorized(new { message = "Invalid email or password" });
            }
            _logger.LogInformation($"Password verified successfully for account {account.AccountId}.");

            if (account.Status != 1)
            {   
                _logger.LogWarning($"Login failed for account {account.AccountId}: Account is not active.");
                return Unauthorized(new { message = "Account is not active" });
            }
            _logger.LogInformation($"Account {account.AccountId} is active. Generating token...");

            var token = GenerateJwtToken(account);
            var accountDto = _mapper.Map<AccountDTO>(account);

            _logger.LogInformation($"Login successful for account {account.AccountId}. Returning response.");
            return Ok(new LoginResponseDTO
            {
                Token = token,
                Account = accountDto
            });
        }

        private string GenerateJwtToken(Account account)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
            var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");
            var jwtExpiryInMinutes = _configuration["Jwt:ExpiryInMinutes"] ?? "60";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Name, account.FullName),
                new Claim(ClaimTypes.Role, account.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtExpiryInMinutes)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            using var sha256 = SHA256.Create();
            var hashedInputBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
            var hashedInputBase64 = Convert.ToBase64String(hashedInputBytes);
            return hashedInputBase64 == storedPassword;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            return !string.IsNullOrEmpty(password) &&
                   password.Length >= 6 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit);
        }
    }

    public class LoginDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public int Role { get; set; } 
    }
} 