using System.Text.Json.Serialization;

namespace Pokedex.Models.PokemonApi
{
    public class FlavorTextEntry
    {
        [JsonPropertyName("flavor_text")]
        public string? FlavorText { get; set; }

        [JsonPropertyName("language")]
        public NameAndUrl? Language { get; set; }
    }
}
