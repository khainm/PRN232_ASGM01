using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;
using NguyenMinhKhai_PRN232_A01_BE.sln.Services;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagsController> _logger;

        public TagsController(ITagService tagService, ILogger<TagsController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        // GET: api/Tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetTags()
        {
            try
            {
                var tags = await _tagService.GetAllAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tags");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Tags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TagDTO>> GetTag(int id)
        {
            try
            {
                var tag = await _tagService.GetByIdAsync(id);

                if (tag == null)
                {
                    return NotFound();
                }

                return Ok(tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tag {TagId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Tags/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TagDTO>>> SearchTags([FromQuery] string term)
        {
            try
            {
                var tags = await _tagService.SearchAsync(term);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tags with term {SearchTerm}", term);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Tags
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TagDTO>> CreateTag(CreateTagDTO createTagDto)
        {
            try
            {
                var tag = await _tagService.CreateAsync(createTagDto);
                return CreatedAtAction(nameof(GetTag), new { id = tag.TagId }, tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tag");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Tags/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTag(int id, UpdateTagDTO updateTagDto)
        {
            try
            {
                var tag = await _tagService.UpdateAsync(id, updateTagDto);
                if (tag == null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tag {TagId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Tags/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                var result = await _tagService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tag {TagId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 