using Microsoft.AspNetCore.Mvc;
using Pokedex.Services;

namespace Pokedex.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonInfoAdapter _pokemonInfoAdapter;

        public PokemonController(IPokemonInfoAdapter pokeAPIAdapter)
        {
            _pokemonInfoAdapter = pokeAPIAdapter;
        }

        // GET: pokemon/{name}
        [HttpGet("{name?}")]
        public async Task<IActionResult> Get(string? name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest(new { Message = $"'{nameof(name)}' field is missing." });

                var pokemon = await _pokemonInfoAdapter.GetBasicPokemonInfoAsync(name);

                if (pokemon == null)
                    return NotFound(new { Message = $"'{name}' pokemon not found." });

                return Ok(pokemon);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Message = "An error occurred while processing your request." });
            }
        }
    }
}
