using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace razorweb.models 
{
    // razorweb.models.MyBlogContext
    public class MyBlogContext : IdentityDbContext<AppUser>
    {
        public MyBlogContext(DbContextOptions<MyBlogContext> options) : base(options)
        {
          //..
          // this.Roles
          // IdentityRole<string>
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

        }

        

        public DbSet<Article> articles { get; set; }
    }
}