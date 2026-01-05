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
            var posts = _context.Posts.OrderByDescending(p => p.CreatedDate).ToList();
            return View(posts);
        }

        // 2. Dashboard tổng quan
        public IActionResult Dashboard()
        {
            ViewBag.TotalPosts = _context.Posts.Count();
            ViewBag.TotalAmount = _context.Posts.Sum(p => p.CurrentAmount);
            ViewBag.TotalUsers = _context.Users.Count();
            return View();
        }

        // 3. Trang tạo bài viết mới (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 4. Xử lý lưu bài viết mới có Upload ảnh (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                if (post.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.ImageFile.FileName);
                    string path = Path.Combine(wwwRootPath + "/images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await post.ImageFile.CopyToAsync(fileStream);
                    }
                    post.ImageUrl = "/images/" + fileName;
                }

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminIndex");
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
        public async Task<IActionResult> Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Posts.Any(e => e.Id == post.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("AdminIndex");
            }
            return View(post);
        }

        // 7. Xóa bài viết
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
    }
}