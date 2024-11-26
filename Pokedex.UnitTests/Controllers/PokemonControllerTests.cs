using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pokedex.Controllers;
using Pokedex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.UnitTests.Controllers
{
    public class PokemonControllerTests
    {
        private PokemonController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new PokemonController();
        }

        [Test]
        public async Task ShouldReturnTheGivenName()
        {
            var pokemonName = "pokemon name";

            var result = await _controller.Get(pokemonName);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var pokemonResult = okResult.Value as Pokemon;
            Assert.IsNotNull(pokemonResult);
            Assert.That(pokemonResult.Name, Is.EqualTo(pokemonName));
        }

        [Test]
        public async Task ShouldReturnBadRequest_IfNameIsEmpty()
        {
            var pokemonName = "";

            var result = await _controller.Get(pokemonName);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }
    }
}
