using System;
using System.Security.Cryptography;

namespace Server.Common.Randomness
{
    public class CryptoProvider : IRandomnessSource, IDisposable
    {
        private readonly RandomNumberGenerator generator = RandomNumberGenerator.Create();

        public byte[] GenerateRandomness(int length)
        {
            var result = new byte[length];
            generator.GetBytes(result);
            return result;
        }

        public void Dispose() => generator.Dispose();
    }
}