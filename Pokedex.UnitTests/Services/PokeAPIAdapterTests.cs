using Pokedex.Services;
using System.Net;

namespace Pokedex.UnitTests.Services
{
    public class PokeAPIAdapterTests : BaseAdapterTests
    {
        private PokeAPIAdapter _pokeAPIAdapter;
        private const string _pokemonUrl = "https://pokeapi.co/api/v2/pokemon/";
        private const string _speciesUrl = "https://pokeapi.co/api/v2/pokemon-species/150/";

        [SetUp]
        public void Setup()
        {
            var httpClient = new HttpClient(_httpMessageHandler);

            _pokeAPIAdapter = new PokeAPIAdapter(httpClient);
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task ShouldReturnNull_IfThePokemonNameIsNullOrEmpty(string? pokemonName)
        {
            var pokemonResult = await _pokeAPIAdapter.GetBasicPokemonInfoAsync(pokemonName);

            Assert.That(pokemonResult.IsSuccessful, Is.False);
        }

        [Test]
        public async Task ShouldReturnNull_IfTheApisPokemonResponseIsNotSuccessful()
        {
            SetupHttpClient(statusCode: HttpStatusCode.NotFound);

            var pokemonResult = await _pokeAPIAdapter.GetBasicPokemonInfoAsync("pokemonName");

            Assert.That(pokemonResult.IsSuccessful, Is.False);
        }

        [Test]
        public async Task ShouldReturnNull_IfTheApisSpeciesResponseIsNotSuccessful()
        {
            var pokemonContent = GetMewtwoPokemonContent();

            var pokemonName = "mewtwo";

            SetupHttpClient(pokemonContent, requestUriPart: _pokemonUrl);

            SetupHttpClient(statusCode: HttpStatusCode.NotFound, requestUriPart: _speciesUrl);

            var pokemonResult = await _pokeAPIAdapter.GetBasicPokemonInfoAsync(pokemonName);

            Assert.That(pokemonResult.IsSuccessful, Is.False);
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

            var pokemonResult = await _pokeAPIAdapter.GetBasicPokemonInfoAsync(expectedPokemonName);

            Assert.That(pokemonResult.IsSuccessful, Is.True);
            Assert.That(pokemonResult.Pokemon.Name, Is.EqualTo(expectedPokemonName));
            Assert.That(pokemonResult.Pokemon.Description, Is.EqualTo(expectedDescription));
            Assert.That(pokemonResult.Pokemon.Habitat, Is.EqualTo(expectedHabitat));
            Assert.That(pokemonResult.Pokemon.IsLegendary, Is.EqualTo(expectedIsLegendary));
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

            var pokemonResult = await _pokeAPIAdapter.GetBasicPokemonInfoAsync(expectedPokemonName);

            Assert.That(pokemonResult.IsSuccessful, Is.True);
            Assert.That(pokemonResult.Pokemon.Description, Is.EqualTo(expectedDescription));
        }
    }
}
