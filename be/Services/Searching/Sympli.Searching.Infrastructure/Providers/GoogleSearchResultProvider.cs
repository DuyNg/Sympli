using Sympli.Searching.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Sympli.Searching.Infrastructure.Providers
{
    /// <summary>
    /// Implementation of ISearchResultProvider using the Google Custom Search API.
    /// </summary>
    public class GoogleSearchResultProvider : ISearchResultProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public GoogleSearchResultProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IEnumerable<string>> GetResultsAsync(string keyword)
        {
            // Retrieve API credentials from configuration.
            string apiKey = _configuration["GoogleSearch:ApiKey"];
            string cx = _configuration["GoogleSearch:CustomSearchEngineId"];

            var client = _httpClientFactory.CreateClient("GoogleSearchClient");
            var requestUri = $"?key={apiKey}&cx={cx}&q={Uri.EscapeDataString(keyword)}";

            var response = await client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var results = new List<string>();

            try
            {
                using JsonDocument jsonDoc = JsonDocument.Parse(content);
                if (jsonDoc.RootElement.TryGetProperty("items", out JsonElement items))
                {
                    foreach (var item in items.EnumerateArray())
                    {
                        if (item.TryGetProperty("title", out JsonElement title))
                        {
                            results.Add(title.GetString() ?? "No Title");
                        }
                    }
                }
            }
            catch (JsonException)
            {
                results.Add("Error parsing results.");
            }

            // Provide a default message if no results are found.
            if (results.Count == 0)
            {
                results.Add("No results found from Google.");
            }

            return results;
        }
    }
}
