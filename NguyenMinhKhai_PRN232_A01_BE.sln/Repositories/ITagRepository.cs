using NguyenMinhKhai_PRN232_A01_BE.sln.Models;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Repositories
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(int id);
        Task<IEnumerable<Tag>> SearchAsync(string term);
        Task<Tag> CreateAsync(Tag tag);
        Task<Tag?> UpdateAsync(int id, string name);
        Task<bool> DeleteAsync(int id);
    }
} 