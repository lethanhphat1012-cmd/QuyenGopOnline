using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuyenGopOnline.Models;
using QuyenGopOnline.Data;
using Microsoft.EntityFrameworkCore; // Đảm bảo có namespace này để nhận diện ApplicationDbContext

namespace QuyenGopOnline.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    // SỬA TẠI ĐÂY: Thêm ApplicationDbContext context vào tham số
    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context; // Bây giờ _context đã có dữ liệu từ Dependency Injection
    }

    public IActionResult Index()
    {
        var posts = _context.Posts.ToList(); // Hoặc logic lấy danh sách bài viết của bạn
        
        // Kiểm tra trạng thái xác thực của User hiện tại
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (!string.IsNullOrEmpty(userEmail))
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            ViewBag.IsVerified = user?.IsVerified ?? false;
        }

        return View(posts);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
