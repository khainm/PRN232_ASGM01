using System;
using System.ComponentModel.DataAnnotations;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Models
{
    public class Tag
    {
        public int TagId { get; set; }

        [Required(ErrorMessage = "Tên thẻ không được để trống")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên thẻ phải từ 2 đến 50 ký tự")]
        public required string Name { get; set; }

        [StringLength(200, ErrorMessage = "Mô tả không được vượt quá 200 ký tự")]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public int? NewsId { get; set; }
        public News? News { get; set; }
    }
}