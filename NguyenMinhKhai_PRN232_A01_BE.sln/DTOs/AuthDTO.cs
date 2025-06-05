using System.ComponentModel.DataAnnotations;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public required string Password { get; set; }
    }

    public class LoginResponseDTO
    {
        public required string Token { get; set; }
        public required AccountDTO Account { get; set; }
    }
} 