using Microsoft.AspNetCore.Mvc;
using QuyenGopOnline.Models;

namespace QuyenGopOnline.Controllers
{
    public class AccountController : Controller
    {
        // GET: Hiển thị trang đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Xử lý dữ liệu khi nhấn nút Đăng nhập
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Giả lập kiểm tra đăng nhập (Sau này Phát sẽ check trong SQL Server tại đây)
                if (model.Email == "admin@gmail.com" && model.Password == "123456")
                {
                    // Đăng nhập thành công -> Chuyển về Dashboard
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }
    }
}