using Microsoft.AspNetCore.Mvc;
using QuyenGopOnline.Models;
using Microsoft.EntityFrameworkCore;
using QuyenGopOnline.Data;

namespace QuyenGopOnline.Controllers
{
    // Bạn có thể thêm phân quyền ở đây nếu đã cấu hình Role
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Trang danh sách người dùng
        public async Task<IActionResult> Users()
        {
            // Kiểm tra xem có phải Admin không (dựa trên Session)
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Index", "Home");

            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // 2. Action Hủy xác thực
        [HttpPost]
        public async Task<IActionResult> RevokeVerification(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsVerified = false;
                user.Role = "User"; // Hạ quyền về người dùng thường
                // Có thể xóa luôn ảnh CCCD cũ nếu muốn bảo mật
                // user.IdCardImage = null; 

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Users));
        }
    }
}