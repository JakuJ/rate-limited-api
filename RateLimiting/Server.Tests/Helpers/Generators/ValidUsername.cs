using FsCheck;

namespace Server.Tests.Generators
{
    public class ValidUsername
    {
        public static Arbitrary<string> Generate()
        {
            return Arb.Default.String().Filter(x => x is { Length: >= 6 and <= 64 } && !x.Contains(':'));
        }
    }
}
