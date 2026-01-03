namespace QuyenGopOnline.Models
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalPosts { get; set; }
        public decimal TotalDonations { get; set; }
        public int TotalTransactions { get; set; }
        
        // Danh sách các giao dịch mới nhất để hiển thị bảng
        public List<User> RecentUsers { get; set; } 
    }
}