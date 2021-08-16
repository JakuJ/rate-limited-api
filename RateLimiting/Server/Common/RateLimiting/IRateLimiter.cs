namespace Server.Common.RateLimiting
{
    public interface IRateLimiter
    {
        (bool, int) CheckQuota(int user, int usage);
        (int, int) GetUserQuota(int user);
    }
}