namespace Server.Models.Random
{
    public class ResponseBody
    {
        public string random { get; }
        public ResponseBody(string random) => this.random = random;
    }
}