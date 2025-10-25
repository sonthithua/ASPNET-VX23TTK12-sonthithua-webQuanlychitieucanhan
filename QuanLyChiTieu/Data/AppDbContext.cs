using System.Data.Entity;
using QuanLyThuChi.Models;

namespace QuanLyThuChi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DefaultConnection") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budgets> Budgets { get; set; }
    }
}
