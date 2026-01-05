using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "Link ảnh minh họa")]
        public string ImageUrl { get; set; } = "https://via.placeholder.com/300x200";

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