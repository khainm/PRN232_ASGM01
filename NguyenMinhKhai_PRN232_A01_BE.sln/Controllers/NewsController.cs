using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Authorization;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;
using NguyenMinhKhai_PRN232_A01_BE.sln.Repositories;
using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;
using AutoMapper;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using NguyenMinhKhai_PRN232_A01_BE.sln.Services;
using Microsoft.EntityFrameworkCore;
using NguyenMinhKhai_PRN232_A01_BE.sln.Data;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NewsController : ODataController
    {
        private readonly INewsService _newsService;
        private readonly ILogger<NewsController> _logger;
        private readonly ApplicationDbContext _context;

        public NewsController(INewsService newsService, ILogger<NewsController> logger, ApplicationDbContext context)
        {
            _newsService = newsService;
            _logger = logger;
            _context = context;
        }

        // OData Endpoints
        [EnableQuery(PageSize = 10, MaxTop = 100)]
        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 300)] // Cache for 5 minutes
        public IQueryable<NewsDTO> Get()
        {
            _logger.LogInformation("Getting all news");
            return _newsService.GetAll();
        }

        [EnableQuery(PageSize = 10, MaxTop = 100)]
        [AllowAnonymous]
        [HttpGet("active")]
        [ResponseCache(Duration = 300)]
        public IQueryable<NewsDTO> GetActive()
        {
            _logger.LogInformation("Getting active news");
            return _newsService.GetActiveNews();
        }

        [EnableQuery(PageSize = 10, MaxTop = 100)]
        [AllowAnonymous]
        [HttpGet("search")]
        public IQueryable<NewsDTO> Search([FromQuery] string term)
        {
            _logger.LogInformation($"Searching news with term: {term}");
            return _newsService.SearchNews(term);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ResponseCache(Duration = 300)]
        public async Task<ActionResult<NewsDTO>> GetById(int id)
        {
            _logger.LogInformation($"Getting news with ID: {id}");
            var news = await _newsService.GetByIdAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return Ok(news);
        }

        [Authorize(Policy = "RequireAdminOrStaffRole")]
        [HttpGet("by-account/{accountId}")]
        public IQueryable<NewsDTO> GetNewsByAccount(int accountId)
        {
            _logger.LogInformation($"Getting news for account ID: {accountId}");
            return _newsService.GetNewsByAccountId(accountId);
        }

        [Authorize(Policy = "RequireAdminOrStaffRole")]
        [HttpGet("filter-by-date")]
        public IQueryable<NewsDTO> FilterNewsByDate(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            _logger.LogInformation($"Filtering news from {startDate} to {endDate}");
            return _newsService.GetNewsByDateRange(startDate, endDate);
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpPost]
        public async Task<ActionResult<NewsDTO>> CreateNews([FromBody] CreateNewsDTO newsDto)
        {
            _logger.LogInformation("Creating new news article");
            var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var news = await _newsService.CreateAsync(newsDto, accountId);
            return CreatedAtAction(nameof(GetById), new { id = news.NewsId }, news);
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("{id}")]
        public async Task<ActionResult<NewsDTO>> UpdateNews(int id, [FromBody] UpdateNewsDTO newsDto)
        {
            _logger.LogInformation($"Updating news with ID: {id}");
            var news = await _newsService.UpdateAsync(id, newsDto);
            if (news == null)
            {
                return NotFound();
            }
            return Ok(news);
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            _logger.LogInformation($"Deleting news with ID: {id}");
            var result = await _newsService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [Authorize(Policy = "RequireStaffRole")]
        [EnableQuery]
        [HttpGet("my-news")]
        public IQueryable<NewsDTO> GetMyNews()
        {
            var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation($"Getting news for account ID: {accountId}");
            return _newsService.GetNewsByAccountId(accountId);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("statistics")]
        public async Task<ActionResult<NewsStatisticsDTO>> GetNewsStatistics(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            _logger.LogInformation($"Getting news statistics from {startDate} to {endDate}");
            var statistics = await _newsService.GetNewsStatisticsAsync(startDate, endDate);
            return statistics;
        }

        [HttpPost("{id}/increment-view")]
        public async Task<IActionResult> IncrementViewCount(int id)
        {
            _logger.LogInformation($"Incrementing view count for news ID: {id}");
            var result = await _newsService.IncrementViewCountAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("history")]
        [Authorize(Roles = "1")] // Chỉ Staff mới có quyền xem lịch sử tin tức của mình
        public async Task<ActionResult<object>> GetNewsHistory(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = "",
            [FromQuery] int? categoryId = null,
            [FromQuery] int? status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                _logger.LogInformation($"GetNewsHistory called by UserId: {userId} with params - Page: {page}, PageSize: {pageSize}, SearchTerm: {searchTerm}, CategoryId: {categoryId}, Status: {status}, StartDate: {startDate}, EndDate: {endDate}");

                var query = _context.NewsArticles
                    .Include(n => n.Category)
                    .Include(n => n.Tags)
                    .Where(n => n.AccountId == userId);

                _logger.LogInformation($"Initial query count for user {userId}: {await query.CountAsync()}");

                // Áp dụng các bộ lọc
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(n => n.Title.Contains(searchTerm) || 
                                           n.Content.Contains(searchTerm));
                    _logger.LogInformation($"Query count after searchTerm filter: {await query.CountAsync()}");
                }

                if (categoryId.HasValue)
                {
                    query = query.Where(n => n.CategoryId == categoryId.Value);
                    _logger.LogInformation($"Query count after categoryId filter: {await query.CountAsync()}");
                }

                if (status.HasValue)
                {
                    query = query.Where(n => n.Status == status.Value);
                    _logger.LogInformation($"Query count after status filter: {await query.CountAsync()}");
                }

                if (startDate.HasValue)
                {
                    query = query.Where(n => n.CreatedDate >= startDate.Value);
                    _logger.LogInformation($"Query count after startDate filter: {await query.CountAsync()}");
                }

                if (endDate.HasValue)
                {
                    query = query.Where(n => n.CreatedDate <= endDate.Value);
                    _logger.LogInformation($"Query count after endDate filter: {await query.CountAsync()}");
                }

                // Tính toán phân trang
                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                _logger.LogInformation($"Final query count for pagination: {totalItems}");

                // Create an empty list of the anonymous type outside the query
                var emptyTagsList = Enumerable.Empty<object>().Select(t => new { TagId = 0, Name = "" }).ToList();

                // Lấy dữ liệu cho trang hiện tại
                var news = await query
                    .OrderByDescending(n => n.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(n => new
                    {
                        n.NewsId,
                        n.Title,
                        n.Content,
                        n.Status,
                        n.CreatedDate,
                        n.UpdatedDate,
                        n.ViewCount,
                        n.IsFeatured,
                        Category = n.Category != null ? new
                        {
                            n.Category.CategoryId,
                            Name = n.Category.Name ?? ""
                        } : null,
                        Tags = n.Tags != null ? n.Tags.Select(t => new
                        {
                            t.TagId,
                            Name = t.Name ?? ""
                        }).ToList() : emptyTagsList // Use the local variable here
                    })
                    .ToListAsync();

                return Ok(new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize,
                    News = news
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 