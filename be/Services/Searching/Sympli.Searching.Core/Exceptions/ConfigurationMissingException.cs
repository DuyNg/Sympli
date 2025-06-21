namespace Sympli.Searching.Core.Exceptions
{
    public class ConfigurationMissingException : Exception
    {
        public ConfigurationMissingException(string key) : base($"Configuration missing for key: {key}") { }
    }

    public class SearchProviderException : Exception
    {
        public SearchProviderException(string message) : base(message) { }
    }
}