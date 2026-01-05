using Microsoft.AspNetCore.Mvc;
using QuyenGopOnline.Data;
using QuyenGopOnline.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting; // Thêm để lấy đường dẫn thư mục web
namespace QuyenGopOnline.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        // Trong class PostController
        private readonly IWebHostEnvironment _hostEnvironment;

        public PostController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Xem danh sách bài viết dành cho Admin
        public IActionResult AdminIndex()
        {
            var posts = _context.Posts.OrderByDescending(p => p.CreatedDate).ToList();
            return View(posts);
        }

        // 2. Trang tạo bài viết mới (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. Xử lý lưu bài viết mới (POST)
        [HttpPost]
        public IActionResult Create(Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Posts.Add(post);
                _context.SaveChanges();
                return RedirectToAction("AdminIndex");
            }
            return View(post);
        }
    }
}
_context = context;
_hostEnvironment = hostEnvironment;
}

[HttpPost]
public async Task<IActionResult> Create(Post post)
{
    if (ModelState.IsValid)
    {
        // Xử lý Upload ảnh
        if (post.ImageFile != null)
        {
            // 1. Tạo đường dẫn thư mục lưu ảnh: wwwroot/images
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.ImageFile.FileName);
            string path = Path.Combine(wwwRootPath + "/images/", fileName);

            // 2. Lưu file vào thư mục
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await post.ImageFile.CopyToAsync(fileStream);
            }

            // 3. Lưu đường dẫn vào database
            post.ImageUrl = "/images/" + fileName;
        }

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return RedirectToAction("AdminIndex");
    }
    return View(post);
}
// 1. Trang Chỉnh sửa (GET)

public IActionResult Edit(int id)

{

    var post = _context.Posts.Find(id);

    if (post == null) return NotFound();

    return View(post);

}



// 2. Xử lý Chỉnh sửa (POST)

[HttpPost]

public async Task<IActionResult> Edit(Post post)

{

    if (ModelState.IsValid)

    {

        // Nếu có upload ảnh mới thì xử lý giống hàm Create, 

        // nếu không thì giữ nguyên ImageUrl cũ.

        _context.Update(post);

        await _context.SaveChangesAsync();

        return RedirectToAction("AdminIndex");

    }

    return View(post);

}
// 3. Xóa bài viết

public IActionResult Delete(int id)

{

    var post = _context.Posts.Find(id);

    if (post != null)

    {

        _context.Posts.Remove(post);

        _context.SaveChanges();

    }

    return RedirectToAction("AdminIndex");

}