using Microsoft.EntityFrameworkCore;
using HomeFinance.web.Models;


namespace HomeFinance.web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Store>().HasData(
                new Store { StoreId = 1, StoreName = "Walmart" },
                new Store { StoreId = 2, StoreName = "Sobeys" },
                new Store { StoreId = 3, StoreName = "Atlantic Superstore" }
             );

        }
    }
}
