using Microsoft.AspNetCore.Mvc;
using QuyenGopOnline.Data;
using QuyenGopOnline.Models;
using System.Linq;
using Tesseract; // Thư viện OCR
using System.IO; // Để xử lý file ảnh
using System.Drawing; // (Có thể cần tùy phiên bản, nhưng Tesseract dùng Pix)
using Newtonsoft.Json;
using System.Text;             // Lỗi CS0103 cần cái này
using System.Net.Http;         // Để dùng HttpClient
using Newtonsoft.Json;

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
                var user = _context.Users
                    .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    // === QUAN TRỌNG: LƯU THÔNG TIN VÀO SESSION TẠI ĐÂY ===
                    HttpContext.Session.SetString("UserRole", user.Role ?? ""); 
            HttpContext.Session.SetString("UserEmail", user.Email ?? "");
            HttpContext.Session.SetString("UserName", user.FullName ?? "Người dùng"); // Đổi FullName thành UserName cho giống Layout
                    // ==================================================

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Post");
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
        // GET: Hiển thị trang đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Xử lý lưu người dùng mới
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email đã tồn tại chưa
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                // Tạo đối tượng User mới từ dữ liệu form
                var newUser = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Password = model.Password, // Lưu ý: Dự án lớn nên băm mật khẩu
                    Role = "User" // Mặc định đăng ký mới là User thường
                };

                _context.Users.Add(newUser);
                _context.SaveChanges(); // Lưu vào SQL Server

                // Sau khi đăng ký xong, chuyển sang trang Login
                return RedirectToAction("Login");
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa sạch Session
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult VerifyIdentity()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            
            // Nếu người dùng đã xác thực thành công rồi
            if (user != null && user.IsVerified)
            {
                // Chuyển hướng thẳng sang trang Profile kèm thông báo
                return RedirectToAction("Profile");
            }

            return View();
        }

        [HttpPost]
public async Task<IActionResult> VerifyIdentity(string qrData, string imageFile)
{
    try
    {
        // 1. Kiểm tra đầu vào
        if (string.IsNullOrEmpty(qrData) || string.IsNullOrEmpty(imageFile))
            return Json(new { success = false, message = "Dữ liệu không đầy đủ từ Camera hoặc QR." });

        // 2. Tiền xử lý dữ liệu
        // Tách số ID từ chuỗi QR (CCCD QR có định dạng: SốID|SốIDCũ|HọTên|NgàySinh|...)
        string qrIdNumber = qrData.Split('|')[0].Trim();
        
        // Loại bỏ header của base64 nếu có (data:image/png;base64,...)
        string base64Image = imageFile.Contains(",") ? imageFile.Split(',')[1] : imageFile;

        // 3. Gọi Gemini AI để trích xuất thông tin từ ảnh chụp
        string apiKey = "AIzaSyDIjaJ5jZdSCATySCLvusBNJhppx2BO_9o"; 
        // Lưu ý: Đảm bảo model name chính xác (gemini-1.5-flash hoặc gemini-2.0-flash-exp)
        string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

        using (var client = new HttpClient())
        {
            var requestBody = new
            {
                contents = new[] {
                    new {
                        parts = new object[] {
                            new { text = @"Trích xuất thông tin từ mặt trước CCCD này. Trả về JSON chính xác theo mẫu: 
                                          {
                                            ""id"": ""12 số định danh"",
                                            ""name"": ""Họ tên đầy đủ"",
                                            ""dob"": ""ngày/tháng/năm"",
                                            ""gender"": ""Nam/Nữ"",
                                            ""nationality"": ""Việt Nam"",
                                            ""home"": ""Quê quán/Nơi thường trú""
                                          }" 
                            },
                            new { inline_data = new { mime_type = "image/png", data = base64Image } }
                        }
                    }
                },
                generationConfig = new { 
                    response_mime_type = "application/json",
                    temperature = 0.1 // Đặt thấp để AI trả kết quả chính xác hơn
                }
            };

            var jsonRequest = JsonConvert.SerializeObject(requestBody);
            var response = await client.PostAsync(apiUrl, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            var resultStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) 
                return Json(new { success = false, message = "Không thể kết nối với trí tuệ nhân tạo." });

            // 4. Phân tích kết quả từ AI
            dynamic geminiResponse = JsonConvert.DeserializeObject(resultStr);
            string aiJsonText = geminiResponse.candidates[0].content.parts[0].text;
            var aiData = JsonConvert.DeserializeObject<dynamic>(aiJsonText);

            string aiIdNumber = aiData?.id?.ToString() ?? "";

            // 5. So khớp dữ liệu giữa mã QR và kết quả AI đọc được từ ảnh
            // Chấp nhận so sánh chứa nhau để giảm thiểu sai sót nhỏ của AI
            if (aiIdNumber == qrIdNumber || qrIdNumber.Contains(aiIdNumber) || aiIdNumber.Contains(qrIdNumber))
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                    return Json(new { success = false, message = "Phiên làm việc hết hạn, vui lòng đăng nhập lại." });

                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                
                if (user != null)
                {
                    // Cập nhật thông tin định danh vào Database
                    user.IsVerified = true;
                    user.IdCardNumber = qrIdNumber;
                    user.FullName = aiData?.name?.ToString();
                    user.DateOfBirth = aiData?.dob?.ToString();
                    user.Gender = aiData?.gender?.ToString();
                    user.Nationality = aiData?.nationality?.ToString();
                    user.Address = aiData?.home?.ToString();
                    user.IdCardImage = imageFile; // Lưu để Admin đối soát

                    // LOGIC BẢO VỆ QUYỀN ADMIN:
                    // Chỉ nâng cấp lên PostCreator nếu đang là User thường
                    if (user.Role != "Admin")
                    {
                        user.Role = "PostCreator";
                        HttpContext.Session.SetString("UserRole", "PostCreator");
                    }
                    else 
                    {
                        // Nếu là Admin thì giữ nguyên Role Admin trong Session
                        HttpContext.Session.SetString("UserRole", "Admin");
                    }

                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Xác thực danh tính thành công!" });
                }
            }

            return Json(new { success = false, message = "Số CCCD trên ảnh và mã QR không khớp. Vui lòng chụp rõ nét hơn." });
        }
    }
    catch (Exception ex)
    {
        // Ghi log lỗi nếu cần thiết
        return Json(new { success = false, message = "Đã xảy ra lỗi hệ thống: " + ex.Message });
    }
}


        public IActionResult VerifiedUsers()
        {
            // Sử dụng tên đầy đủ của Model để tránh xung đột
            List<QuyenGopOnline.Models.User> users = _context.Users
                .Where(u => u.IsVerified == true)
                .ToList();
                
            return View(users);
        }

        [HttpGet]
public IActionResult Profile()
{
    // 1. Lấy Email từ Session để biết "ai" đang xem
    var userEmail = HttpContext.Session.GetString("UserEmail");
    
    if (string.IsNullOrEmpty(userEmail))
    {
        // Nếu chưa đăng nhập thì đẩy về trang Login
        return RedirectToAction("Login");
    }

    // 2. Tìm thông tin người dùng trong Database
    var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

    if (user == null)
    {
        return NotFound();
    }

    // 3. Trả về View (Nó sẽ tìm file Views/Account/Profile.cshtml)
    return View(user);
}
    }
}