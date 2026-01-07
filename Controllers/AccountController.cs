using Microsoft.AspNetCore.Mvc;
using QuyenGopOnline.Data;
using QuyenGopOnline.Models;
using System.Linq;

namespace QuyenGopOnline.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Tiêm (Inject) DbContext vào Controller qua Constructor
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    // === QUAN TRỌNG: LƯU THÔNG TIN VÀO SESSION TẠI ĐÂY ===
                    HttpContext.Session.SetString("Role", user.Role ?? "");
                    HttpContext.Session.SetString("UserEmail", user.Email ?? "");
                    HttpContext.Session.SetString("FullName", user.FullName ?? "Người dùng");
                    // ==================================================

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Post");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
                }
            }
            return View(model);
        }
        // GET: Hiển thị trang đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Xử lý lưu người dùng mới
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email đã tồn tại chưa
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                // Tạo đối tượng User mới từ dữ liệu form
                var newUser = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Password = model.Password, // Lưu ý: Dự án lớn nên băm mật khẩu
                    Role = "User" // Mặc định đăng ký mới là User thường
                };

                _context.Users.Add(newUser);
                _context.SaveChanges(); // Lưu vào SQL Server

                // Sau khi đăng ký xong, chuyển sang trang Login
                return RedirectToAction("Login");
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa sạch Session
            return RedirectToAction("Login");
        }

    }
}