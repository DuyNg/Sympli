using Microsoft.Extensions.Options;
using Moq;
using Sympli.Searching.Core.Entities;
using Sympli.Searching.Core.Interfaces;
using Sympli.Searching.Infrastructure.Providers;

namespace Sympli.Searching.Tests.Infrastructure.Providers
{
    [TestClass]
    public class GoogleSearchResultProviderTest
    {
        private Mock<IHttpClientWrapper> _httpClientMock;
        private Mock<IOptions<AppSettings>> _appSettingsMock;
        private Mock<ICacheService> _cacheServiceMock;
        private GoogleSearchResultProvider _googleSearchResultProvider;
        public GoogleSearchResultProviderTest()
        {
            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _appSettingsMock.Setup(o => o.Value)
                .Returns(new AppSettings
                {
                    GoogleSearch = new SearchSetting
                    {
                        Url = "https://api.google.com/search",
                        Limit = 10 // Set the limit for the test
                    }
                });
            _cacheServiceMock = new Mock<ICacheService>();
            _httpClientMock = new Mock<IHttpClientWrapper>();
            _googleSearchResultProvider = new GoogleSearchResultProvider(
                _httpClientMock.Object,
                _appSettingsMock.Object,
                _cacheServiceMock.Object
            );
        }

        [TestMethod]
        public async Task GetRankPositionsAsync_ShouldReturnRankPositions_WhenValidKeywordAndUrlProvided()
        {
            // Arrange
            string keyword = "test keyword";
            string targetUrl = "https://example.com";
            var expectedResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("<a href=\"https://example.com\">Example</a>")
            }; 

            // Mock the HttpClient response
            _cacheServiceMock.Setup(cache => cache.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);
            _cacheServiceMock.Setup(cache => cache.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Callback(() => { });
            _httpClientMock.Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedResponse);
            // Act
            var result = await _googleSearchResultProvider.GetRankPositionsAsync(keyword, targetUrl);
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetRankPositionsAsync_ShouldReturnZero_WhenNoMatchesFound()
        {
            // Arrange
            string keyword = "test keyword";
            string targetUrl = "https://abcexample.com";
            var expectedResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("<a href=\"https://anotherexample.com\">Another Example</a>")
            };
            // Mock the HttpClient response
            _cacheServiceMock.Setup(cache => cache.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);
            _cacheServiceMock.Setup(cache => cache.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Callback(() => { });
            _httpClientMock.Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedResponse);
            // Act
            var result = await _googleSearchResultProvider.GetRankPositionsAsync(keyword, targetUrl);
            // Assert
            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public async Task GetRankPositionsAsync_ShouldReturnCachedResult_WhenCacheIsAvailable()
        {
            // Arrange
            string keyword = "test keyword";
            string targetUrl = "https://example.com";
            string cachedResult = "1, 2, 3";
            string cacheKey = $"google:{keyword}:{targetUrl}".ToLowerInvariant();
            _cacheServiceMock.Setup(cache => cache.TryGetValue(cacheKey, out cachedResult))
                .Returns(true);
            // Act
            var result = await _googleSearchResultProvider.GetRankPositionsAsync(keyword, targetUrl);
            // Assert
            Assert.AreEqual(cachedResult, result);
        }
    }
}
