using FsCheck;

namespace Server.Tests.Generators
{
    public static class InvalidPassword
    {
        public static Arbitrary<string> Generate() =>
            Arb.Default.String().Filter(x => x is {Length: < 8 or > 64});
    }
}