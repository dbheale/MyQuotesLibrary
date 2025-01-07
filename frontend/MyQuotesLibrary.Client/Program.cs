using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyQuotesLibrary.Client;
using MyQuotesLibrary.Client.Data;
using MyQuotesLibrary.Client.SqliteHelpers;
using Microsoft.EntityFrameworkCore;
using MyQuotesLibrary.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSqliteWasmDbContextFactory<MyQuotesDbContext>(options =>
    options.UseSqlite($"Data Source=myQuotes.sqlite3"));

builder.Services.AddScoped<QuoteService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
