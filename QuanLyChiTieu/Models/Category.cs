using System.ComponentModel.DataAnnotations;

namespace QuanLyThuChi.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; } // "Chi" hoặc "Thu"
    }
}
