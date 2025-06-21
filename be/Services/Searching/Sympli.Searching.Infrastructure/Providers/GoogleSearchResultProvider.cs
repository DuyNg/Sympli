using Sympli.Searching.Core.Interfaces;
using Sympli.Searching.Core.Entities;
using Microsoft.Extensions.Options;
using Sympli.Searching.Core.Constants;
using System.Text.RegularExpressions;
using Sympli.Searching.Core.Utilities;
using Sympli.Searching.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace Sympli.Searching.Infrastructure.Providers
{
    /// <summary>
    /// Implementation of ISearchResultProvider using the Google Custom Search API.
    /// </summary>
    public class GoogleSearchResultProvider : ISearchResultProvider
    {
        private readonly IHttpClientWrapper _httpClient;
        private readonly ICacheService _cache;
        private readonly SearchSetting _searchSetting;
        private readonly int _expireTime;
        private readonly ILogger<GoogleSearchResultProvider> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="appSettings"></param>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        public GoogleSearchResultProvider(
            IHttpClientWrapper httpClient,
            IOptions<AppSettings> appSettings,
            ICacheService cache,
            ILogger<GoogleSearchResultProvider> logger)
        {
            _httpClient = httpClient;
            _searchSetting = appSettings.Value.GoogleSearch;
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

            string cacheKey = $"google:{keyword}:{targetUrl}".ToLowerInvariant();

            if (_cache.TryGetValue(cacheKey, out string cachedResult))
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                return cachedResult;
            }

            var ranks = new List<int>();
            var start = 0;
            var position = 1;
            int maxResults = _searchSetting.Limit;

            while (start < maxResults)
            {
                var requestUri = $"/search?q={Uri.EscapeDataString(keyword)}&start={start}";
                _logger.LogInformation("Fetching Google results for keyword: {Keyword}, start: {Start}", keyword, start);

                var response = await _httpClient.GetAsync(CommonConstants.GoogleSearchClient, requestUri);

                if (response == null)
                {
                    _logger.LogError("Google API returned null response for URI: {RequestUri}", requestUri);
                    throw new SearchProviderException("Failed to fetch results from Google: null response.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        _logger.LogWarning("Rate limit exceeded for Google API.");
                        throw new SearchProviderException("Rate limit exceeded for Google Search.");
                    }
                    _logger.LogError("Google API request failed with status: {StatusCode}, URI: {RequestUri}", response.StatusCode, requestUri);
                    throw new SearchProviderException($"Google API request failed with status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                {
                    _logger.LogWarning("Google API returned empty content for URI: {RequestUri}", requestUri);
                    throw new SearchProviderException("Google API returned empty content.");
                }

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
                await Task.Delay(500); // Polite delay between requests
            }

            var result = ranks.Count > 0 ? string.Join(", ", ranks) : "0";
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_expireTime));
            _logger.LogInformation("Cached result for key: {CacheKey}, Result: {Result}", cacheKey, result);
            return result;
        }
    }
}