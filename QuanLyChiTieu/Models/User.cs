using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuChi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; } // 🔹 Thêm dòng này

        public string FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
