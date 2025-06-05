using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên danh mục phải từ 2 đến 100 ký tự")]
        public required string Name { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        [Range(0, 1, ErrorMessage = "Trạng thái không hợp lệ (0: Inactive, 1: Active)")]
        public int Status { get; set; } // 1 = active, 0 = inactive

        [Required]
        public DateTime CreatedDate { get; set; }

        public int Order { get; set; }

        public ICollection<News> NewsArticles { get; set; } = new List<News>();
    }
}