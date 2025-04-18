using Sympli.Searching.Core.Interfaces;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;
using Sympli.Searching.Core.Entities;
using Microsoft.Extensions.Options;
using Sympli.Searching.Core.Constants;
using System.Text.RegularExpressions;
using Sympli.Searching.Core.Utilities;
using Microsoft.Extensions.Caching.Memory;

namespace Sympli.Searching.Infrastructure.Providers
{
    /// <summary>
    /// Implementation of ISearchResultProvider using the Bing Search API.
    /// </summary>
    public class BingSearchResultProvider : ISearchResultProvider
    {
        private readonly IHttpClientWrapper _httpClient;
        private readonly ICacheService _cache;
        private readonly SearchSetting _searchSetting;
        private readonly int _expireTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="appSettings"></param>
        /// <param name="cache"></param>
        public BingSearchResultProvider(IHttpClientWrapper httpClient, IOptions<AppSettings> appSettings, ICacheService cache)
        {
            _httpClient = httpClient;
            _searchSetting = appSettings.Value.BingSearch;
            _expireTime = appSettings.Value.CacheingExpireation;
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="targetUrl"></param>
        /// <returns></returns>
        public async Task<string> GetRankPositionsAsync(string keyword, string targetUrl)
        {
            string cacheKey = $"bing:{keyword}:{targetUrl}".ToLowerInvariant();

            if (_cache.TryGetValue(cacheKey, out string cachedResult))
            {
                return cachedResult;
            }

            var ranks = new List<int>();
            var first = 0;
            var position = 1;
            int maxResults = _searchSetting.Limit;

            while (position <= maxResults)
            {
                var requestUri = $"search?q={Uri.EscapeDataString(keyword)}&first={first + 1}";
                var response = await _httpClient.GetStringAsync(CommonConstants.BingSearchClient, requestUri);

                // Simplified: Parse URLs from Bing results
                var matches = Regex.Matches(response, @"<a href=""(http[^""]+)""");
                foreach (Match match in matches)
                {
                    var resultUrl = ParseUrlHelper.NormalizeUrl(match.Groups[1].Value);
                    if (resultUrl.Contains(ParseUrlHelper.NormalizeUrl(targetUrl), StringComparison.OrdinalIgnoreCase))
                    {
                        ranks.Add(position);
                    }

                    position++;
                    if (position > maxResults) break;
                }

                first += 10;
                await Task.Delay(1000); // Delay for courtesy
            }

            var result = ranks.Count > 0 ? string.Join(", ", ranks) : "0";
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_expireTime));
            return result;
        }
    }
}
