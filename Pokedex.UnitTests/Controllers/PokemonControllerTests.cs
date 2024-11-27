using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Pokedex.Controllers;
using Pokedex.Models;
using Pokedex.Services;
using Pokedex.Services.Enum;
using Pokedex.Services.Interfaces;

namespace Pokedex.UnitTests.Controllers
{
    public class PokemonControllerTests
    {
        private IPokemonInfoAdapter _pokemonInfoAdapter;
        private ITranslationAdapter _translationAdapter;
        private PokemonController _controller;

        [SetUp]
        public void Setup()
        {
            _pokemonInfoAdapter = A.Fake<IPokemonInfoAdapter>();
            _translationAdapter = A.Fake<ITranslationAdapter>();

            _controller = new PokemonController(_pokemonInfoAdapter, _translationAdapter);
        }

        public void SetupGetBasicPokemonInfo(PokemonDto? pokemon)
        {
            A.CallTo(() => _pokemonInfoAdapter.GetBasicPokemonInfoAsync(A<string>.Ignored))
                .Returns(pokemon);
        }

        private static void AssertResultIs<T>(IActionResult result, int statusCode)
            where T : ObjectResult
        {
            var actionResult = result as T;
            Assert.IsNotNull(actionResult);
            Assert.That(actionResult.StatusCode, Is.EqualTo(statusCode));
        }

        [Test]
        public async Task Get_ShouldReturnBadRequest_IfNameIsEmpty()
        {
            var pokemonName = "";

            var result = await _controller.Get(pokemonName);

            AssertResultIs<BadRequestObjectResult>(result, 400);
        }

        [Test]
        public async Task Get_ShouldReturnNotFound_IfAMatchingPokemonIsNotFound()
        {
            SetupGetBasicPokemonInfo(null);

            var result = await _controller.Get("pokemon name");

            AssertResultIs<NotFoundObjectResult>(result, 404);
        }

        [Test]
        public async Task Get_ShouldReturnThePokemon_IfFound()
        {
            var expectedPokemon = new PokemonDto
            { 
                Name = "pokemon name",
                Description = "description",
                Habitat = "habitat",
                IsLegendary = true
            };

            SetupGetBasicPokemonInfo(expectedPokemon);

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
        public async Task Get_ShouldReturnServerError_IfThereIsAnException()
        {
            A.CallTo(() => _pokemonInfoAdapter.GetBasicPokemonInfoAsync(A<string>.Ignored))
                .Throws<Exception>();

            var result = await _controller.Get("pokemon name");

            AssertResultIs<ObjectResult>(result, 500);
        }

        [Test]
        public async Task GetTranslated_ShouldReturnBadRequest_IfNameIsEmpty()
        {
            var pokemonName = "";

            var result = await _controller.GetTranslated(pokemonName);

            AssertResultIs<BadRequestObjectResult>(result, 400);
        }

        [Test]
        public async Task GetTranslated_ShouldReturnNotFound_IfAMatchingPokemonIsNotFound()
        {
            SetupGetBasicPokemonInfo(null);

            var result = await _controller.GetTranslated("pokemon name");

            AssertResultIs<NotFoundObjectResult>(result, 404);
        }

        [TestCase("cave", false)]
        [TestCase("rare", true)]
        public async Task GetTranslated_ShouldReturnThePokemonWithYodaTranslationOnDescription(string habitat, bool isLegendary)
        {
            var expectedPokemon = new PokemonDto
            {
                Name = "pokemon name",
                Description = "description",
                Habitat = habitat,
                IsLegendary = isLegendary
            };

            SetupGetBasicPokemonInfo(expectedPokemon);

            var expectedDescription = "yoda description";

            A.CallTo(() => _translationAdapter.GetTranslationAsync(TranslationLanguage.Yoda, expectedPokemon.Description))
                .Returns(new TranslationResult
                { 
                    TranslationSuccessful = true,
                    TranslatedText = expectedDescription
                });

            var result = await _controller.GetTranslated(expectedPokemon.Name);

            AssertResultIs<OkObjectResult>(result, 200);

            var pokemonResult = ((OkObjectResult)result).Value as Pokemon;
            Assert.IsNotNull(pokemonResult);
            Assert.That(pokemonResult.Name, Is.EqualTo(expectedPokemon.Name));
            Assert.That(pokemonResult.Description, Is.EqualTo(expectedDescription));
            Assert.That(pokemonResult.Habitat, Is.EqualTo(expectedPokemon.Habitat));
            Assert.That(pokemonResult.IsLegendary, Is.EqualTo(expectedPokemon.IsLegendary));
        }

        [TestCase("anything but cave")]
        public async Task GetTranslated_ShouldReturnThePokemonWithShakespeareTranslationOnDescription(string habitat)
        {
            var expectedPokemon = new PokemonDto
            {
                Name = "pokemon name",
                Description = "description",
                Habitat = habitat,
                IsLegendary = false
            };

            SetupGetBasicPokemonInfo(expectedPokemon);

            var expectedDescription = "shakespeare description";

            A.CallTo(() => _translationAdapter.GetTranslationAsync(TranslationLanguage.Shakespeare, expectedPokemon.Description))
                .Returns(new TranslationResult
                {
                    TranslationSuccessful = true,
                    TranslatedText = expectedDescription
                });

            var result = await _controller.GetTranslated(expectedPokemon.Name);

            AssertResultIs<OkObjectResult>(result, 200);

            var pokemonResult = ((OkObjectResult)result).Value as Pokemon;
            Assert.IsNotNull(pokemonResult);
            Assert.That(pokemonResult.Name, Is.EqualTo(expectedPokemon.Name));
            Assert.That(pokemonResult.Description, Is.EqualTo(expectedDescription));
            Assert.That(pokemonResult.Habitat, Is.EqualTo(expectedPokemon.Habitat));
            Assert.That(pokemonResult.IsLegendary, Is.EqualTo(expectedPokemon.IsLegendary));
        }

        [Test]
        public async Task GetTranslated_ShouldReturnThePokemonWithStandardDescription_IfTranslationThrowsException()
        {
            var expectedPokemon = new PokemonDto
            {
                Name = "pokemon name",
                Description = "description",
                Habitat = "habitat",
                IsLegendary = false
            };

            SetupGetBasicPokemonInfo(expectedPokemon);

            A.CallTo(() => _translationAdapter.GetTranslationAsync(TranslationLanguage.Shakespeare, expectedPokemon.Description))
                .Throws<Exception>();

            var result = await _controller.GetTranslated(expectedPokemon.Name);

            AssertResultIs<OkObjectResult>(result, 200);

            var pokemonResult = ((OkObjectResult)result).Value as Pokemon;
            Assert.IsNotNull(pokemonResult);
            Assert.That(pokemonResult.Name, Is.EqualTo(expectedPokemon.Name));
            Assert.That(pokemonResult.Description, Is.EqualTo(expectedPokemon.Description));
            Assert.That(pokemonResult.Habitat, Is.EqualTo(expectedPokemon.Habitat));
            Assert.That(pokemonResult.IsLegendary, Is.EqualTo(expectedPokemon.IsLegendary));
        }

        [Test]
        public async Task GetTranslated_ShouldReturnThePokemonWithStandardDescription_IfTranslationIsNotPossible()
        {
            var expectedPokemon = new PokemonDto
            {
                Name = "pokemon name",
                Description = "description",
                Habitat = "habitat",
                IsLegendary = false
            };

            SetupGetBasicPokemonInfo(expectedPokemon);

            A.CallTo(() => _translationAdapter.GetTranslationAsync(TranslationLanguage.Shakespeare, expectedPokemon.Description))
                .Returns(new TranslationResult
                {
                    TranslationSuccessful = false
                });

            var result = await _controller.GetTranslated(expectedPokemon.Name);

            AssertResultIs<OkObjectResult>(result, 200);

            var pokemonResult = ((OkObjectResult)result).Value as Pokemon;
            Assert.IsNotNull(pokemonResult);
            Assert.That(pokemonResult.Name, Is.EqualTo(expectedPokemon.Name));
            Assert.That(pokemonResult.Description, Is.EqualTo(expectedPokemon.Description));
            Assert.That(pokemonResult.Habitat, Is.EqualTo(expectedPokemon.Habitat));
            Assert.That(pokemonResult.IsLegendary, Is.EqualTo(expectedPokemon.IsLegendary));
        }

        [Test]
        public async Task GetTranslated_ShouldReturnServerError_IfThereIsAnException()
        {
            A.CallTo(() => _pokemonInfoAdapter.GetBasicPokemonInfoAsync(A<string>.Ignored))
                .Throws<Exception>();

            var result = await _controller.GetTranslated("pokemon name");

            AssertResultIs<ObjectResult>(result, 500);
        }
    }
}
