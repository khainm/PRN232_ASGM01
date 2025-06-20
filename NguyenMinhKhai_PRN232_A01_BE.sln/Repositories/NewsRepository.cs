using Microsoft.EntityFrameworkCore;
using NguyenMinhKhai_PRN232_A01_BE.sln.Data;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;
using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly ApplicationDbContext _context;

        public NewsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<News> GetAll()
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .OrderByDescending(n => n.CreatedDate);
        }

        public IQueryable<News> GetActiveNews()
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .Where(n => n.Status == 1)
                .OrderByDescending(n => n.CreatedDate);
        }

        public IQueryable<News> SearchNews(string searchTerm)
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .Where(n => n.Status == 1 && (n.Title.Contains(searchTerm) ||
                           n.Content.Contains(searchTerm) ||
                           (n.Category != null && n.Category.Name.Contains(searchTerm))))
                .OrderByDescending(n => n.CreatedDate);
        }

        public async Task<News?> GetByIdAsync(int id)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsId == id);
        }

        public async Task<News?> GetActiveByIdAsync(int id)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsId == id && n.Status == 1);
        }

        public async Task<News> CreateAsync(CreateNewsDTO newsDto, int accountId)
        {
            var news = new News
            {
                Title = newsDto.Title,
                Content = newsDto.Content,
                CategoryId = newsDto.CategoryId,
                AccountId = accountId,
                Status = 1, // Active
                CreatedDate = DateTime.Now,
                ViewCount = 0
            };

            _context.NewsArticles.Add(news);
            await _context.SaveChangesAsync();

            // Add tags
            if (newsDto.TagIds != null && newsDto.TagIds.Any())
            {
                foreach (var tagId in newsDto.TagIds)
                {
                    var tag = await _context.Tags.FindAsync(tagId);
                    if (tag != null)
                    {
                        news.Tags.Add(tag);
                    }
                }
                await _context.SaveChangesAsync();
            }

            return news;
        }

        public async Task<News?> UpdateAsync(int id, UpdateNewsDTO newsDto)
        {
            var news = await _context.NewsArticles
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsId == id);

            if (news == null)
            {
                return null;
            }

            news.Title = newsDto.Title;
            news.Content = newsDto.Content;
            news.CategoryId = newsDto.CategoryId;
            news.Status = newsDto.Status;

            // Update tags
            if (newsDto.TagIds != null)
            {
                news.Tags.Clear();
                foreach (var tagId in newsDto.TagIds)
                {
                    var tag = await _context.Tags.FindAsync(tagId);
                    if (tag != null)
                    {
                        news.Tags.Add(tag);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return news;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var news = await _context.NewsArticles.FindAsync(id);
            if (news == null)
                return false;
            _context.NewsArticles.Remove(news);
            await _context.SaveChangesAsync();
            return true;
        }

        public IQueryable<News> GetNewsByAccountId(int accountId)
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .Where(n => n.AccountId == accountId)
                .OrderByDescending(n => n.CreatedDate);
        }

        public IQueryable<News> GetNewsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .OrderByDescending(n => n.CreatedDate);
        }

        public IQueryable<News> GetNewsStatistics(DateTime startDate, DateTime endDate)
        {
            // This is a placeholder. You may want to implement actual statistics logic here.
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .OrderByDescending(n => n.CreatedDate);
        }

        public async Task<IEnumerable<News>> GetByCategoryAsync(int categoryId)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .Where(n => n.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> GetByTagAsync(int tagId)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .Where(n => n.Tags.Any(t => t.TagId == tagId))
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> SearchNewsAsync(string keyword)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .Where(n => n.Title.Contains(keyword) || 
                           n.Content.Contains(keyword) || 
                           (n.Category != null && n.Category.Name.Contains(keyword)))
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> GetFeaturedNewsAsync()
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .Where(n => n.IsFeatured)
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> GetNewsByStatusAsync(int status)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .Where(n => n.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> GetNewsByAccountAsync(int accountId)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Include(n => n.Tags)
                .Where(n => n.AccountId == accountId)
                .ToListAsync();
        }

        public async Task<NewsStatisticsDTO> GetNewsStatisticsAsync()
        {
            var totalNews = await _context.NewsArticles.CountAsync();
            var activeNews = await _context.NewsArticles.CountAsync(n => n.Status == 1);
            var featuredNews = await _context.NewsArticles.CountAsync(n => n.IsFeatured);
            var totalViews = await _context.NewsArticles.SumAsync(n => n.ViewCount);

            return new NewsStatisticsDTO
            {
                TotalNews = totalNews,
                ActiveNews = activeNews,
                FeaturedNews = featuredNews,
                TotalViews = totalViews
            };
        }

        public async Task<bool> IncrementViewCountAsync(int id)
        {
            var news = await _context.NewsArticles.FindAsync(id);
            if (news == null || news.Status != 1) // Only increment for active news
                return false;

            news.ViewCount++;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<NewsStatisticsDTO> GetNewsStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var news = await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Account)
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .ToListAsync();

            var statistics = new NewsStatisticsDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalNews = news.Count,
                ActiveNews = news.Count(n => n.Status == 1),
                InactiveNews = news.Count(n => n.Status == 0),
                FeaturedNews = news.Count(n => n.Status == 2),
                TotalViews = news.Sum(n => n.ViewCount),
                CategoryStatistics = news
                    .GroupBy(n => new { n.CategoryId, n.Category?.Name })
                    .Select(g => new CategoryStatisticsDTO
                    {
                        CategoryId = g.Key.CategoryId,
                        CategoryName = g.Key.Name ?? "Uncategorized",
                        NewsCount = g.Count(),
                        ViewCount = g.Sum(n => n.ViewCount)
                    })
                    .ToList(),
                AccountStatistics = news
                    .GroupBy(n => new { n.AccountId, n.Account?.FullName })
                    .Select(g => new AccountStatisticsDTO
                    {
                        AccountId = g.Key.AccountId,
                        FullName = g.Key.FullName ?? "Unknown",
                        NewsCount = g.Count(),
                        ViewCount = g.Sum(n => n.ViewCount)
                    })
                    .ToList()
            };

            return statistics;
        }
    }
} 