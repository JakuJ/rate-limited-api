namespace Server.Common.Randomness
{
    public interface IRandomnessSource
    {
        byte[] GenerateRandomness(int length);
    }
}