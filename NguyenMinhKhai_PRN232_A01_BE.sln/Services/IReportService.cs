using System;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Services
{
    public interface IReportService
    {
        Task<object> GetStatisticsAsync(DateTime? startDate, DateTime? endDate);
        Task<object> GetNewsByCategoryAsync(DateTime? startDate, DateTime? endDate);
        Task<object> GetNewsByStaffAsync(DateTime? startDate, DateTime? endDate);
        Task<object> GetNewsTrendsAsync(DateTime? startDate, DateTime? endDate);
    }
} 