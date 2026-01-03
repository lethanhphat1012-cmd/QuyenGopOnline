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
                // TRUY VẤN DB: Tìm user có email và password khớp
                // Lưu ý: Trong dự án thực tế, password nên được băm (Hash), ở đây mình làm so sánh chuỗi để bạn dễ hiểu trước.
                var user = _context.Users
                    .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    // Đăng nhập thành công
                    // Tùy vào Role (Admin/User) để điều hướng
                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Dashboard");
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
    }
}