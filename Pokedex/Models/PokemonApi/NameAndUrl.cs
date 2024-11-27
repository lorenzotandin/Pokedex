using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Pokedex.Models.PokemonApi
{
    [DebuggerDisplay("{Name}")]
    public class NameAndUrl
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
