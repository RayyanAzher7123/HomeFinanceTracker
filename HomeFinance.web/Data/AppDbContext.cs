using Microsoft.EntityFrameworkCore;
using HomeFinance.web.Models;


namespace HomeFinance.web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Expense> Expenses { get; set; }
    }
}
