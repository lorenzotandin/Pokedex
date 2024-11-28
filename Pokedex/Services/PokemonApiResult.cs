using Pokedex.Models;

namespace Pokedex.Services
{
    public class PokemonApiResult
    {
        private PokemonApiResult()
        {
            IsSuccessful = false;
        }

        public static PokemonApiResult Failure()
        {
            return new PokemonApiResult();
        }

        public static PokemonApiResult Success(PokemonDto? pokemon)
        {
            return new PokemonApiResult
            {
                Pokemon = pokemon,
                IsSuccessful = true
            };
        }

        public PokemonDto? Pokemon { get; set; }

        public bool IsSuccessful { get; set; }
    }
}
