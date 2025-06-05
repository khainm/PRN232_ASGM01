using System.ComponentModel.DataAnnotations;
using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs.Validation;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.DTOs
{
    /// <summary>
    /// Data Transfer Object for updating an existing news article
    /// </summary>
    public class UpdateNewsDTO
    {
        /// <summary>
        /// Title of the news article
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Content of the news article
        /// </summary>
        [Required(ErrorMessage = "Content is required")]
        [MinLength(10, ErrorMessage = "Content must be at least 10 characters")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// ID of the category this news belongs to
        /// </summary>
        [Required(ErrorMessage = "Category is required")]
        [CategoryExists(ErrorMessage = "Category does not exist")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Status of the news article (0: Inactive, 1: Active)
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [Range(0, 1, ErrorMessage = "Status must be either 0 or 1")]
        public int Status { get; set; }

        /// <summary>
        /// List of tag IDs to associate with this news
        /// </summary>
        [MinLength(1, ErrorMessage = "At least one tag is required")]
        public List<int> TagIds { get; set; } = new();
    }
} 