using System.ComponentModel.DataAnnotations;

namespace WebGoiYThuoc.Models
{
    public class TrieuChung
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string TenTrieuChung { get; set; } = string.Empty;
        public string? MoTa { get; set; }

        // Quan hệ nhiều - nhiều với Bệnh
        public List<Benh> Benhs { get; set; } = new();
    }
}