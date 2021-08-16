using FsCheck;

namespace Server.Tests.Generators
{
    public static class InvalidPassword
    {
        public static Arbitrary<string> Generate() =>
            Arb.Default
                .NonEmptyString()
                .Filter(x => x.Get is {Length: < 8 or > 64} || x.Get.Contains(':'))
                .Convert(x => x.Get, NonEmptyString.NewNonEmptyString);
    }
}