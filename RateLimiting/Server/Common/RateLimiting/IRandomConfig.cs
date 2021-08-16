namespace Server.Common.RateLimiting
{
    public interface IRandomConfig
    {
        int DefaultSize { get; }
        int DefaultLimit { get; }
        int DefaultWindow { get; }
    }
}