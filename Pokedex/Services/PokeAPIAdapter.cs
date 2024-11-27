using Pokedex.Models;
using System.Text.Json;
using PokemonApi = Pokedex.Models.PokemonApi;

namespace Pokedex.Services
{
    public class PokeAPIAdapter : IPokemonInfoAdapter
    {
        string POKE_API_BASE_URL = "https://pokeapi.co/api/v2/pokemon/";

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

        public async Task<Pokemon?> GetBasicPokemonInfoAsync(string? pokemonName)
        {
            if (string.IsNullOrEmpty(pokemonName))
                return null;

            var pokemonInfoUrl = $"{POKE_API_BASE_URL}{pokemonName}";

            var response = await _httpClient.GetAsync(pokemonInfoUrl);

            if (!response.IsSuccessStatusCode)
                return null;

            var pokemonResponse = await DeserializeContentAsync<PokemonApi.Pokemon>(response);

            //TODO: call to species api

            return new Pokemon
            {
                Name = pokemonResponse.Name,
                Description = "description",
                Habitat = "habitat",
                IsLegendary = true
            };
        }

        private async Task<T?> DeserializeContentAsync<T>(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(responseContent, _jsonSerializerOptions);
        }
    }
}
