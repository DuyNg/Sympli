using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sympli.Searching.Core.Entities
{
    /// <summary>
    /// Represents a search query including the keyword and the date it was created.
    /// </summary>
    public class SearchQuery
    {
        public string KeyWord { get; set; }
        public DateTime CreatedAt { get; set; }  = DateTime.UtcNow;
        public SearchQuery(string keyWord) 
        {
            KeyWord = keyWord;
        }
    }
}
