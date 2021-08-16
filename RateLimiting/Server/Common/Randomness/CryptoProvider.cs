using System;
using System.Security.Cryptography;

namespace Server.Common.Randomness
{
    /// <summary>
    /// Generates random bytes using the <see cref="RandomNumberGenerator"/> class.
    /// </summary>
    public class CryptoProvider : IRandomnessSource, IDisposable
    {
        private readonly RandomNumberGenerator generator = RandomNumberGenerator.Create();

        /// <inheritdoc/>
        public byte[] GenerateRandomness(int length)
        {
            var result = new byte[length];
            generator.GetBytes(result);
            return result;
        }

        /// <inheritdoc/>
        public void Dispose() => generator.Dispose();
    }
}
