using Microsoft.EntityFrameworkCore;
using WebApplication4.Models;

namespace WebApplication4.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } 
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction > Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
   
    }

   
}
