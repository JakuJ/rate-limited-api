using System.Text.Json.Serialization;

namespace Server.Models.Random
{
    /// <summary>
    /// A body of the response from the <b>/random</b> endpoint.
    /// </summary>
    public record ResponseBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseBody"/> class.
        /// </summary>
        /// <param name="random">Base64-encoded array of random bytes.</param>
        public ResponseBody(string random) => Random = random;

        /// <summary>
        /// Gets the base64-encoded array of random bytes.
        /// </summary>
        [JsonPropertyName("random")]
        public string Random { get; init; }
    }
}
