using System;
using System.ComponentModel.DataAnnotations;

namespace WebGoiYThuoc.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [Required]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string GioiTinh { get; set; } = "Nam";

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}