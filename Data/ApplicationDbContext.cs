using DoWellAdvanced.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoWellAdvanced.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Spreadsheet> Spreadsheets { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<SpreadsheetTag> SpreadsheetTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship
            modelBuilder.Entity<SpreadsheetTag>()
                .HasKey(st => new { st.SpreadsheetId, st.TagId });

            // Seed data voor Tags (deze hebben geen UserId nodig)
            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "Werk", IsVisible = true },
                new Tag { Id = 2, Name = "Persoonlijk", IsVisible = true },
                new Tag { Id = 3, Name = "Project", IsVisible = true },
                new Tag { Id = 4, Name = "Planning", IsVisible = true },
                new Tag { Id = 5, Name = "Budget", IsVisible = true }
            );
        }
    }
}
