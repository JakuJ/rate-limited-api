using FsCheck;

namespace Server.Tests.Generators
{
    public class ValidPassword
    {
        public static Arbitrary<string> Generate() =>
            Arb.Default.String().Filter(x => x is {Length: >= 8 and <= 64} && !x.Contains(':'));
    }
}