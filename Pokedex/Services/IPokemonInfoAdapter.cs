using Pokedex.Models;

namespace Pokedex.Services
{
    public interface IPokemonInfoAdapter
    {
        Task<Pokemon> GetBasicPokemonInfoAsync(string pokemonName);
    }
}