using AutoMapper;
using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;
using NguyenMinhKhai_PRN232_A01_BE.sln.Repositories;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TagService> _logger;

        public TagService(ITagRepository tagRepository, IMapper mapper, ILogger<TagService> logger)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TagDTO>> GetAllAsync()
        {
            try
            {
                var tags = await _tagRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<TagDTO>>(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tags");
                throw;
            }
        }

        public async Task<TagDTO?> GetByIdAsync(int id)
        {
            try
            {
                var tag = await _tagRepository.GetByIdAsync(id);
                return tag != null ? _mapper.Map<TagDTO>(tag) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tag {TagId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<TagDTO>> SearchAsync(string term)
        {
            try
            {
                var tags = await _tagRepository.SearchAsync(term);
                return _mapper.Map<IEnumerable<TagDTO>>(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tags with term {SearchTerm}", term);
                throw;
            }
        }

        public async Task<TagDTO> CreateAsync(CreateTagDTO createTagDto)
        {
            try
            {
                var tag = new Tag
                {
                    Name = createTagDto.Name,
                    CreatedDate = DateTime.UtcNow
                };

                var createdTag = await _tagRepository.CreateAsync(tag);
                return _mapper.Map<TagDTO>(createdTag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tag");
                throw;
            }
        }

        public async Task<TagDTO?> UpdateAsync(int id, UpdateTagDTO updateTagDto)
        {
            try
            {
                var updatedTag = await _tagRepository.UpdateAsync(id, updateTagDto.Name);
                return updatedTag != null ? _mapper.Map<TagDTO>(updatedTag) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tag {TagId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                return await _tagRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tag {TagId}", id);
                throw;
            }
        }
    }
} 