// <copyright file="BrowserCache.cs" company="Jeremy Likness">
// Copyright (c) Jeremy Likness. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;

namespace MyQuotesLibrary.Client.SqliteHelpers
{
    /// <summary>
    /// Wrapper for JavaScript code to synchronize the database.
    /// </summary>
    public sealed class BrowserCache : IAsyncDisposable, IBrowserCache
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserCache"/> class.
        /// </summary>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/> instance.</param>
        public BrowserCache(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./browserCache.js").AsTask()!);
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }

        public async Task<byte[]?> GetDataFromIndexedDbAsync(string filename)
        {
            var module = await moduleTask.Value;
            
            var base64String = await module.InvokeAsync<string>("getDataFromIndexedDb", filename);

            if (!string.IsNullOrEmpty(base64String))
            {
                return Convert.FromBase64String(base64String);
            }
            return Array.Empty<byte>();
        }

        public async Task<int> SaveDataToIndexedDb(string filename, byte[] data)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<int>("saveDataToIndexedDb", filename, data);
        }

        public async Task<bool> GenerateDownloadLinkAsync(ElementReference parent, string filename)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<bool>("generateDownloadLink", parent, filename);
        }
    }
}
