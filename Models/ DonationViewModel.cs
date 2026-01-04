using System.ComponentModel.DataAnnotations;

namespace QuyenGopOnline.Models
{
    public class DonationViewModel
    {
        public int PostId { get; set; } // ID của bài viết đang xem
       public string? PostTitle { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        [Range(1000, 100000000, ErrorMessage = "Số tiền tối thiểu là 1,000đ")]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string Note { get; set; }
    }
}