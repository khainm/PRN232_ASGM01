using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NguyenMinhKhai_PRN232_A01_BE.sln.Data;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;

namespace FUNewsManagementSystem.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.NewsArticles.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(n => n.CreatedDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(n => n.CreatedDate <= endDate.Value);

            var totalNews = await query.CountAsync();
            var activeNews = await query.CountAsync(n => n.Status == 1);
            var totalCategories = await _context.Categories.CountAsync();
            var totalStaff = await _context.Accounts.CountAsync(a => a.Role == 1);
            var totalTags = await _context.Tags.CountAsync();

            return new
            {
                TotalNews = totalNews,
                ActiveNews = activeNews,
                InactiveNews = totalNews - activeNews,
                TotalCategories = totalCategories,
                TotalStaff = totalStaff,
                TotalTags = totalTags
            };
        }

        public async Task<object> GetNewsByCategoryAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.NewsArticles
                .Include(n => n.Category)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(n => n.CreatedDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(n => n.CreatedDate <= endDate.Value);

            var newsByCategory = await query
                .Where(n => n.Category != null)
                .GroupBy(n => n.Category!.Name)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    Count = g.Count(),
                    ActiveCount = g.Count(n => n.Status == 1),
                    InactiveCount = g.Count(n => n.Status == 0)
                })
                .ToListAsync();

            return newsByCategory;
        }

        public async Task<object> GetNewsByStaffAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.NewsArticles
                .Include(n => n.Account)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(n => n.CreatedDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(n => n.CreatedDate <= endDate.Value);

            var newsByStaff = await query
                .Where(n => n.Account != null)
                .GroupBy(n => new { n.Account!.AccountId, n.Account.FullName })
                .Select(g => new
                {
                    StaffId = g.Key.AccountId,
                    StaffName = g.Key.FullName,
                    TotalNews = g.Count(),
                    ActiveNews = g.Count(n => n.Status == 1),
                    InactiveNews = g.Count(n => n.Status == 0)
                })
                .ToListAsync();

            return newsByStaff;
        }

        public async Task<object> GetNewsTrendsAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.NewsArticles.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(n => n.CreatedDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(n => n.CreatedDate <= endDate.Value);

            var newsTrends = await query
                .GroupBy(n => new { n.CreatedDate.Year, n.CreatedDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count(),
                    ActiveCount = g.Count(n => n.Status == 1),
                    InactiveCount = g.Count(n => n.Status == 0)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            return newsTrends;
        }
    }
} 