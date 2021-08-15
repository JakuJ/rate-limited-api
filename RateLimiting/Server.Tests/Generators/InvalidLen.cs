using FsCheck;

namespace Server.Tests.Generators
{
    public class InvalidLen
    {
        public static Arbitrary<int> Generate() =>
            Arb.Default.Int32().Filter(x => x is < 32 or > 1024);
    }
}