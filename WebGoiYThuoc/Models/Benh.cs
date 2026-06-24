using System.ComponentModel.DataAnnotations;
using WebGoiYThuoc.Models;

namespace WebGoiYThuoc.Models
{
    public class Benh
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string TenBenh { get; set; } = string.Empty;
        public string? MoTaBenh { get; set; }
        public string? LoiKhuyen { get; set; }

        // Mối quan hệ nhiều - nhiều với Triệu Chứng và Thuốc
        public List<TrieuChung> TrieuChungs { get; set; } = new();
        public List<Thuoc> Thuocs { get; set; } = new();
    }
}