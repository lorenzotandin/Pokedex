using FakeItEasy;
using Pokedex.Models;
using Pokedex.Services;
using System.Net;
using System.Text.Json;

namespace Pokedex.UnitTests.Services
{
    public class PokeAPIAdapterTests
    {
        private FakeableHttpMessageHandler _httpMessageHandler;
        private PokeAPIAdapter _pokeAPIAdapter;
        private const string _pokemonUrl = "https://pokeapi.co/api/v2/pokemon/";
        private const string _speciesUrl = "https://pokeapi.co/api/v2/pokemon-species/150/";

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

        private void SetupHttpClient(string? content = null, HttpStatusCode statusCode = HttpStatusCode.OK, string? requestUriPart = null)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode
            };

            if (!string.IsNullOrEmpty(content))
                response.Content = new StringContent(content);

            Func<HttpRequestMessage, bool> requestMatcher = requestUriPart == null
                ? r => true
                : r => r.RequestUri!.ToString().StartsWith(requestUriPart);

            A.CallTo(() => _httpMessageHandler.FakeSendAsync(
                A<HttpRequestMessage>.That.Matches(r => requestMatcher(r)), A<CancellationToken>.Ignored))
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
        public async Task ShouldReturnNull_IfTheApisPokemonResponseIsNotSuccessful()
        {
            SetupHttpClient(statusCode: HttpStatusCode.NotFound);

            var pokemon = await _pokeAPIAdapter.GetBasicPokemonInfoAsync("pokemonName");

            Assert.That(pokemon, Is.Null);
        }

        [Test]
        public async Task ShouldReturnNull_IfTheApisSpeciesResponseIsNotSuccessful()
        {
            var pokemonContent = GetMewtwoPokemonContent();

            var pokemonName = "mewtwo";

            SetupHttpClient(pokemonContent, requestUriPart: _pokemonUrl);

            SetupHttpClient(statusCode: HttpStatusCode.NotFound, requestUriPart: _speciesUrl);

            var pokemon = await _pokeAPIAdapter.GetBasicPokemonInfoAsync(pokemonName);

            Assert.That(pokemon, Is.Null);
        }

        private static string GetMewtwoPokemonContent()
        {
            return $@"{{
                ""name"": ""mewtwo"",
                ""species"": {{
                    ""name"": ""mewtwo"",
                    ""url"": ""{_speciesUrl}""
                }}
            }}";
        }

        [Test]
        public async Task ShouldReturnThePokemon_IfFound()
        {
            var pokemonContent = GetMewtwoPokemonContent();

            var expectedPokemonName = "mewtwo";

            SetupHttpClient(pokemonContent, requestUriPart: _pokemonUrl);

            var speciesContent = $@"{{
                ""flavor_text_entries"": [{{
                        ""flavor_text"": ""It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments."",
                        ""language"": {{
                            ""name"": ""en"",
                            ""url"": ""https://pokeapi.co/api/v2/language/9/""
                        }}
                    }}
                ],
                ""habitat"": {{
                    ""name"": ""rare"",
                    ""url"": ""https://pokeapi.co/api/v2/pokemon-habitat/5/""
                }},
                ""is_legendary"": true,
                ""name"": ""mewtwo""
            }}";

            var expectedDescription = "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.";
            var expectedHabitat = "rare";
            var expectedIsLegendary = true;

            SetupHttpClient(speciesContent, requestUriPart: _speciesUrl);

            var pokemon = await _pokeAPIAdapter.GetBasicPokemonInfoAsync(expectedPokemonName);

            Assert.That(pokemon, Is.Not.Null);
            Assert.That(pokemon.Name, Is.EqualTo(expectedPokemonName));
            Assert.That(pokemon.Description, Is.EqualTo(expectedDescription));
            Assert.That(pokemon.Habitat, Is.EqualTo(expectedHabitat));
            Assert.That(pokemon.IsLegendary, Is.EqualTo(expectedIsLegendary));
        }

        [Test]
        public async Task ShouldUseAnyEnglishDescription_IfThereAreMoreLanguages()
        {
            var pokemonContent = GetMewtwoPokemonContent();

            var expectedPokemonName = "mewtwo";

            SetupHttpClient(pokemonContent, requestUriPart: _pokemonUrl);

            var speciesContent = $@"{{
                ""flavor_text_entries"": [{{
                        ""flavor_text"": ""Creato da uno scienziato\ndopo anni di orribili\nesperimenti di\ningegneria genetica."",
                        ""language"": {{
                            ""name"": ""it"",
                            ""url"": ""https://pokeapi.co/api/v2/language/8/""
                        }}
                    }},{{
                        ""flavor_text"": ""It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments."",
                        ""language"": {{
                            ""name"": ""en"",
                            ""url"": ""https://pokeapi.co/api/v2/language/9/""
                        }}
                    }}
                ],
                ""habitat"": {{
                    ""name"": ""rare"",
                    ""url"": ""https://pokeapi.co/api/v2/pokemon-habitat/5/""
                }},
                ""is_legendary"": true,
                ""name"": ""mewtwo""
            }}";

            var expectedDescription = "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.";

            SetupHttpClient(speciesContent, requestUriPart: _speciesUrl);

            var pokemon = await _pokeAPIAdapter.GetBasicPokemonInfoAsync(expectedPokemonName);

            Assert.That(pokemon, Is.Not.Null);
            Assert.That(pokemon.Description, Is.EqualTo(expectedDescription));
        }
    }
}
