using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Models
{
    public class News
    {
        public int NewsId { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Tiêu đề phải từ 10 đến 200 ký tự")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [MinLength(50, ErrorMessage = "Nội dung phải có ít nhất 50 ký tự")]
        public required string Content { get; set; }

        [Required(ErrorMessage = "Ngày tạo không được để trống")]
        [DataType(DataType.DateTime, ErrorMessage = "Định dạng ngày không hợp lệ")]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Ngày cập nhật không được để trống")]
        [DataType(DataType.DateTime, ErrorMessage = "Định dạng ngày không hợp lệ")]
        public DateTime UpdatedDate { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        [Range(0, 1, ErrorMessage = "Trạng thái không hợp lệ (0: Inactive, 1: Active)")]
        public int Status { get; set; } // 1 = active, 0 = inactive

        [Required(ErrorMessage = "Danh mục không được để trống")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required(ErrorMessage = "Tài khoản không được để trống")]
        public int AccountId { get; set; }
        public Account? Account { get; set; }

        public int ViewCount { get; set; }

        public bool IsFeatured { get; set; }

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}