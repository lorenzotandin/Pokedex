using Pokedex.Models;

namespace Pokedex.Services
{
    public class PokeAPIAdapter : IPokemonInfoAdapter
    {
        string POKE_API_BASE_URL = "https://pokeapi.co/";

        public PokeAPIAdapter()
        {
        }

        public async Task<Pokemon?> GetBasicPokemonInfoAsync(string pokemonName)
        {
            //var pokemonInfoUrl = $"{POKE_API_BASE_URL}{pokemonName}";

            return new Pokemon
            {
                Name = pokemonName,
                Description = "description",
                Habitat = "habitat",
                IsLegendary = true
            };
        }
    }
}
