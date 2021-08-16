namespace Server.Common.RateLimiting
{
    public interface IRateLimiter
    {
        (bool success, int remaining) CheckQuota(int user, int usage);
        (int bytes, int seconds) GetUserLimit(int user);
    }
}