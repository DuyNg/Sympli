namespace Sympli.Searching.Core.Entities
{

    public class AppSettings
    {
        public SearchSetting GoogleSearch { get; set; }
        public SearchSetting BingSearch { get; set; }

        public int CacheingExpireation { get; set; } = 60; // in munutes
    }

    public class SearchSetting
    {
        public string Url { get; set; }
        public int Limit { get; set; }
    }
}
