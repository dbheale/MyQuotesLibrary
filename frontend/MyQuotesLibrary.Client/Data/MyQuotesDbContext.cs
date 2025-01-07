using Microsoft.EntityFrameworkCore;

namespace MyQuotesLibrary.Client.Data
{
    public class MyQuotesDbContext : DbContext
    {
        public MyQuotesDbContext(DbContextOptions<MyQuotesDbContext> options) : base(options) { }

        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quote>(e =>
            {
                e.ToTable("Quotes");
                e.HasKey(k => k.Id);

                e.HasMany(s => s.Tags)
                .WithMany(c => c.Quotes)
                .UsingEntity<Dictionary<string, object>>(
                    "QuoteTags",
                    j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                    j => j.HasOne<Quote>().WithMany().HasForeignKey("QuoteId")
                );

                e.HasMany(s => s.Categories)
                .WithMany(c => c.Quotes)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoryTags",
                    j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                    j => j.HasOne<Quote>().WithMany().HasForeignKey("QuoteId")
                );

            });
            modelBuilder.Entity<Tag>().ToTable("Tags");
            modelBuilder.Entity<Category>().ToTable("Categories");

            // Seeding default categories and tags
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Inspiration" },
                new Category { Id = 2, Name = "Motivation" },
                new Category { Id = 3, Name = "Life Lessons" }
            );

            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Text = "Wisdom" },
                new Tag { Id = 2, Text = "Courage" },
                new Tag { Id = 3, Text = "Success" }
            );
        }
    }

    public class Quote
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Tag>? Tags { get; set; }
        public virtual ICollection<Category>? Categories { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public virtual ICollection<Quote>? Quotes { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Quote>? Quotes { get; set; }
    }
}