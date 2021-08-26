namespace Server.Internal.Randomness
{
    /// <summary>
    /// Represents a source of random bytes for the <b>/random</b> endpoint.
    /// </summary>
    public interface IRandomnessSource
    {
        /// <summary>
        /// Generate an array of random bytes.
        /// </summary>
        /// <param name="length">Number of bytes to generate.</param>
        /// <returns>An array of random bytes.</returns>
        byte[] GenerateRandomness(int length);
    }
}
