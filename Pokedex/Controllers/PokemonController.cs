using Microsoft.AspNetCore.Mvc;

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
            return Ok(name);
        }
    }
}
