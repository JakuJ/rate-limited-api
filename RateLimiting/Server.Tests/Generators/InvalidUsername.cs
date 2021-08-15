using FsCheck;

namespace Server.Tests.Generators
{
    public static class InvalidUsername
    {
        public static Arbitrary<string> Generate() =>
            Arb.Default.String().Filter(x => x is {Length: < 6 or > 64});
    }
}