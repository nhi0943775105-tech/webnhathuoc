using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebGoiYThuoc.Models;

namespace WebGoiYThuoc.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;

        public AccountController(UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GIAO DIỆN ĐĂNG KÝ
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // XỬ LÝ ĐĂNG KÝ THÔNG TIN TỪ FIGMA
        [HttpPost]
        public async Task<IActionResult> Register(string hoTen, string email, string gioiTinh, System.DateTime ngaySinh, string password)
        {
            var userExist = await _userManager.FindByEmailAsync(email);
            if (userExist != null)
            {
                ViewBag.Error = "Địa chỉ email này đã được sử dụng!";
                return View();
            }

            var newAccount = new TaiKhoan
            {
                UserName = email, // Lấy Email làm UserName đăng nhập
                Email = email,
                HoTen = hoTen,
                GioiTinh = gioiTinh,
                NgaySinh = ngaySinh,
                VaiTro = "User" // Mặc định tự tạo trên Web là User thường
            };

            var result = await _userManager.CreateAsync(newAccount, password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(newAccount, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            string errorMsg = "";
            foreach (var error in result.Errors) { errorMsg += error.Description + " "; }
            ViewBag.Error = errorMsg;
            return View();
        }

        // GIAO DIỆN ĐĂNG NHẬP
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // XỬ LÝ ĐĂNG NHẬP
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // Thực hiện kiểm tra mật khẩu
                var result = await _signInManager.PasswordSignInAsync(user.UserName!, password, isPersistent: true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Nạp vai trò thực tế từ database vào Cookie đăng nhập để phân quyền thao tác
                    var claims = new System.Collections.Generic.List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.VaiTro)
            };
                    await _signInManager.SignInWithClaimsAsync(user, isPersistent: true, claims);

                    // SỬA TẠI ĐÂY: Dù tài khoản là Admin hay User thì đều đi thẳng về Trang chủ người dùng
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác!";
            return View();
        }

        // CHỨC NĂNG ĐĂNG XUẤT
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}