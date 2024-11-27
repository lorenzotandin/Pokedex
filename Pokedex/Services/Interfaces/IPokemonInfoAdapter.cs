using Pokedex.Models;

namespace Pokedex.Services.Interfaces
{
    public interface IPokemonInfoAdapter
    {
        Task<PokemonDto?> GetBasicPokemonInfoAsync(string pokemonName);
    }
}