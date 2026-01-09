using System.ComponentModel.DataAnnotations;

namespace QuyenGopOnline.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public bool IsVerified { get; set; } = false;

        public string? IdCardNumber { get; set; }

        // CÁC TRƯỜNG CÒN THIẾU PHÁT CẦN THÊM VÀO ĐÂY:
        public string? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? Nationality { get; set; }

        public string? Address { get; set; }

        public string? IdCardImage { get; set; } // Lưu chuỗi Base64
    }
}