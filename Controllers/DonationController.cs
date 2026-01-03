using Microsoft.AspNetCore.Mvc;
using QuyenGopOnline.Data;
using QuyenGopOnline.Models;
using System.Security.Claims; // Để lấy ID người dùng đã đăng nhập

namespace QuyenGopOnline.Controllers
{
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: Xử lý khi nhấn nút Quyên góp
        [HttpPost]
        public IActionResult SubmitDonation(DonationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Lấy thông tin người dùng đang đăng nhập (Giả định Phát đã lưu UserId vào Session/Cookie)
                // Ở mức cơ bản, Huy có thể lấy tạm UserId từ Database hoặc gán cứng để test
                var userId = 1; // Ví dụ là Admin hoặc User đầu tiên

                // 2. Tạo bản ghi giao dịch mới
                var transaction = new Transaction
                {
                    UserId = userId,
                    PostId = model.PostId,
                    Amount = model.Amount,
                    Note = model.Note,
                    DonationDate = DateTime.Now
                };

                // 3. Lưu vào bảng Transactions
                _context.Transactions.Add(transaction);
                // 4. (Nâng cao) Cập nhật tổng tiền vào bảng Posts (Nếu Hào đã tạo bảng Posts)
                var post = _context.Posts?.Find(model.PostId);
                if (post != null)
                {
                    post.CurrentAmount += model.Amount;
                }

                _context.SaveChanges();

                // 5. Chuyển hướng về trang lịch sử hoặc thông báo thành công
                TempData["Message"] = "Quyên góp thành công! Cảm ơn bạn.";
                return RedirectToAction("Index", "Dashboard");
            }

            return View(model);
        }
    }
}