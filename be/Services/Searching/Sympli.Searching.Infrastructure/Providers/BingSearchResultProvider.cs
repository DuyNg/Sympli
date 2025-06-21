using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sympli.Searching.Core.Constants;
using Sympli.Searching.Core.Entities;
using Sympli.Searching.Core.Exceptions;
using Sympli.Searching.Core.Interfaces;
using Sympli.Searching.Core.Utilities;
using System.Text.RegularExpressions;

namespace Sympli.Searching.Infrastructure.Providers
{
    /// <summary>
    /// /// Represents a provider for Bing search results.
    /// </summary>
    public class BingSearchResultProvider : ISearchResultProvider
    {
        private readonly IHttpClientWrapper _httpClient;
        private readonly ICacheService _cache;
        private readonly SearchSetting _searchSetting;
        private readonly int _expireTime;
        private readonly ILogger<BingSearchResultProvider> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="appSettings"></param>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        public BingSearchResultProvider(
            IHttpClientWrapper httpClient,
            IOptions<AppSettings> appSettings,
            ICacheService cache,
            ILogger<BingSearchResultProvider> logger)
        {
            _httpClient = httpClient;
            _searchSetting = appSettings.Value.BingSearch;
            _expireTime = appSettings.Value.CacheingExpireation;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="targetUrl"></param>
        /// <returns></returns>
        /// <exception cref="SearchProviderException"></exception>
        public async Task<string> GetRankPositionsAsync(string keyword, string targetUrl)
        {
            if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(targetUrl))
            {
                _logger.LogWarning("Invalid input: keyword or targetUrl is empty.");
                throw new SearchProviderException("Keyword and target URL cannot be empty.");
            }

            string cacheKey = $"bing:{keyword}:{targetUrl}".ToLowerInvariant();

            if (_cache.TryGetValue(cacheKey, out string cachedResult))
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                return cachedResult;
            }

            var ranks = new List<int>();
            var first = 0;
            var position = 1;
            int maxResults = _searchSetting.Limit;

            while (position <= maxResults)
            {
                var requestUri = $"?q={Uri.EscapeDataString(keyword)}&first={first + 1}";
                _logger.LogInformation("Fetching Bing results for keyword: {Keyword}, first: {First}", keyword, first + 1);

                string content;
                try
                {
                    content = await _httpClient.GetStringAsync(CommonConstants.BingSearchClient, requestUri);
                }
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        _logger.LogWarning("Rate limit exceeded for Bing API.");
                        throw new SearchProviderException("Rate limit exceeded for Bing Search.");
                    }
                    _logger.LogError(ex, "Bing API request failed for URI: {RequestUri}", requestUri);
                    throw new SearchProviderException($"Bing API request failed: {ex.Message}");
                }

                if (string.IsNullOrEmpty(content))
                {
                    _logger.LogWarning("Bing API returned empty content for URI: {RequestUri}", requestUri);
                    throw new SearchProviderException("Bing API returned empty content.");
                }

                var matches = Regex.Matches(content, @"<a href=""(http[^""]+)""");
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
                await Task.Delay(500); // Delay for courtesy
            }

            var result = ranks.Count > 0 ? string.Join(", ", ranks) : "0";
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_expireTime));
            _logger.LogInformation("Cached result for key: {CacheKey}, Result: {Result}", cacheKey, result);
            return result;
        }
    }
}