using Microsoft.EntityFrameworkCore;

namespace MyQuotesLibrary.Client.Data
{
    public class MyQuotesDbContext : DbContext
    {
        public MyQuotesDbContext(DbContextOptions<MyQuotesDbContext> options) : base(options) { }

        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<QuoteTag> QuoteTags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<QuoteCategory> QuoteCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quote>().ToTable("Quotes");
            modelBuilder.Entity<Tag>().ToTable("Tags");
            modelBuilder.Entity<QuoteTag>().ToTable("QuoteTags");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<QuoteCategory>().ToTable("QuoteCategories");

            modelBuilder.Entity<QuoteTag>()
                .HasKey(qt => new { qt.QuoteId, qt.TagId });

            modelBuilder.Entity<QuoteTag>()
                .HasOne(qt => qt.Quote)
                .WithMany(q => q.Tags)
                .HasForeignKey(qt => qt.QuoteId);

            modelBuilder.Entity<QuoteTag>()
                .HasOne(qt => qt.Tag)
                .WithMany(t => t.Quotes)
                .HasForeignKey(qt => qt.TagId);

            modelBuilder.Entity<QuoteCategory>()
                .HasKey(qc => new { qc.QuoteId, qc.CategoryId });

            modelBuilder.Entity<QuoteCategory>()
                .HasOne(qc => qc.Quote)
                .WithMany(q => q.Categories)
                .HasForeignKey(qc => qc.QuoteId);

            modelBuilder.Entity<QuoteCategory>()
                .HasOne(qc => qc.Category)
                .WithMany(c => c.Quotes)
                .HasForeignKey(qc => qc.CategoryId);

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

        public virtual ICollection<QuoteTag> Tags { get; set; }
        public virtual ICollection<QuoteCategory> Categories { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public virtual ICollection<QuoteTag> Quotes { get; set; }
    }

    public class QuoteTag
    {
        public int QuoteId { get; set; }
        public virtual Quote Quote { get; set; }
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<QuoteCategory> Quotes { get; set; }
    }

    public class QuoteCategory
    {
        public int QuoteId { get; set; }
        public virtual Quote Quote { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}