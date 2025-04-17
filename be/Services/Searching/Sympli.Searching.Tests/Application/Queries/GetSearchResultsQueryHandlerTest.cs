using Moq;
using Sympli.Searching.Application.Queries;
using Sympli.Searching.Core.Enums;
using Sympli.Searching.Core.Interfaces;

namespace Sympli.Searching.Tests.Application.Queries
{
    [TestClass]
    public class GetSearchResultsQueryHandlerTests
    {
        private Mock<ISearchProviderFactory> _searchProviderFactoryMock;
        private Mock<ISearchResultProvider> _searchResultProviderMock;
        private GetSearchResultsQueryHandler _handler;

        public GetSearchResultsQueryHandlerTests()
        {
            Setup();
        }

        [TestInitialize]
        public void Setup()
        {
            _searchProviderFactoryMock = new Mock<ISearchProviderFactory>();
            _searchResultProviderMock = new Mock<ISearchResultProvider>();
            _handler = new GetSearchResultsQueryHandler(_searchProviderFactoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnSearchResults_WhenQueryIsValid()
        {
            // Arrange
            var query = new GetSearchResultsQuery(
                "test",
                "https://example.com",
                SearchEngineEnum.Google
            );

            var expectedResults = "1, 2, 3";

            _searchResultProviderMock
                .Setup(provider => provider.GetRankPositionsAsync(query.Keyword, query.Url))
                .ReturnsAsync("1, 2, 3"); // Fix: Ensure the mock returns a string as per the ISearchResultProvider interface.

            _searchProviderFactoryMock
                .Setup(factory => factory.GetProvider(query.Engine))
                .Returns(_searchResultProviderMock.Object);

            
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResults, result.Positions);
            Assert.AreEqual("Google", result.Browser); // Assuming EnumHelper maps 'Google' to 'Google'.
        }

        [TestMethod]
        public async Task Handle_ShouldThrowException_WhenProviderThrowsError()
        {
            // Arrange
            var query = new GetSearchResultsQuery(
                "test",
                "https://example.com",
                SearchEngineEnum.Google
            );

            _searchResultProviderMock
                .Setup(provider => provider.GetRankPositionsAsync(query.Keyword, query.Url))
                .ThrowsAsync(new Exception("Provider error"));

            _searchProviderFactoryMock
                .Setup(factory => factory.GetProvider(query.Engine))
                .Returns(_searchResultProviderMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            Assert.AreEqual("Provider error", exception.Message);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnEmptyPositions_WhenNoResultsFound()
        {
            // Arrange
            var query = new GetSearchResultsQuery(
                "nonexistent",
                "https://example.com",
                SearchEngineEnum.Google
            );

            var expectedResults = "0"; // No results found
            _searchResultProviderMock
                .Setup(provider => provider.GetRankPositionsAsync(query.Keyword, query.Url))
                .ReturnsAsync(expectedResults);

            _searchProviderFactoryMock
                .Setup(factory => factory.GetProvider(query.Engine))
                .Returns(_searchResultProviderMock.Object);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("0", result.Positions);
        }

        [TestMethod]
        public async Task Handle_ShouldThrowNullReferenceException_WhenKeywordIsNull()
        {
            // Arrange
            var query = new GetSearchResultsQuery(
                null,
                "https://example.com",
                SearchEngineEnum.Google
            );

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [TestMethod]
        public async Task Handle_ShouldThrowArgumentException_WhenEngineIsUnsupported()
        {
            // Arrange
            var query = new GetSearchResultsQuery(
                "test",
                "https://example.com",
                (SearchEngineEnum)999 // Invalid engine value
            );

            _searchProviderFactoryMock
                .Setup(factory => factory.GetProvider(query.Engine))
                .Throws(new ArgumentException("Unsupported search engine"));

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.AreEqual("Unsupported search engine", exception.Message);
        }
    }
}