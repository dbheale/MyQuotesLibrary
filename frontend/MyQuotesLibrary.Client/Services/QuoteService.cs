using Microsoft.EntityFrameworkCore;
using MyQuotesLibrary.Client.Data;
using MyQuotesLibrary.Client.SqliteHelpers;

namespace MyQuotesLibrary.Client.Services
{
    public class QuoteService
    {
        private readonly ILogger<QuoteService> logger;
        private readonly ISqliteWasmDbContextFactory<MyQuotesDbContext> dbContextFactory;

        public QuoteService(ILogger<QuoteService> logger,
            ISqliteWasmDbContextFactory<MyQuotesDbContext> dbContextFactory)
        {
            this.logger = logger;
            this.dbContextFactory = dbContextFactory;
        }

        public async Task<IEnumerable<Quote>> GetAllAsync()
        {
            using var DbContext = await dbContextFactory.CreateDbContextAsync();

            return await DbContext.Quotes.ToListAsync();
        }

        public async Task<Quote> AddAsync(Quote quote)
        {
            using var DbContext = await dbContextFactory.CreateDbContextAsync();
            var categoryIds = quote.Categories?.Select(s => s.Id).ToArray() ?? [];
            var tagIds = quote.Tags?.Select(s => s.Id).ToArray() ?? [];
            var categories = DbContext.Categories.Where(w => categoryIds.Contains(w.Id)).ToList();
            var tags = DbContext.Tags.Where(w => tagIds.Contains(w.Id)).ToList();
            var newQuote = new Quote
            {
                Text = quote.Text,
                Author = quote.Author,
                CreatedAt = DateTime.UtcNow,
                Categories = categories,
                Tags = tags
            };
            DbContext.Quotes.Add(newQuote);
            await DbContext.SaveChangesAsync();
            logger.LogDebug("Result: {@model}", newQuote);
            return quote;
        }

        public async Task<Quote> Update(Quote quote)
        {
            using (var DbContext = await dbContextFactory.CreateDbContextAsync())
            {
                var dbQuote = DbContext.Quotes.Find(quote.Id) ?? throw new ArgumentException($"Quote not found by Id {quote.Id}");
                dbQuote.Text = quote.Text;
                dbQuote.Author = quote.Author;
                dbQuote.Categories.Clear();
                foreach (var category in quote.Categories)
                {
                    dbQuote.Categories.Add(category);
                }
                dbQuote.Tags.Clear();
                foreach (var tag in quote.Tags)
                {
                    dbQuote.Tags.Add(tag);
                }
                await DbContext.SaveChangesAsync();
            }
            logger.LogDebug("Result: {@model}", quote);
            return quote;
        }



        public async Task Delete(int quoteId)
        {
            using (var DbContext = await dbContextFactory.CreateDbContextAsync())
            {
                var dbQuote = DbContext.Quotes.Find(quoteId) ?? throw new ArgumentException($"Quote not found by Id {quoteId}");
                DbContext.Quotes.Remove(dbQuote);
                await DbContext.SaveChangesAsync();
            }
        }


    }
}
