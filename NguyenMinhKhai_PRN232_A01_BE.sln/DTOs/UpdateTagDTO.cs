using System.ComponentModel.DataAnnotations;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.DTOs
{
    /// <summary>
    /// Data Transfer Object for updating an existing tag
    /// </summary>
    public class UpdateTagDTO
    {
        /// <summary>
        /// The name of the tag
        /// </summary>
        [Required(ErrorMessage = "Tag name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tag name must be between 2 and 50 characters")]
        public string Name { get; set; } = string.Empty;
    }
} 