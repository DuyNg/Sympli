using Sympli.Searching.Core.Constants;
using Sympli.Searching.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sympli.Searching.Infrastructure.Services
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private IHttpClientFactory _httpClientFactory;
        public HttpClientWrapper(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Creates and configures an <see cref="HttpClient"/> instance using the configuration that corresponds
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<string> GetStringAsync(string clientName, string url)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            return client.GetStringAsync(url);
        }


        /// <summary>
        /// Creates and configures an <see cref="HttpClient"/> instance using the configuration that corresponds
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetAsync(string clientName, string url)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            return client.GetAsync(url);
        }
    }
}
