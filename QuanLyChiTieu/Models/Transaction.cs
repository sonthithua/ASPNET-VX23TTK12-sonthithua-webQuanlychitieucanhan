using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuChi.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string Note { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(10)]
        public string Type { get; set; } // <-- Dòng này bị thiếu

        public virtual User User { get; set; }
        public virtual Category Category { get; set; }
    }
}
