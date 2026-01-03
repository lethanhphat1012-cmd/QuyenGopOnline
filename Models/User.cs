public class User {
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } // Lưu ý: Dự án thật nên mã hóa Hash
    public string FullName { get; set; }
    public string Role { get; set; } // "Admin" hoặc "User"
}