using Microsoft.AspNetCore.Mvc;
using Pokedex.Models;
using Pokedex.Services.Enum;
using Pokedex.Services.Interfaces;

namespace Pokedex.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonInfoAdapter _pokemonInfoAdapter;
        private readonly ITranslationAdapter _translationAdapter;

        public PokemonController(IPokemonInfoAdapter pokeAPIAdapter, ITranslationAdapter translationAdapter)
        {
            _pokemonInfoAdapter = pokeAPIAdapter;
            _translationAdapter = translationAdapter;
        }

        // GET: pokemon/{name}
        [HttpGet("{name?}")]
        public async Task<IActionResult> Get(string? name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest(new { Message = $"'{nameof(name)}' field is missing." });

                var pokemonDto = await _pokemonInfoAdapter.GetBasicPokemonInfoAsync(name);

                if (pokemonDto == null)
                    return NotFound(new { Message = $"'{name}' pokemon not found." });

                var pokemon = Map(pokemonDto);

                return Ok(pokemon);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Message = "An error occurred while processing your request." });
            }
        }

        // GET: pokemon/translated/{name}
        [HttpGet("translated/{name?}")]
        public async Task<IActionResult> GetTranslated(string? name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest(new { Message = $"'{nameof(name)}' field is missing." });

                var pokemonDto = await _pokemonInfoAdapter.GetBasicPokemonInfoAsync(name);

                if (pokemonDto == null)
                    return NotFound(new { Message = $"'{name}' pokemon not found." });

                var pokemon = Map(pokemonDto);

                var translation = (pokemon.Habitat == "cave" || pokemon.IsLegendary)
                    ? TranslationLanguage.Yoda
                    : TranslationLanguage.Shakespeare;

                try
                {
                    var translationResult = await _translationAdapter.GetTranslationAsync(translation, pokemon.Description);

                    if (translationResult.TranslationSuccessful)
                        pokemon.Description = translationResult.TranslatedText;
                }
                catch (Exception)
                {
                    //pokemon.Description remains unchanged
                }

                return Ok(pokemon);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Message = "An error occurred while processing your request." });
            }
        }

        private static Pokemon? Map(PokemonDto? pokemonDto)
        {
            if (pokemonDto == null)
                return null;

            return new Pokemon
            {
                Description = pokemonDto.Description,
                Habitat = pokemonDto.Habitat,
                IsLegendary = pokemonDto.IsLegendary,
                Name = pokemonDto.Name
            };
        }
    }
}
