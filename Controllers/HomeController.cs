using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuyenGopOnline.Models;
using QuyenGopOnline.Data; // Đảm bảo có namespace này để nhận diện ApplicationDbContext

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
        // Lấy danh sách bài viết từ DB. 
        // Dùng .ToList() để đảm bảo dữ liệu được tải lên trước khi ra View
        var posts = _context.Posts.ToList() ?? new List<Post>(); 
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
