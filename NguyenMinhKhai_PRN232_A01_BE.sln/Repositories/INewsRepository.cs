using Microsoft.EntityFrameworkCore;
using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Repositories
{
    public interface INewsRepository
    {
        IQueryable<News> GetAll();
        IQueryable<News> GetActiveNews();
        IQueryable<News> GetNewsByAccountId(int accountId);
        IQueryable<News> SearchNews(string searchTerm);
        IQueryable<News> GetNewsByDateRange(DateTime startDate, DateTime endDate);
        IQueryable<News> GetNewsStatistics(DateTime startDate, DateTime endDate);
        Task<News?> GetByIdAsync(int id);
        Task<News?> GetActiveByIdAsync(int id);
        Task<News> CreateAsync(CreateNewsDTO newsDto, int accountId);
        Task<News?> UpdateAsync(int id, UpdateNewsDTO newsDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> IncrementViewCountAsync(int id);
        Task<NewsStatisticsDTO> GetNewsStatisticsAsync(DateTime startDate, DateTime endDate);
    }
} 