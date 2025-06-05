using System.ComponentModel.DataAnnotations;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.DTOs
{
    /// <summary>
    /// Data Transfer Object for News entity
    /// </summary>
    public class NewsDTO
    {
        /// <summary>
        /// Unique identifier for the news article
        /// </summary>
        public int NewsId { get; set; }

        /// <summary>
        /// Title of the news article
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Content of the news article
        /// </summary>
        public required string Content { get; set; }

        /// <summary>
        /// Date when the news article was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Status of the news article (0: Inactive, 1: Active)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// ID of the category this news belongs to
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Name of the category this news belongs to
        /// </summary>
        public required string CategoryName { get; set; }

        /// <summary>
        /// ID of the account that created this news
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Name of the account that created this news
        /// </summary>
        public required string AccountName { get; set; }

        /// <summary>
        /// List of tags associated with this news
        /// </summary>
        public List<string> Tags { get; set; } = new();
    }
} 