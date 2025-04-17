using Microsoft.Extensions.Options;
using Moq;
using Sympli.Searching.Core.Entities;
using Sympli.Searching.Core.Interfaces;
using Sympli.Searching.Infrastructure.Providers;

namespace Sympli.Searching.Tests.Infrastructure.Providers
{
    [TestClass]
    public class BingSearchResultProviderTest
    {
        private Mock<IHttpClientWrapper> _httpClientMock;
        private Mock<IOptions<AppSettings>> _appSettingsMock;
        private Mock<ICacheService> _cacheServiceMock;
        private BingSearchResultProvider _bingSearchResultProvider;
        public BingSearchResultProviderTest()
        {
            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _appSettingsMock.Setup(o => o.Value)
                .Returns(new AppSettings
                {
                    BingSearch = new SearchSetting
                    {
                        Url = "https://api.bing.microsoft.com/v7.0/search",
                        Limit = 10 // Set the limit for the test
                    }
                });

            _cacheServiceMock = new Mock<ICacheService>();
            _httpClientMock = new Mock<IHttpClientWrapper>();

            _bingSearchResultProvider = new BingSearchResultProvider(
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
            string expectedResponse = "<a href=\"https://example.com\">Example</a>";
            // Mock the HttpClient response

            _cacheServiceMock.Setup(cache => cache.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);
            _cacheServiceMock.Setup(cache => cache.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Callback(() => { });

            _httpClientMock.Setup(client => client.GetStringAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _bingSearchResultProvider.GetRankPositionsAsync(keyword, targetUrl);
            // Assert
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public async Task GetRankPositionsAsync_ShouldReturnZero_WhenNoResultsFound()
        {
            // Arrange
            string keyword = "nonexistent keyword";
            string targetUrl = "https://nonexistent.com";
            string expectedResponse = "<a href=\"https://example.com\">Example</a>";
            // Mock the HttpClient response
            _cacheServiceMock.Setup(cache => cache.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);
            _cacheServiceMock.Setup(cache => cache.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Callback(() => { });

            _httpClientMock.Setup(client => client.GetStringAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedResponse);
            // Act
            var result = await _bingSearchResultProvider.GetRankPositionsAsync(keyword, targetUrl);
            // Assert
            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public async Task GetRankPositionsAsync_ShouldReturnCachedResult_WhenCacheIsAvailable()
        {
            // Arrange
            string keyword = "cached keyword";
            string targetUrl = "https://cached.com";
            string cachedResult = "1, 2, 3";
            _cacheServiceMock.Setup(cache => cache.TryGetValue(It.IsAny<string>(), out cachedResult))
                .Returns(true);
            // Act
            var result = await _bingSearchResultProvider.GetRankPositionsAsync(keyword, targetUrl);
            // Assert
            Assert.AreEqual(cachedResult, result);
        }

    }
}
