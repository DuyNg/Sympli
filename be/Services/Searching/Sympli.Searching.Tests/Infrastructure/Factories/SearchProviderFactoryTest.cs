using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Sympli.Searching.Core.Entities;
using Sympli.Searching.Core.Enums;
using Sympli.Searching.Core.Interfaces;
using Sympli.Searching.Infrastructure.Factories;
using Sympli.Searching.Infrastructure.Providers;

namespace Sympli.Searching.Tests.Infrastructure.Factories
{
    [TestClass]
    public class SearchProviderFactoryTest
    {
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<IHttpClientWrapper> _httpClientMock;
        private Mock<IOptions<AppSettings>> _appSettingsMock;
        private Mock<ICacheService> _cacheServiceMock;
        private SearchProviderFactory _searchProviderFactory;

        private BingSearchResultProvider _bingSearchResultProvider;
        private GoogleSearchResultProvider _googleSearchResultProvider;

        public SearchProviderFactoryTest()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _httpClientMock = new Mock<IHttpClientWrapper>();
            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _cacheServiceMock = new Mock<ICacheService>();
            _searchProviderFactory = new SearchProviderFactory(_serviceProviderMock.Object);

            _appSettingsMock.Setup(o => o.Value)
                .Returns(new AppSettings
                {
                    BingSearch = new SearchSetting(),
                    GoogleSearch = new SearchSetting()
                });

            _bingSearchResultProvider = new BingSearchResultProvider(
                _httpClientMock.Object,
                _appSettingsMock.Object,
                _cacheServiceMock.Object
            );

            _googleSearchResultProvider = new GoogleSearchResultProvider(
                _httpClientMock.Object,
                _appSettingsMock.Object,
                _cacheServiceMock.Object
            );
        }

        [TestMethod]
        public void GetProvider_ShouldReturnBing_WhenBingIsRequested()
        {
            // Arrange  
            var expectedEngine = SearchEngineEnum.Bing;

            _serviceProviderMock
                .Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(_bingSearchResultProvider);
            // Act  
            var result = _searchProviderFactory.GetProvider(expectedEngine);
            // Assert  
            Assert.IsInstanceOfType(result, typeof(BingSearchResultProvider));
        }

        [TestMethod]
        public void GetProvider_ShouldReturnGoogle_WhenGoogleIsRequested()
        {
            // Arrange  
            var expectedEngine = SearchEngineEnum.Google;

            _serviceProviderMock
                .Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(_googleSearchResultProvider);

            // Act  
            var result = _searchProviderFactory.GetProvider(expectedEngine);

            // Assert  
            Assert.IsInstanceOfType(result, typeof(GoogleSearchResultProvider));
        }

        [TestMethod]
        public void GetProvider_ShouldThrowException_WhenInvalidEngineIsRequested()
        {
            // Arrange  
            var expectedEngine = (SearchEngineEnum)999;

            _serviceProviderMock
                .Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(_googleSearchResultProvider);

            // Act & Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                _searchProviderFactory.GetProvider(expectedEngine));
        }
    }
}
