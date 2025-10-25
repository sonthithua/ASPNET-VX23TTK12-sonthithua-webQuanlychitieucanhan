using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuChi.Models
{
    [Table("Budgets")]
    public class Budgets
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Số tiền ngân sách")]
        public decimal Amount { get; set; }

        // ✅ Thêm 2 dòng này:
        [Required]
        [Display(Name = "Tháng")]
        public int Month { get; set; }

        [Required]
        [Display(Name = "Năm")]
        public int Year { get; set; }

        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [StringLength(255)]
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
