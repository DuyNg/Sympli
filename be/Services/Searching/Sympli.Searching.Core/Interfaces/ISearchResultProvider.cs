using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sympli.Searching.Core.Interfaces
{
    public interface ISearchResultProvider
    {
        Task<string> GetRankPositionsAsync(string keyword, string targetUrl);
    }
}
