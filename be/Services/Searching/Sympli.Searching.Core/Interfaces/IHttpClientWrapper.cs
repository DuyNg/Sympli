namespace Sympli.Searching.Core.Interfaces
{
    public interface IHttpClientWrapper
    {
        Task<string> GetStringAsync(string clientName, string url);
        Task<HttpResponseMessage> GetAsync(string clientName, string url);
    }
}
