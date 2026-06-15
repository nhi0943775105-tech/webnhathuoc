using System.ComponentModel.DataAnnotations;

namespace WebGoiYThuoc.Models
{
    public class Thuoc
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string TenThuoc { get; set; } = string.Empty;
        public string? CongDung { get; set; }

        // Quan hệ nhiều - nhiều với Bệnh
        public List<Benh> Benhs { get; set; } = new();
    }
}