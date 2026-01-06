using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Cần thiết cho [NotMapped]
using Microsoft.AspNetCore.Http; // Cần thiết cho IFormFile

namespace QuyenGopOnline.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [Display(Name = "Tiêu đề bài viết")]
        public string Title { get; set; } = "";

        [Display(Name = "Mô tả hoàn cảnh")]
        public string Description { get; set; } = "";

        [Display(Name = "Đường dẫn ảnh")]
        public string ImageUrl { get; set; } = "/images/default-post.jpg";

        [NotMapped] // Field này chỉ dùng để nhận file, không tạo cột trong Database
        [Display(Name = "Chọn ảnh từ máy tính")]
        public IFormFile? ImageFile { get; set; } 

        [Required(ErrorMessage = "Vui lòng nhập số tiền cần kêu gọi")]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền mục tiêu phải lớn hơn 1,000đ")]
        [Display(Name = "Số tiền mục tiêu (VNĐ)")]
        public decimal TargetAmount { get; set; }

        [Display(Name = "Số tiền hiện có")]
        public decimal CurrentAmount { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}