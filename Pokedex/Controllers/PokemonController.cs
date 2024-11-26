using Microsoft.AspNetCore.Mvc;
using Pokedex.Models;

namespace Pokedex.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        // GET: pokemon/{name}
        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Pokemon name is required");

            var pokemon = new Pokemon
            {
                Name = name,
                Description = "description",
                Habitat = "habitat",
                IsLegendary = true
            };

            return Ok(pokemon);
        }
    }
}
