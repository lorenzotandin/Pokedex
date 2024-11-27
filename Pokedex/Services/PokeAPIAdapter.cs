using Pokedex.Models;
using Pokedex.Services.Interfaces;
using System.Text.Json;
using PokemonApi = Pokedex.Models.PokemonApi;

namespace Pokedex.Services
{
    public class PokeAPIAdapter : IPokemonInfoAdapter
    {
        const string POKE_API_BASE_URL = "https://pokeapi.co/api/v2/pokemon/";

        private readonly HttpClient _httpClient;

        private JsonSerializerOptions _jsonSerializerOptions;

        public PokeAPIAdapter(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<PokemonDto?> GetBasicPokemonInfoAsync(string? pokemonName)
        {
            if (string.IsNullOrEmpty(pokemonName))
                return null;

            var pokemonInfoUrl = $"{POKE_API_BASE_URL}{pokemonName}";

            var response = await _httpClient.GetAsync(pokemonInfoUrl);

            if (!response.IsSuccessStatusCode)
                return null;

            var pokemon = await DeserializeContentAsync<PokemonApi.Pokemon>(response);
            
            var speciesResponse = await _httpClient.GetAsync(pokemon!.Species!.Url);

            if (!speciesResponse.IsSuccessStatusCode)
                return null;

            var species = await DeserializeContentAsync<PokemonApi.Species>(speciesResponse);

            var description = species.FlavorTextEntries
                .Where(e => e.Language?.Name == "en")
                .FirstOrDefault()
                ?.FlavorText
                ?.Replace("\n", " ")
                ?.Replace("\f", " ");

            return new PokemonDto
            {
                Name = pokemon.Name,
                Description = description,
                Habitat = species.Habitat.Name,
                IsLegendary = species.IsLegendary
            };
        }

        private async Task<T> DeserializeContentAsync<T>(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(responseContent, _jsonSerializerOptions)!;
        }
    }
}
