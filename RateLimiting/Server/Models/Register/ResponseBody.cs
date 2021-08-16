using System.Text.Json.Serialization;

namespace Server.Models.Register
{
    /// <summary>
    /// Body of a successful response from the <b>/register</b> endpoint.
    /// </summary>
    public record ResponseBody
    {
        /// <summary>
        /// Gets the ID of the registered client.
        /// </summary>
        [JsonPropertyName("clientID")]
        public int Id { get; init; }
    }
}
