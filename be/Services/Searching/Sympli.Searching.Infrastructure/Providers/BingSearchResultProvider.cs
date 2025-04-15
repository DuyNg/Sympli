using Sympli.Searching.Core.Interfaces;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;

namespace Sympli.Searching.Infrastructure.Providers
{
    /// <summary>
    /// Implementation of ISearchResultProvider using the Bing Search API.
    /// </summary>
    public class BingSearchResultProvider : ISearchResultProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public BingSearchResultProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IEnumerable<string>> GetResultsAsync(string keyword)
        {
            string apiKey = _configuration["BingSearch:ApiKey"];

            // Create a client configured for Bing searches.
            var client = _httpClientFactory.CreateClient("BingSearchClient");
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

            // Construct a simple request URL using the keyword.
            var requestUri = $"?q={Uri.EscapeDataString(keyword)}";

            var response = await client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var results = new List<string>();

            try
            {
                using JsonDocument jsonDoc = JsonDocument.Parse(content);
                // Simplified parsing logic: adapt based on the actual Bing API JSON schema.
                if (jsonDoc.RootElement.TryGetProperty("webPages", out var webPages) &&
                    webPages.TryGetProperty("value", out var items))
                {
                    foreach (var item in items.EnumerateArray())
                    {
                        if (item.TryGetProperty("name", out var name))
                        {
                            results.Add(name.GetString() ?? "No Title");
                        }
                    }
                }
            }
            catch (JsonException)
            {
                results.Add("Error parsing Bing results.");
            }

            if (results.Count == 0)
            {
                results.Add("No results found from Bing.");
            }

            return results;
        }
    }
}
