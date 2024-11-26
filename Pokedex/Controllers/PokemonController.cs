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
        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "field is required" });

            var pokemon = await _pokemonInfoAdapter.GetBasicPokemonInfoAsync(name);

            return Ok(pokemon);
        }
    }
}
