using Pokedex.Models;

namespace Pokedex.Services.Interfaces
{
    public interface IPokemonInfoAdapter
    {
        Task<PokemonApiResult> GetBasicPokemonInfoAsync(string pokemonName);
    }
}