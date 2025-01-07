using Microsoft.EntityFrameworkCore;
using MyQuotesLibrary.Client.Data;
using MyQuotesLibrary.Client.SqliteHelpers;

namespace MyQuotesLibrary.Client.Services
{
    public class TagService
    {
        private readonly ILogger<QuoteService> logger;
        private readonly ISqliteWasmDbContextFactory<MyQuotesDbContext> dbContextFactory;

        public TagService(ILogger<QuoteService> logger,
            ISqliteWasmDbContextFactory<MyQuotesDbContext> dbContextFactory)
        {
            this.logger = logger;
            this.dbContextFactory = dbContextFactory;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            using var DbContext = await dbContextFactory.CreateDbContextAsync();

            return await DbContext.Tags.ToListAsync();
        }

        public async Task<Tag> Add(string text)
        {
            using var DbContext = await dbContextFactory.CreateDbContextAsync();

            var tag = new Tag
            {
                Text = text
            };

            await DbContext.Tags.AddAsync(tag);

            return tag;
        }

        public async Task DeleteAsync(int tagId)
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();

            var tag = dbContext.Tags.Find(tagId) ?? throw new ArgumentException($"No tag found for Id {tagId}");

            tag.Quotes?.Clear();

            dbContext.Tags.Remove(tag);

            await dbContext.SaveChangesAsync();

        }
    }
}
