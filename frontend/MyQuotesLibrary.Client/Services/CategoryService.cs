using Microsoft.EntityFrameworkCore;
using MyQuotesLibrary.Client.Data;
using MyQuotesLibrary.Client.SqliteHelpers;

namespace MyQuotesLibrary.Client.Services
{
    public class CategoryService
    {
        private readonly ILogger<QuoteService> logger;
        private readonly ISqliteWasmDbContextFactory<MyQuotesDbContext> dbContextFactory;

        public CategoryService(ILogger<QuoteService> logger,
            ISqliteWasmDbContextFactory<MyQuotesDbContext> dbContextFactory)
        {
            this.logger = logger;
            this.dbContextFactory = dbContextFactory;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            using var DbContext = await dbContextFactory.CreateDbContextAsync();

            return await DbContext.Categories.ToListAsync();
        }

        public async Task<Category> AddAsync(string text)
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();

            var category = new Category
            {
                Name = text
            };

            await dbContext.Categories.AddAsync(category);

            return category;
        }

        public async Task DeleteAsync(int categoryId)
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();

            var category = dbContext.Categories.Find(categoryId) ?? throw new ArgumentException($"No category found for Id {categoryId}");

            category.Quotes?.Clear();
            
            dbContext.Categories.Remove(category);

            await dbContext.SaveChangesAsync();

        }
    }
}
