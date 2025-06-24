using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Services
{
    public interface ITagService
    {
        Task<IEnumerable<TagDTO>> GetAllAsync();
        Task<TagDTO?> GetByIdAsync(int id);
        Task<IEnumerable<TagDTO>> SearchAsync(string term);
        Task<TagDTO> CreateAsync(CreateTagDTO createTagDto);
        Task<TagDTO?> UpdateAsync(int id, UpdateTagDTO updateTagDto);
        Task<bool> DeleteAsync(int id);
    }
} 