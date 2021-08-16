using FsCheck;

namespace Server.Tests.Generators
{
    public static class InvalidUsername
    {
        public static Arbitrary<string> Generate()
        {
            return Arb.Default
                .NonEmptyString()
                .Filter(x => x.Get is { Length: < 6 or > 64 } || x.Get.Contains(':'))
                .Convert(x => x.Get, NonEmptyString.NewNonEmptyString);
        }
    }
}
