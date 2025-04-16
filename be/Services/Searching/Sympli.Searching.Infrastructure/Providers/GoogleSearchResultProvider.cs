using Sympli.Searching.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using Sympli.Searching.Core.Entities;
using Microsoft.Extensions.Options;
using Sympli.Searching.Core.Constants;
using System.Text.RegularExpressions;
using Sympli.Searching.Core.Utilities;
using Microsoft.Extensions.Caching.Memory;

namespace Sympli.Searching.Infrastructure.Providers
{
    /// <summary>
    /// Implementation of ISearchResultProvider using the Google Custom Search API.
    /// </summary>
    public class GoogleSearchResultProvider : ISearchResultProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly SearchSetting _searchSetting;
        private readonly int _expireTime;

        public GoogleSearchResultProvider(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings, IMemoryCache cache)
        {
            _httpClient = httpClientFactory.CreateClient(CommonConstants.GoogleSearchClient);
            _searchSetting = appSettings.Value.GoogleSearch;
            _expireTime = appSettings.Value.CacheingExpireation;
            _cache = cache;
        }

        public async Task<string> GetRankPositionsAsync(string keyword, string targetUrl)
        {
            string cacheKey = $"google:{keyword}:{targetUrl}".ToLowerInvariant();

            //if (_cache.TryGetValue(cacheKey, out string cachedResult))
            //{
            //    return cachedResult;
            //}

            var ranks = new List<int>();
            var start = 0;
            var position = 1;
            int maxResults = _searchSetting.Limit;

            while (start < maxResults)
            {
                var requestUri = $"/search?q={Uri.EscapeDataString(keyword)}&start={start}";

                var response = await _httpClient.GetAsync(requestUri);

                if (response == null || !response.IsSuccessStatusCode) return "0";

                var content = await response.Content.ReadAsStringAsync();
                var matches = Regex.Matches(content, @"<a[^>]+href=\""(?<url>https?://[^\""]+)\""[^>]*>");

                foreach (Match match in matches)
                {
                    var resultUrl = match.Groups["url"].Value;

                    if (ParseUrlHelper.NormalizeUrl(resultUrl).Contains(ParseUrlHelper.NormalizeUrl(targetUrl), StringComparison.OrdinalIgnoreCase))
                    {
                        ranks.Add(position);
                    }

                    position++;
                    if (position > maxResults)
                        break;
                }

                start += 10;
                await Task.Delay(1000); // polite delay between requests
            }
            var result = ranks.Count > 0 ? string.Join(", ", ranks) : "0";
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_expireTime));
            return result;
        }

    }
}
