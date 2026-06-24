using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebGoiYThuoc.Data;
using WebGoiYThuoc.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký DbContext với connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<TaiKhoan, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Thiết lập đường dẫn trang đăng nhập mặc định
// Thiết lập phân quyền đường dẫn và Cookie của Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied"; // Tuyến đường khi tài khoản không đủ quyền (User thường cố vào Admin)
    options.ExpireTimeSpan = System.TimeSpan.FromMinutes(60);
});

// BỔ SUNG QUAN TRỌNG: Khai báo chính sách kiểm tra trường VaiTro cho Admin Panel
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(System.Security.Claims.ClaimTypes.Role, "Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// SeedDatabase.Initialize(app); // Khởi tạo dữ liệu ban đầu
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<TaiKhoan>>();

        // Đặt email admin mặc định theo đúng format Concept Figma của bạn
        string adminEmail = "admin@medsuggest.vn";
        string adminPassword = "AdminPassword123!"; // Mật khẩu có chữ hoa, chữ thường, số và ký tự đặc biệt theo chuẩn bảo mật

        // Kiểm tra xem tài khoản admin này đã tồn tại trong bảng AspNetUsers chưa
        var accountExist = await userManager.FindByEmailAsync(adminEmail);

        if (accountExist == null)
        {
            // Tạo mới đối tượng TaiKhoan Admin với đầy đủ trường thông tin mở rộng
            var adminUser = new TaiKhoan
            {
                UserName = adminEmail,
                Email = adminEmail,
                HoTen = "Quản Trị Viên Hệ Thống",
                GioiTinh = "Nam",
                NgaySinh = new System.DateTime(1990, 1, 1),
                VaiTro = "Admin", // Gán quyền Admin tối cao
                EmailConfirmed = true
            };

            // Tiến hành tạo và mã hóa mật khẩu tự động
            IdentityResult createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (createAdminResult.Succeeded)
            {
                System.Diagnostics.Debug.WriteLine("👉 Khởi tạo tài khoản Admin mặc định thành công!");
            }
        }
    }
    catch (System.Exception ex)
    {
        System.Diagnostics.Debug.WriteLine("❌ Lỗi khởi tạo dữ liệu Admin: " + ex.Message);
    }
}
// -----------------------------------------------------------

app.Run();
