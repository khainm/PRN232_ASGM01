using System.ComponentModel.DataAnnotations;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.DTOs
{
    /// <summary>
    /// Data Transfer Object for Admin to update an existing account
    /// </summary>
    public class AdminUpdateAccountDTO
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống")]
        [Range(0, 2, ErrorMessage = "Vai trò không hợp lệ (0: Admin, 1: Staff, 2: Lecturer)")]
        public int Role { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        [Range(0, 1, ErrorMessage = "Trạng thái không hợp lệ (0: Inactive, 1: Active)")]
        public int Status { get; set; }
    }
} 