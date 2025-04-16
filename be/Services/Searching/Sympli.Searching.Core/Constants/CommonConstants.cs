using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sympli.Searching.Core.Constants
{
    public static class CommonConstants
    {
        public const string GoogleSearchClient = "GoogleSearchClient";
        public const string BingSearchClient = "BingSearchClient";
        public const string SearchEngineNotFoundMessage = "Search engine not found.";

        public const string ExtractLinkGoogleResult = @"<a\s+[^>]*jsname=""[^""]*""[^>]*ping=""[^""]*""[^>]*>(.*?)<\/a>";
        public const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";
    }
}
