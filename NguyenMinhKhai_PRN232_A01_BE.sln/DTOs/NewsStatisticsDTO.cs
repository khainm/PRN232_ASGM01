using System.ComponentModel.DataAnnotations;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.DTOs
{
    /// <summary>
    /// Data transfer object for news statistics
    /// </summary>
    public class NewsStatisticsDTO
    {
        /// <summary>
        /// Start date of the statistics period
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the statistics period
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Total number of news articles
        /// </summary>
        public int TotalNews { get; set; }

        /// <summary>
        /// Number of active news articles
        /// </summary>
        public int ActiveNews { get; set; }

        /// <summary>
        /// Number of inactive news articles
        /// </summary>
        public int InactiveNews { get; set; }

        /// <summary>
        /// Number of featured news articles
        /// </summary>
        public int FeaturedNews { get; set; }

        /// <summary>
        /// Total number of views across all news articles
        /// </summary>
        public int TotalViews { get; set; }

        /// <summary>
        /// Statistics by category
        /// </summary>
        public List<CategoryStatisticsDTO> CategoryStatistics { get; set; } = new();

        /// <summary>
        /// Statistics by account
        /// </summary>
        public List<AccountStatisticsDTO> AccountStatistics { get; set; } = new();
    }

    /// <summary>
    /// Data transfer object for category statistics
    /// </summary>
    public class CategoryStatisticsDTO
    {
        /// <summary>
        /// Category ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Category name
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Number of news articles in this category
        /// </summary>
        public int NewsCount { get; set; }

        /// <summary>
        /// Total number of views for news articles in this category
        /// </summary>
        public int ViewCount { get; set; }
    }

    /// <summary>
    /// Data transfer object for account statistics
    /// </summary>
    public class AccountStatisticsDTO
    {
        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Account full name
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Number of news articles created by this account
        /// </summary>
        public int NewsCount { get; set; }

        /// <summary>
        /// Total number of views for news articles created by this account
        /// </summary>
        public int ViewCount { get; set; }
    }
} 