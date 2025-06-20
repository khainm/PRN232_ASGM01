using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Authorization;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;
using NguyenMinhKhai_PRN232_A01_BE.sln.Repositories;
using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ODataController
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        // OData Endpoints
        [EnableQuery(PageSize = 10, MaxTop = 100)]
        [AllowAnonymous]
        [HttpGet]
        public IQueryable<CategoryDTO> Get()
        {
            var categories = _categoryRepository.GetAll()
                .Select(c => new CategoryDTO
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Status = c.Status,
                    NewsCount = c.NewsArticles.Count
                });
            return categories;
        }

        [EnableQuery(PageSize = 10, MaxTop = 100)]
        [AllowAnonymous]
        [HttpGet("search")]
        public IQueryable<CategoryDTO> Search([FromQuery] string term)
        {
            var categories = _categoryRepository.SearchCategories(term)
                .Select(c => new CategoryDTO
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Status = c.Status,
                    NewsCount = c.NewsArticles.Count
                });
            return categories;
        }

        // RESTful Endpoints
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDTO>> GetById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            var categoryDto = new CategoryDTO
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Status = category.Status,
                NewsCount = category.NewsArticles.Count
            };

            return Ok(categoryDto);
        }

        [HttpPost]
        [Authorize(Policy = "RequireStaffRole")]
        public async Task<ActionResult<CategoryDTO>> Create([FromBody] CreateCategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = _mapper.Map<Category>(categoryDto);
            category.Status = 1;
            var createdCategory = await _categoryRepository.CreateAsync(category);
            
            var resultDto = new CategoryDTO
            {
                CategoryId = createdCategory.CategoryId,
                Name = createdCategory.Name,
                Status = createdCategory.Status,
                NewsCount = 0 // New category has no news
            };

            return CreatedAtAction(nameof(GetById), new { id = createdCategory.CategoryId }, resultDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireStaffRole")]
        public async Task<ActionResult<CategoryDTO>> Update(int id, [FromBody] UpdateCategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                return NotFound();

            _mapper.Map(categoryDto, existingCategory);
            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);

            var resultDto = new CategoryDTO
            {
                CategoryId = updatedCategory.CategoryId,
                Name = updatedCategory.Name,
                Status = updatedCategory.Status,
                NewsCount = updatedCategory.NewsArticles.Count
            };

            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireStaffRole")]
        public async Task<ActionResult> Delete(int id)
        {
            var hasNews = await _categoryRepository.HasNewsArticlesAsync(id);
            if (hasNews)
                return BadRequest("Cannot delete category that has associated news articles.");

            var result = await _categoryRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("simple")]
        public async Task<ActionResult<List<CategoryDTO>>> GetSimple()
        {
            var categories = await _categoryRepository.GetAll()
                .Select(c => new CategoryDTO
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Status = c.Status,
                    NewsCount = c.NewsArticles.Count
                }).ToListAsync();
            return Ok(categories);
        }
    }
}