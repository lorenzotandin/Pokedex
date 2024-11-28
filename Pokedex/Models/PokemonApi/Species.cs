using System.Text.Json.Serialization;

namespace Pokedex.Models.PokemonApi
{
    public class Species
    {
        [JsonPropertyName("flavor_text_entries")]
        public IEnumerable<FlavorTextEntry>? FlavorTextEntries { get; set; }

        [JsonPropertyName("habitat")]
        public NameAndUrl? Habitat { get; set; }

        [JsonPropertyName("is_legendary")]
        public bool IsLegendary { get; set; }
    }
}
