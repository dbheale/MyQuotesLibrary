// <copyright file="IBrowserCache.cs" company="Jeremy Likness">
// Copyright (c) Jeremy Likness. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Components;

namespace MyQuotesLibrary.Client.SqliteHelpers
{
    /// <summary>
    /// Wrapper for JavaScript module functions that interact with the cache.
    /// </summary>
    public interface IBrowserCache
    {
        Task<byte[]?> GetDataFromIndexedDbAsync(string filename);
        Task<bool> GenerateDownloadLinkAsync(ElementReference parent, string filename);
        Task<int> SaveDataToIndexedDb(string filename, byte[] data);
    }
}
