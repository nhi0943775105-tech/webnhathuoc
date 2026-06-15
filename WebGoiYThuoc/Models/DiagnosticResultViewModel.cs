using System.Collections.Generic;

namespace WebGoiYThuoc.Models
{
    public class DiagnosticResultViewModel
    {
        public Benh? BenhDuDoan { get; set; }
        public double TyLeTrungKhop { get; set; } // Ví dụ: 85.5%
        public List<TrieuChung> TrieuChungPhuHop { get; set; } = new List<TrieuChung>();
        public List<TrieuChung> TrieuChungThieu { get; set; } = new List<TrieuChung>();
        public List<Thuoc> ThuocGoiY { get; set; } = new List<Thuoc>();
    }
}