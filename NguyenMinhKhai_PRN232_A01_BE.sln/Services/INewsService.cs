using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Services
{
    public interface INewsService
    {
        IQueryable<NewsDTO> GetAll();
        IQueryable<NewsDTO> GetActiveNews();
        IQueryable<NewsDTO> GetNewsByAccountId(int accountId);
        IQueryable<NewsDTO> SearchNews(string searchTerm);
        IQueryable<NewsDTO> GetNewsByDateRange(DateTime startDate, DateTime endDate);
        Task<NewsDTO?> GetByIdAsync(int id);
        Task<NewsDTO> CreateAsync(CreateNewsDTO newsDto, int accountId);
        Task<NewsDTO?> UpdateAsync(int id, UpdateNewsDTO newsDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> IncrementViewCountAsync(int id);
        Task<NewsStatisticsDTO> GetNewsStatisticsAsync(DateTime startDate, DateTime endDate);
    }
} 