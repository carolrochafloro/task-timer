using Microsoft.EntityFrameworkCore;
using task_timer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace task_timer.Context
{
    public class TTDbContext : IdentityDbContext<AppUser>
    {
        public TTDbContext(DbContextOptions<TTDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<AppTask> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
