using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Services
{
    public interface ICategoryService
    {
        IQueryable<Category> GetAll();
        IQueryable<Category> SearchCategories(string searchTerm);
        Task<Category> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasNewsArticlesAsync(int id);
    }
} 