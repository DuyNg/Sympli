namespace Sympli.Searching.Core.Utilities
{
    public static class ParseUrlHelper
    {
        public static string NormalizeUrl(string url)
        {
            return url.Replace("https://", "")
                      .Replace("http://", "")
                      .TrimEnd('/')
                      .ToLowerInvariant();
        }

    }
}
