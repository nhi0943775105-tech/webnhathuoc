using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebGoiYThuoc.Models
{
    // Kế thừa IdentityUser để lấy toàn bộ các trường Email, Số điện thoại, Cơ chế bảo mật mật khẩu
    public class TaiKhoan : IdentityUser
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [StringLength(10)]
        public string GioiTinh { get; set; } = "Nam"; // Gồm 2 giá trị từ Figma: "Nam" hoặc "Nữ"

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string VaiTro { get; set; } = "User"; // Gồm 2 giá trị: "Admin" hoặc "User"
    }
}