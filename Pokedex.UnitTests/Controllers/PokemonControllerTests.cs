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
                .Returns(pokemon == null ? PokemonApiResult.Failure() : PokemonApiResult.Success(pokemon));
        }

        private static void AssertResultIs<T>(IActionResult result, int statusCode)
            where T : ObjectResult
        {
            var actionResult = result as T;
            Assert.IsNotNull(actionResult);
            Assert.That(actionResult.StatusCode, Is.EqualTo(statusCode));
        }

        private static PokemonDto CreatePokemonDto()
        {
            return new PokemonDto
            {
                Name = "pokemon name",
                Description = "description",
                Habitat = "habitat",
                IsLegendary = false
            };
        }

        public void SetupGetTranslation(TranslationLanguage language = TranslationLanguage.Shakespeare,
            string? sourceText = "", string translatedText = "", bool isSuccessful = true)
        {
            A.CallTo(() => _translationAdapter.GetTranslationAsync(language, sourceText))
                .Returns(isSuccessful ? TranslationApiResult.Success(translatedText) : TranslationApiResult.Failure());
        }

        public void AssertResultMatchesPokemon(IActionResult result, string? name, string? description, string? habitat, bool isLegendary)
        {
            AssertResultIs<OkObjectResult>(result, 200);

            var pokemonResult = ((OkObjectResult)result).Value as Pokemon;
            Assert.IsNotNull(pokemonResult);
            Assert.That(pokemonResult.Name, Is.EqualTo(name));
            Assert.That(pokemonResult.Description, Is.EqualTo(description));
            Assert.That(pokemonResult.Habitat, Is.EqualTo(habitat));
            Assert.That(pokemonResult.IsLegendary, Is.EqualTo(isLegendary));
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
            var expectedPokemon = CreatePokemonDto();

            SetupGetBasicPokemonInfo(expectedPokemon);

            var result = await _controller.Get(expectedPokemon.Name);

            AssertResultMatchesPokemon(result,
                name: expectedPokemon.Name,
                description: expectedPokemon.Description,
                habitat: expectedPokemon.Habitat,
                isLegendary: expectedPokemon.IsLegendary);
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
            var expectedPokemon = CreatePokemonDto();
            expectedPokemon.Habitat = habitat;
            expectedPokemon.IsLegendary = isLegendary;

            SetupGetBasicPokemonInfo(expectedPokemon);

            var expectedDescription = "yoda description";

            SetupGetTranslation(TranslationLanguage.Yoda, sourceText: expectedPokemon.Description, translatedText: expectedDescription);

            var result = await _controller.GetTranslated(expectedPokemon.Name);

            AssertResultMatchesPokemon(result,
                name: expectedPokemon.Name,
                description: expectedDescription,
                habitat: expectedPokemon.Habitat,
                isLegendary: expectedPokemon.IsLegendary);
        }

        [TestCase("anything but cave")]
        public async Task GetTranslated_ShouldReturnThePokemonWithShakespeareTranslationOnDescription(string habitat)
        {
            var expectedPokemon = CreatePokemonDto();
            expectedPokemon.Habitat = habitat;
            expectedPokemon.IsLegendary = false;

            SetupGetBasicPokemonInfo(expectedPokemon);

            var expectedDescription = "shakespeare description";

            SetupGetTranslation(TranslationLanguage.Shakespeare, sourceText: expectedPokemon.Description, translatedText: expectedDescription);

            var result = await _controller.GetTranslated(expectedPokemon.Name);

            AssertResultMatchesPokemon(result,
                name: expectedPokemon.Name,
                description: expectedDescription,
                habitat: expectedPokemon.Habitat,
                isLegendary: expectedPokemon.IsLegendary);
        }

        [Test]
        public async Task GetTranslated_ShouldReturnThePokemonWithStandardDescription_IfTranslationThrowsException()
        {
            var expectedPokemon = CreatePokemonDto();

            SetupGetBasicPokemonInfo(expectedPokemon);

            A.CallTo(() => _translationAdapter.GetTranslationAsync(TranslationLanguage.Shakespeare, expectedPokemon.Description))
                .Throws<Exception>();

            var result = await _controller.GetTranslated(expectedPokemon.Name);

            AssertResultMatchesPokemon(result,
                name: expectedPokemon.Name,
                description: expectedPokemon.Description,
                habitat: expectedPokemon.Habitat,
                isLegendary: expectedPokemon.IsLegendary);
        }

        [Test]
        public async Task GetTranslated_ShouldReturnThePokemonWithStandardDescription_IfTranslationIsNotPossible()
        {
            var expectedPokemon = CreatePokemonDto();

            SetupGetBasicPokemonInfo(expectedPokemon);

            SetupGetTranslation(isSuccessful: false);

            var result = await _controller.GetTranslated(expectedPokemon.Name);

            AssertResultMatchesPokemon(result,
                name: expectedPokemon.Name,
                description: expectedPokemon.Description,
                habitat: expectedPokemon.Habitat,
                isLegendary: expectedPokemon.IsLegendary);
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
