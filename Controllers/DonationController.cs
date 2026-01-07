using Microsoft.AspNetCore.Mvc;
using QuyenGopOnline.Data;
using QuyenGopOnline.Models;
using System.Security.Claims; // Để lấy ID người dùng đã đăng nhập
using OfficeOpenXml.Style;
using OfficeOpenXml;



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
    // Kiểm tra tính hợp lệ của dữ liệu
    if (ModelState.IsValid)
    {
        try 
        {
            // 1. Giả lập ID người dùng (Đảm bảo trong bảng Users đã có người có Id = 1)
            var userId = 1; 

            // 2. Tạo đối tượng Transaction
            var transaction = new Transaction
            {
                UserId = userId,
                PostId = model.PostId,
                Amount = model.Amount,
                Note = model.Note,
                DonationDate = DateTime.Now
            };

            // 3. Thêm vào DbContext
            _context.Transactions.Add(transaction);

            // 4. Cập nhật tiền cho bài viết (Nếu đã tạo bảng Posts theo hướng dẫn trước)
            var post = _context.Posts.FirstOrDefault(p => p.Id == model.PostId);
            if (post != null)
            {
                post.CurrentAmount += model.Amount;
                _context.Posts.Update(post);
            }

            // --- QUAN TRỌNG: LỆNH LƯU XUỐNG SQL SERVER ---
            _context.SaveChanges(); 

            // 5. Thành công -> Về Dashboard
            TempData["Message"] = "Quyên góp thành công!";
            // Trong hàm SubmitDonation
            return RedirectToAction("Dashboard", "Post");
        }
        catch (Exception ex)
        {
            // Nếu lỗi Database (ví dụ sai khóa ngoại), in ra để sửa
            ModelState.AddModelError("", "Lỗi lưu Database: " + ex.Message);
        }
    }
    else 
    {
        // Debug: In lỗi ra cửa sổ Output của Visual Studio
        var errors = ModelState.Values.SelectMany(v => v.Errors);
        foreach(var error in errors) 
        {
            System.Diagnostics.Debug.WriteLine("Lỗi Validation: " + error.ErrorMessage);
        }
    }

    // Nếu lỗi, quay lại trang TestDonate để nhập lại
    return View("TestDonate", model);
}
        // GET: /Donation/History
        public IActionResult History()
        {
            // Lấy danh sách giao dịch, bao gồm cả thông tin User và Bài viết (nếu đã tạo bảng Posts)
            // Sắp xếp theo ngày mới nhất lên đầu
            var transactions = _context.Transactions
            .OrderByDescending(t => t.DonationDate)
            .ToList();

            return View(transactions);
        }

        [HttpGet]
public IActionResult TestDonate(int postId)
{
    // Tạo một model trống và gán PostId vào để Form biết đang quyên góp cho ai
    var model = new DonationViewModel
    {
        PostId = postId
    };
    
    // Kiểm tra xem bài viết có tồn tại không (để tránh lỗi)
    var post = _context.Posts.Find(postId);
    if (post == null) return NotFound("Bài viết không tồn tại");

    ViewBag.PostTitle = post.Title;
    return View(model);
}

public IActionResult ExportToExcel()
{
    // 1. Lấy dữ liệu sao kê (kèm thông tin bài viết)
    var data = _context.Transactions
                       .OrderByDescending(t => t.DonationDate)
                       .ToList();

    // 2. Thiết lập cấu hình EPPlus (Bắt buộc cho bản miễn phí)
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    using (var package = new ExcelPackage())
    {
        // 3. Tạo một Sheet mới
        var worksheet = package.Workbook.Worksheets.Add("SaoKeGiaoDich");

        // 4. Tạo Header cho bảng
        worksheet.Cells[1, 1].Value = "Mã GD";
        worksheet.Cells[1, 2].Value = "Ngày Quyên Góp";
        worksheet.Cells[1, 3].Value = "Số Tiền (VNĐ)";
        worksheet.Cells[1, 4].Value = "Nội Dung/Lời Nhắn";

        // Định dạng Header cho đẹp
        using (var range = worksheet.Cells[1, 1, 1, 4])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
        }

        // 5. Đổ dữ liệu từ Database vào các dòng
        int row = 2;
        foreach (var item in data)
        {
            worksheet.Cells[row, 1].Value = item.Id;
            worksheet.Cells[row, 2].Value = item.DonationDate.ToString("dd/MM/yyyy HH:mm");
            worksheet.Cells[row, 3].Value = item.Amount;
            worksheet.Cells[row, 4].Value = item.Note;
            row++;
        }

        // Tự động căn chỉnh độ rộng cột
        worksheet.Cells.AutoFitColumns();

        // 6. Xuất file về trình duyệt
        var stream = new MemoryStream();
        package.SaveAs(stream);
        var content = stream.ToArray();

        return File(content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "SaoKeGiaoDich_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
    }
}

        
    }
}