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

        // TRANG DASHBOARD CHÍNH
        public async Task<IActionResult> Index(string activeTab = "tab-benh")
        {
            var danhhSachBenh = await _context.Benhs
                .Include(b => b.TrieuChungs)
                .Include(b => b.Thuocs)
                .ToListAsync();

            ViewBag.TrieuChungs = await _context.TrieuChungs.ToListAsync();
            ViewBag.Thuocs = await _context.Thuocs.ToListAsync();
            ViewBag.ActiveTab = activeTab;

            return View(danhhSachBenh);
        }

        [HttpPost]
        public async Task<IActionResult> AddBenh(Benh model)
        {
            if (!string.IsNullOrEmpty(model.TenBenh))
            {
                _context.Benhs.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { activeTab = "tab-benh" });
        }

        [HttpPost]
        public async Task<IActionResult> AddThuoc(Thuoc model)
        {
            if (!string.IsNullOrEmpty(model.TenThuoc))
            {
                _context.Thuocs.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { activeTab = "tab-thuoc" });
        }

        [HttpPost]
        public async Task<IActionResult> AddTrieuChung(TrieuChung model)
        {
            if (!string.IsNullOrEmpty(model.TenTrieuChung))
            {
                _context.TrieuChungs.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { activeTab = "tab-trieuchung" });
        }

        [HttpPost]
        public async Task<IActionResult> LinkBenhDetail(int benhId, List<int> selectedTrieuChungs, List<int> selectedThuocs)
        {
            var benh = await _context.Benhs
                .Include(b => b.TrieuChungs)
                .Include(b => b.Thuocs)
                .FirstOrDefaultAsync(b => b.Id == benhId);

            if (benh != null)
            {
                benh.TrieuChungs.Clear();
                if (selectedTrieuChungs != null)
                {
                    var trieuChungs = await _context.TrieuChungs.Where(t => selectedTrieuChungs.Contains(t.Id)).ToListAsync();
                    benh.TrieuChungs.AddRange(trieuChungs);
                }

                benh.Thuocs.Clear();
                if (selectedThuocs != null)
                {
                    var thuocs = await _context.Thuocs.Where(t => selectedThuocs.Contains(t.Id)).ToListAsync();
                    benh.Thuocs.AddRange(thuocs);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { activeTab = "tab-lienket" });
        }

        // ========== SỬA / XÓA BỆNH ==========
        [HttpGet]
        public async Task<IActionResult> GetBenh(int id)
        {
            var benh = await _context.Benhs.FindAsync(id);
            if (benh == null) return NotFound();
            return Json(new { tenBenh = benh.TenBenh, moTaBenh = benh.MoTaBenh, loiKhuyen = benh.LoiKhuyen });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBenh(int id, string TenBenh, string MoTaBenh, string LoiKhuyen)
        {
            var benh = await _context.Benhs.FindAsync(id);
            if (benh == null) return NotFound();

            if (string.IsNullOrWhiteSpace(TenBenh))
                return BadRequest("Tên bệnh không được để trống.");

            benh.TenBenh = TenBenh;
            benh.MoTaBenh = MoTaBenh ?? "";
            benh.LoiKhuyen = LoiKhuyen ?? "";
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBenh(int id)
        {
            var benh = await _context.Benhs.FindAsync(id);
            if (benh == null) return NotFound();

            _context.Benhs.Remove(benh);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ========== SỬA / XÓA TRIỆU CHỨNG ==========
        [HttpGet]
        public async Task<IActionResult> GetTrieuChung(int id)
        {
            var tc = await _context.TrieuChungs.FindAsync(id);
            if (tc == null) return NotFound();
            return Json(new { tenTrieuChung = tc.TenTrieuChung, moTa = tc.MoTa });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTrieuChung(int id, string TenTrieuChung, string MoTa)
        {
            var tc = await _context.TrieuChungs.FindAsync(id);
            if (tc == null) return NotFound();

            if (string.IsNullOrWhiteSpace(TenTrieuChung))
                return BadRequest("Tên triệu chứng không được để trống.");

            tc.TenTrieuChung = TenTrieuChung;
            tc.MoTa = MoTa ?? "";
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTrieuChung(int id)
        {
            var tc = await _context.TrieuChungs.FindAsync(id);
            if (tc == null) return NotFound();

            _context.TrieuChungs.Remove(tc);
            await _context.SaveChangesAsync();
            return Ok();
        }
        // ========== SỬA / XÓA THUỐC ==========
        [HttpGet]
        public async Task<IActionResult> GetThuoc(int id)
        {
            var thuoc = await _context.Thuocs.FindAsync(id);
            if (thuoc == null) return NotFound();
            return Json(new { tenThuoc = thuoc.TenThuoc, congDung = thuoc.CongDung });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateThuoc(int id, string TenThuoc, string CongDung)
        {
            var thuoc = await _context.Thuocs.FindAsync(id);
            if (thuoc == null) return NotFound();

            if (string.IsNullOrWhiteSpace(TenThuoc))
                return BadRequest("Tên thuốc không được để trống.");

            thuoc.TenThuoc = TenThuoc;
            thuoc.CongDung = CongDung ?? "";
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteThuoc(int id)
        {
            var thuoc = await _context.Thuocs.FindAsync(id);
            if (thuoc == null) return NotFound();

            _context.Thuocs.Remove(thuoc);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}