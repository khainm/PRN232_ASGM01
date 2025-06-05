using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
            ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ hoa, 1 chữ thường và 1 số")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống")]
        [Range(0, 2, ErrorMessage = "Vai trò không hợp lệ (0: Admin, 1: Staff, 2: Lecturer)")]
        public int Role { get; set; } // 0 = Admin, 1 = Staff, 2 = Lecturer

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        [Required]
        [Range(0, 1, ErrorMessage = "Trạng thái không hợp lệ (0: Inactive, 1: Active)")]
        public int Status { get; set; } // 0 = Inactive, 1 = Active

        public ICollection<News> NewsArticles { get; set; } = new List<News>();
    }
}