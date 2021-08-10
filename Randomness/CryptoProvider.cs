using System;
using System.Security.Cryptography;

namespace RateLimiting.Randomness
{
    public class CryptoProvider : IRandomGenerator
    {
        private readonly RandomNumberGenerator generator = RandomNumberGenerator.Create();

        public byte[] GenerateRandomness(int length)
        {
            var result = new byte[length];
            generator.GetBytes(result);
            return result;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(generator);
            generator.Dispose();
        }
    }
}