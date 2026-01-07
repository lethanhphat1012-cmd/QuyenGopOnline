using Microsoft.EntityFrameworkCore;
using QuyenGopOnline.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Đăng ký các dịch vụ hệ thống
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// 3. Cấu hình Session (QUAN TRỌNG: Phải có Cache)
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// 4. Cấu hình HTTP request pipeline (Middleware)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Đảm bảo dùng cái này để load CSS/JS

app.UseRouting();

// 5. BẬT SESSION TẠI ĐÂY (Phải nằm sau UseRouting và trước UseAuthorization)
app.UseSession(); 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
