using Microsoft.AspNetCore.Mvc;
using QuyenGopOnline.Data;
using QuyenGopOnline.Models;
using System.Linq;

namespace QuyenGopOnline.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy dữ liệu thực tế từ DB
            var model = new DashboardViewModel
            {
                TotalUsers = _context.Users.Count(),
                // Giả lập các số liệu chưa có bảng (Huy và Hào sẽ cập nhật sau)
                TotalPosts = 0, 
                TotalDonations = 0,
                TotalTransactions = 0,
                RecentUsers = _context.Users.OrderByDescending(u => u.Id).Take(5).ToList()
            };

            return View(model);
        }
    }
}