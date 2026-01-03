using Microsoft.EntityFrameworkCore;
using QuyenGopOnline.Models;

namespace QuyenGopOnline.Data 
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor này cực kỳ quan trọng để EF Core nạp ConnectionString
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        // Sau này Huy và Hào sẽ thêm DbSet cho Bài viết và Giao dịch ở đây
        public DbSet<Transaction> Transactions { get; set; }
    }
}