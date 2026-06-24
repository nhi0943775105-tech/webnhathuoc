using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebGoiYThuoc.Models;

namespace WebGoiYThuoc.Data
{
    // Kế thừa chuẩn từ IdentityDbContext để quản lý bảng AspNetUsers thay vì TaiKhoan thủ công
    public class ApplicationDbContext : IdentityDbContext<TaiKhoan>
    {
        // 1. Hàm khởi tạo chính dùng cho runtime hệ thống lấy cấu hình từ Program.cs
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 2. BỔ SUNG QUAN TRỌNG: Hàm khởi tạo trống cứu cánh khi .NET Identity bị lạc đường tìm Service
        public ApplicationDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Điền thẳng chuỗi kết nối SQL Server của bạn vào đây để đảm bảo an toàn tuyệt đối
                optionsBuilder.UseSqlServer("Server=.;Database=WebGoiYThuocDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        public DbSet<Benh> Benhs { get; set; }
        public DbSet<TrieuChung> TrieuChungs { get; set; }
        public DbSet<Thuoc> Thuocs { get; set; }
    }
}