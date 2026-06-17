using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGoiYThuoc.Data;
using WebGoiYThuoc.Models;

namespace WebGoiYThuoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // TRANG DASHBOARD CHÍNH (Đã nâng cấp nạp dữ liệu liên kết và giữ Tab)
        public async Task<IActionResult> Index(string activeTab = "tab-benh")
        {
            // Tải danh sách bệnh kèm theo Triệu chứng và Thuốc đã gán để phục vụ bảng thống kê dữ liệu liên kết
            var danhhSachBenh = await _context.Benhs
                .Include(b => b.TrieuChungs)
                .Include(b => b.Thuocs)
                .ToListAsync();

            // NẠP DỮ LIỆU SANG VIEW CHO CỘT 2 VÀ CỘT 3 HẾT BỊ LỖI TRỐNG
            ViewBag.TrieuChungs = await _context.TrieuChungs.ToListAsync();
            ViewBag.Thuocs = await _context.Thuocs.ToListAsync();

            // Gửi tên Tab hiện tại sang View để giao diện tự động bật sáng Tab đó lên
            ViewBag.ActiveTab = activeTab;

            return View(danhhSachBenh);
        }

        // XỬ LÝ THÊM BỆNH
        [HttpPost]
        public async Task<IActionResult> AddBenh(Benh model)
        {
            if (!string.IsNullOrEmpty(model.TenBenh))
            {
                _context.Benhs.Add(model);
                await _context.SaveChangesAsync();
            }
            // Giữ Admin ở lại Tab bệnh lý
            return RedirectToAction(nameof(Index), new { activeTab = "tab-benh" });
        }

        // XỬ LÝ THÊM THUỐC
        [HttpPost]
        public async Task<IActionResult> AddThuoc(Thuoc model)
        {
            if (!string.IsNullOrEmpty(model.TenThuoc))
            {
                _context.Thuocs.Add(model);
                await _context.SaveChangesAsync();
            }
            // Giữ Admin ở lại Tab thuốc
            return RedirectToAction(nameof(Index), new { activeTab = "tab-thuoc" });
        }

        // XỬ LÝ THÊM TRIỆU CHỨNG
        [HttpPost]
        public async Task<IActionResult> AddTrieuChung(TrieuChung model)
        {
            if (!string.IsNullOrEmpty(model.TenTrieuChung))
            {
                _context.TrieuChungs.Add(model);
                await _context.SaveChangesAsync();
            }
            // Giữ Admin ở lại Tab triệu chứng
            return RedirectToAction(nameof(Index), new { activeTab = "tab-trieuchung" });
        }

        // XỬ LÝ GÁN GHÉP TRIỆU CHỨNG VÀ THUỐC CHO BỆNH (Đã sửa luồng giữ Tab liên kết)
        [HttpPost]
        public async Task<IActionResult> LinkBenhDetail(int benhId, List<int> selectedTrieuChungs, List<int> selectedThuocs)
        {
            // 1. Tìm bệnh cần cấu hình (bao gồm cả danh sách liên kết hiện tại của nó)
            var benh = await _context.Benhs
                .Include(b => b.TrieuChungs)
                .Include(b => b.Thuocs)
                .FirstOrDefaultAsync(b => b.Id == benhId);

            if (benh != null)
            {
                // 2. Cập nhật mối quan hệ Nhiều - Nhiều với Triệu Chứng
                benh.TrieuChungs.Clear(); // Xóa liên kết cũ để nạp lại từ đầu
                if (selectedTrieuChungs != null)
                {
                    var trieuChungs = await _context.TrieuChungs
                        .Where(t => selectedTrieuChungs.Contains(t.Id)).ToListAsync();
                    benh.TrieuChungs.AddRange(trieuChungs);
                }

                // 3. Cập nhật mối quan hệ Nhiều - Nhiều với Thuốc Gợi Ý
                benh.Thuocs.Clear(); // Xóa liên kết cũ
                if (selectedThuocs != null)
                {
                    var thuocs = await _context.Thuocs
                        .Where(t => selectedThuocs.Contains(t.Id)).ToListAsync();
                    benh.Thuocs.AddRange(thuocs);
                }

                // 4. Lưu thay đổi xuống SQL Server
                await _context.SaveChangesAsync();
            }

            // ĐẶC BIỆT: Sau khi lưu xong, ép hệ thống ở lại đúng Tab cấu hình liên kết
            return RedirectToAction(nameof(Index), new { activeTab = "tab-lienket" });
        }
    }
}