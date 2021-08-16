using FsCheck;

namespace Server.Tests.Generators
{
    public class ValidLen
    {
        public static Arbitrary<int> Generate() =>
            Arb.Default.Int32().Filter(x => x >= 32);
    }
}