using Microsoft.EntityFrameworkCore;
using PierogiesBot.Manager.Models.Entities;

namespace PierogiesBot.Manager.Services
{
    public class AppDbContext : DbContext
    {
        public DbSet<Settings> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}