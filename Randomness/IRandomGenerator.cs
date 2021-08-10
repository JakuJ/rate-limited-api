using System;

namespace RateLimiting.Randomness
{
    public interface IRandomGenerator : IDisposable
    {
        byte[] GenerateRandomness(int length);
    }
}