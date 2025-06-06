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
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<GoalContribution> GoalContributions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Goal>(entity =>
            {
                // Chuyển enum GoalType sang string varchar(50)
                entity.Property(e => e.GoalType)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired()
                    .HasDefaultValue(GoalType.Other);

                // Chuyển enum GoalPriority sang string varchar(20)
                entity.Property(e => e.Priority)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired()
                    .HasDefaultValue(GoalPriority.Medium);

                // Cấu hình thêm cho các trường khác nếu cần
            });
        }
    }
}
