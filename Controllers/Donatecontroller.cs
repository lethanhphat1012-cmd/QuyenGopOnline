using OfficeOpenXml; // Thêm 2 dòng này ở đầu file
using OfficeOpenXml.Style;
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