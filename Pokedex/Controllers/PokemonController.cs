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

            return Ok(name);
        }
    }
}
