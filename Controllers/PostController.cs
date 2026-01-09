using Microsoft.AspNetCore.Mvc;
using QuyenGopOnline.Data;
using QuyenGopOnline.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace QuyenGopOnline.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        // CHỈ CẦN 1 CONSTRUCTOR DUY NHẤT
        public PostController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // 1. Xem danh sách bài viết dành cho Admin
        public IActionResult AdminIndex()
{
    // Lấy danh sách bài viết kèm theo danh sách giao dịch liên quan
    var posts = _context.Posts.ToList();
    
    foreach (var item in posts)
    {
        // Tính tổng tiền từ bảng Transactions dựa trên PostId
        // Thêm .AsEnumerable() để ép nó tính toán chính xác
        var total = _context.Transactions
                            .Where(t => t.PostId == item.Id)
                            .AsEnumerable() 
                            .Sum(t => t.Amount);
                            
        item.CurrentAmount = total;
    }
    
    return View(posts);
}

        // 2. Dashboard tổng quan
        public IActionResult Dashboard()
{
    // 1. Tính toán dữ liệu
    var totalAmount = _context.Transactions.Sum(t => (decimal?)t.Amount) ?? 0;
    var postCount = _context.Posts.Count();
    var userCount = _context.Users.Count();
    var transactionCount = _context.Transactions.Count();
    var recentUsers = _context.Users.OrderByDescending(u => u.Id).Take(5).ToList();

    // 2. Nạp vào ViewModel (Đảm bảo class DashboardViewModel đã có đủ các property này)
    var model = new DashboardViewModel
    {
        TotalDonations = totalAmount, // Kiểm tra xem tên biến trong Model là gì
        TotalPosts = postCount,
        TotalUsers = userCount,
        TotalTransactions = transactionCount,
        RecentUsers = recentUsers
    };

    return View(model); // Truyền model sang View
}


        // 3. Trang tạo bài viết mới (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 4. Xử lý lưu bài viết mới có Upload ảnh (POST)
        [HttpPost]
[ValidateAntiForgeryToken] // Chống tấn công giả mạo yêu cầu
public async Task<IActionResult> Create(Post post)
{
    // 1. Kiểm tra quyền truy cập từ Session
    var userRole = HttpContext.Session.GetString("UserRole");
    var userEmail = HttpContext.Session.GetString("UserEmail");

    if (userRole != "PostCreator" && userRole != "Admin")
    {
        return RedirectToAction("VerifyIdentity", "Account");
    }

    if (ModelState.IsValid)
    {
        // 2. Xử lý lưu File ảnh
        if (post.ImageFile != null)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string folderPath = Path.Combine(wwwRootPath, "images");
            
            // Đảm bảo thư mục /images/ tồn tại
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.ImageFile.FileName);
            string filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await post.ImageFile.CopyToAsync(fileStream);
            }
            post.ImageUrl = "/images/" + fileName;
        }

        // 3. Thiết lập các thông số mặc định cho bài viết
        post.CreatedAt = DateTime.Now; // Ngày tạo bài
        post.IsApproved = false;       // Mặc định chờ Admin duyệt bài (nếu cần)
        
        // 4. Lấy ID của người dùng hiện tại để gán làm chủ bài viết
        var currentUser = _context.Users.FirstOrDefault(u => u.Email == userEmail);
        if (currentUser != null)
        {
            post.UserId = currentUser.Id; // Giả sử model Post có trường UserId
        }

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        // 5. Thông báo thành công và chuyển hướng
        TempData["Success"] = "Bài viết đã được gửi và đang chờ phê duyệt!";
        return RedirectToAction("Index", "Home"); // Trả về trang chủ sau khi đăng
    }

    return View(post);
}

        // 5. Trang Chỉnh sửa (GET)
        public IActionResult Edit(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return NotFound();
            return View(post);
        }

        // 6. Xử lý Chỉnh sửa (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if (id != post.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Lấy dữ liệu gốc từ Database (Tránh mất CurrentAmount và CreatedDate)
                    var existingPost = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingPost == null) return NotFound();

                    // 2. Xử lý ImageFile nếu Admin có chọn ảnh mới
                    if (post.ImageFile != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.ImageFile.FileName);
                        string path = Path.Combine(wwwRootPath, "images", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await post.ImageFile.CopyToAsync(fileStream);
                        }
                        // Gán link ảnh mới
                        post.ImageUrl = "/images/" + fileName;
                    }
                    else
                    {
                        // Nếu không chọn ảnh mới, giữ nguyên ảnh cũ từ database
                        post.ImageUrl = existingPost.ImageUrl;
                    }

                    // 3. Giữ nguyên các giá trị không được phép sửa trong trang Edit (như số tiền hiện có)
                    post.CurrentAmount = existingPost.CurrentAmount;
                    post.CreatedDate = existingPost.CreatedDate;

                    // 4. Cập nhật vào DB
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(AdminIndex));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id)) return NotFound();
                    else throw;
                }
            }
            return View(post);
        }
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        // 7. Xóa bài viết
       // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            
            if (post == null) return NotFound();

            // Nếu CurrentAmount không tự cập nhật, hãy tính trực tiếp từ bảng Transactions
            post.CurrentAmount = await _context.Transactions
                                        .Where(t => t.PostId == id)
                                        .SumAsync(t => t.Amount);

            return View(post);
        }

// POST: Post/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                // Xóa file ảnh trong thư mục wwwroot nếu cần
                if (!string.IsNullOrEmpty(post.ImageUrl))
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, post.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã xóa bài viết thành công!" });
            }
            return Json(new { success = false, message = "Lỗi khi xóa bài viết." });
        }

    }
}