namespace Pokedex.Models
{
    public class PokemonDto
    {
        public string? Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public string? Habitat { get; set; } = string.Empty;
        
        public bool HabitatIsCave => Habitat?.ToLower() == "cave";

        public bool IsLegendary { get; set; }
    }
}
