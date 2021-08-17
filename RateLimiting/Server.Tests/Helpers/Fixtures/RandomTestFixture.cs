namespace Server.Tests.Helpers.Fixtures
{
    public class RandomTestFixture
    {
        private static int uniqueId;

        public int UniqueId => uniqueId++;
    }
}
