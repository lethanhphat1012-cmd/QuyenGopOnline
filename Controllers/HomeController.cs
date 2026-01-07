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

    public async Task<IActionResult> Index()
    {
        var posts = await _context.Posts.ToListAsync();
        
        foreach (var item in posts)
        {
            // Tính toán lại số tiền hiện có dựa trên bảng Transactions
            item.CurrentAmount = await _context.Transactions
                                        .Where(t => t.PostId == item.Id)
                                        .SumAsync(t => t.Amount);
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
