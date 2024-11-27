using FakeItEasy;
using Pokedex.Services;
using System.Net;

namespace Pokedex.UnitTests.Services
{
    public class PokeAPIAdapterTests
    {
        private FakeableHttpMessageHandler _httpMessageHandler;
        private PokeAPIAdapter _pokeAPIAdapter;

        [SetUp]
        public void Setup()
        {
            _httpMessageHandler = A.Fake<FakeableHttpMessageHandler>();

            var httpClient = new HttpClient(_httpMessageHandler);

            _pokeAPIAdapter = new PokeAPIAdapter(httpClient);
        }

        [TearDown]
        public void TearDown()
        {
            _httpMessageHandler.Dispose();
        }

        private void SetupHttpClient(string? content = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode
            };

            if (!string.IsNullOrEmpty(content))
                response.Content = new StringContent(content);

            A.CallTo(() => _httpMessageHandler.FakeSendAsync(
                A<HttpRequestMessage>.Ignored, A<CancellationToken>.Ignored))
                .Returns(response);
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task ShouldReturnNull_IfThePokemonNameIsNullOrEmpty(string? pokemonName)
        {
            var pokemon = await _pokeAPIAdapter.GetBasicPokemonInfoAsync(pokemonName);

            Assert.That(pokemon, Is.Null);
        }

        [Test]
        public async Task ShouldReturnNull_IfTheApisResponseIsNotSuccessful()
        {
            SetupHttpClient(statusCode: HttpStatusCode.NotFound);

            var pokemon = await _pokeAPIAdapter.GetBasicPokemonInfoAsync("pokemonName");

            Assert.That(pokemon, Is.Null);
        }

        [Test]
        public async Task ShouldReturnThePokemon_IfFound()
        {
            var pokemonName = "mewtwo";

            var content = @"{
                ""name"": """ + pokemonName + @""",
                ""species"": {
                    ""name"": """ + pokemonName + @""",
                    ""url"": ""https://pokeapi.co/api/v2/pokemon-species/150/""
                }
            }";

            SetupHttpClient(content);

            var pokemon = await _pokeAPIAdapter.GetBasicPokemonInfoAsync(pokemonName);

            Assert.That(pokemon, Is.Not.Null);
            Assert.That(pokemon.Name, Is.EqualTo(pokemonName));
        }
    }
}
