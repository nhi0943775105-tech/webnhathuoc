using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebGoiYThuoc.Data;
using WebGoiYThuoc.Models;

namespace WebGoiYThuoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // Tiêm IConfiguration vào
        public HomeController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // TRANG CHỦ
        public async Task<IActionResult> Index(string searchQuery = "")
        {
            ViewBag.Diseases = await _context.Benhs.Include(b => b.TrieuChungs).ToListAsync();
            ViewBag.Thuocs = await _context.Thuocs.Include(t => t.Benhs).ThenInclude(b => b.TrieuChungs).ToListAsync();
            ViewBag.InitialSearchQuery = searchQuery;

            var trieuChungs = await _context.TrieuChungs.ToListAsync();
            return View(trieuChungs);
        }

        // TRANG KHẢO SÁT
        public IActionResult Survey()
        {
            return View();
        }

        // XỬ LÝ KHẢO SÁT VÀ CHUYỂN VỀ TRANG CHỦ
        [HttpPost]
        public IActionResult SubmitSurvey(IFormCollection data)
        {
            var symptoms = new List<string>();

            if (data["s_ho"] == "on") symptoms.Add("ho");
            if (data["s_sot"] == "on") symptoms.Add("sốt");
            if (data["s_dauhong"] == "on") symptoms.Add("đau họng");
            if (data["s_khotho"] == "on") symptoms.Add("khó thở");
            if (data["s_daubung"] == "on") symptoms.Add("đau bụng");
            if (data["s_tieuchay"] == "on") symptoms.Add("tiêu chảy");
            if (data["s_buonnon"] == "on") symptoms.Add("buồn nôn");
            if (data["s_dayhoi"] == "on") symptoms.Add("đầy hơi");
            if (data["s_daudau"] == "on") symptoms.Add("đau đầu");
            if (data["s_moivaigay"] == "on") symptoms.Add("mỏi vai gáy");
            if (data["s_tebi"] == "on") symptoms.Add("tê bì");
            if (data["s_phatban"] == "on") symptoms.Add("phát ban");

            string query = string.Join(", ", symptoms);
            if (string.IsNullOrWhiteSpace(query)) query = "Khám tổng quát";

            return RedirectToAction("Index", new { searchQuery = query });
        }

        // =======================================================
        // GỌI API GOOGLE GEMINI VỚI ENDPOINT gemini-flash-latest           
        // =======================================================
        [HttpGet]
        public async Task<IActionResult> GetAIAssistantAdvice(string symptoms)
        {
            if (string.IsNullOrWhiteSpace(symptoms)) return Json(new { advice = "" });

            // Đọc tất cả cấu hình từ 1 nơi: appsettings.json
            string apiKey = _configuration["GeminiApiKey"];
            string apiUrl = _configuration["GeminiApiSettings:ApiUrl"] ?? "https://generativelanguage.googleapis.com/v1beta";
            string model = _configuration["GeminiApiSettings:DefaultModel"] ?? "gemini-flash-latest";
            int timeout = int.TryParse(_configuration["GeminiApiSettings:TimeoutSeconds"], out var t) ? t : 30;

            if (string.IsNullOrEmpty(apiKey)) return Json(new { advice = "🤖 **Lỗi hệ thống:** Chưa tìm thấy API Key." });

            // Prompt giữ nguyên
            string prompt = $"Bạn là một Dược sĩ/Bác sĩ tư vấn y tế trực tuyến. Bệnh nhân có triệu chứng: [{symptoms}]. Hãy đánh giá mức độ nghiêm trọng và đưa ra lời khuyên y tế, cách chăm sóc tại nhà an toàn (Dưới 150 chữ, dùng markdown, dùng biểu tượng cảm xúc y tế). Cuối cùng luôn nhắc họ xem gợi ý thuốc và bệnh theo triệu chứng bên dưới.";

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeout);
            string url = $"{apiUrl}/models/{model}:generateContent?key={apiKey}";

            var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseString);
                    var text = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text").GetString();

                    return Json(new { advice = text });
                }
                return Json(new { advice = $"🤖 **Lỗi AI:** Google từ chối. (Mã: {response.StatusCode})" });
            }
            catch (Exception ex)
            {
                return Json(new { advice = $"🤖 **Lỗi mạng:** Không thể kết nối tới Google Gemini. Chi tiết: {ex.Message}" });
            }
        }

        // Thêm method này vào HomeController của bạn
        public IActionResult TestGeminiAPI()
        {
            return View();
        }
    }
}