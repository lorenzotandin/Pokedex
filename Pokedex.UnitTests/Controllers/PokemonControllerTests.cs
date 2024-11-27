using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Pokedex.Controllers;
using Pokedex.Models;
using Pokedex.Services;

namespace Pokedex.UnitTests.Controllers
{
    public class PokemonControllerTests
    {
        private IPokemonInfoAdapter _pokemonInfoAdapter;
        private PokemonController _controller;

        [SetUp]
        public void Setup()
        {
            _pokemonInfoAdapter = A.Fake<IPokemonInfoAdapter>();

            _controller = new PokemonController(_pokemonInfoAdapter);
        }

        [Test]
        public async Task ShouldReturnThePokemon_IfFound()
        {
            var expectedPokemon = new Pokemon
            { 
                Name = "pokemon name",
                Description = "description",
                Habitat = "habitat",
                IsLegendary = true
            };

            A.CallTo(() => _pokemonInfoAdapter.GetBasicPokemonInfoAsync(expectedPokemon.Name))
                .Returns(expectedPokemon);

            var result = await _controller.Get(expectedPokemon.Name);

            AssertResultIs<OkObjectResult>(result, 200);

            var pokemonResult = ((OkObjectResult)result).Value as Pokemon;
            Assert.IsNotNull(pokemonResult);
            Assert.That(pokemonResult.Name, Is.EqualTo(expectedPokemon.Name));
            Assert.That(pokemonResult.Description, Is.EqualTo(expectedPokemon.Description));
            Assert.That(pokemonResult.Habitat, Is.EqualTo(expectedPokemon.Habitat));
            Assert.That(pokemonResult.IsLegendary, Is.EqualTo(expectedPokemon.IsLegendary));
        }

        [Test]
        public async Task ShouldReturnBadRequest_IfNameIsEmpty()
        {
            var pokemonName = "";

            var result = await _controller.Get(pokemonName);

            AssertResultIs<BadRequestObjectResult>(result, 400);
        }

        [Test]
        public async Task ShouldReturnServerError_IfThereIsAnException()
        {
            A.CallTo(() => _pokemonInfoAdapter.GetBasicPokemonInfoAsync(A<string>.Ignored))
                .Throws<Exception>();

            var result = await _controller.Get("pokemon name");

            AssertResultIs<ObjectResult>(result, 500);
        }

        [Test]
        public async Task ShouldReturnNotFound_IfAMatchingPokemonIsNotFound()
        {
            A.CallTo(() => _pokemonInfoAdapter.GetBasicPokemonInfoAsync(A<string>.Ignored))
                .Returns((Pokemon?)null);

            var result = await _controller.Get("pokemon name");

            AssertResultIs<NotFoundObjectResult>(result, 404);
        }

        private static void AssertResultIs<T>(IActionResult result, int statusCode)
            where T : ObjectResult
        {
            var actionResult = result as T;
            Assert.IsNotNull(actionResult);
            Assert.That(actionResult.StatusCode, Is.EqualTo(statusCode));
        }
    }
}
