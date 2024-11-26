using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pokedex.Controllers;
using Pokedex.Models;
using Pokedex.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var pokemonResult = okResult.Value as Pokemon;
            Assert.IsNotNull(pokemonResult);
            Assert.AreEqual(expectedPokemon.Name, pokemonResult.Name);
            Assert.AreEqual(expectedPokemon.Description, pokemonResult.Description);
            Assert.AreEqual(expectedPokemon.Habitat, pokemonResult.Habitat);
            Assert.AreEqual(expectedPokemon.IsLegendary, pokemonResult.IsLegendary);
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
