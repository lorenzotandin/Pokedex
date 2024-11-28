using Pokedex.Services;
using Pokedex.Services.Enum;
using System.Net;

namespace Pokedex.UnitTests.Services
{
    public class FunTranslationAdapterTests : BaseAdapterTests
    {
        private FunTranslationAdapter _funTranslationAdapter;
        private const string _yodaTranslateUrl = "https://api.funtranslations.com/translate/yoda.json";
        private const string _shakespeareTranslateUrl = "https://api.funtranslations.com/translate/shakespeare.json";

        [SetUp]
        public void Setup()
        {
            var httpClient = new HttpClient(_httpMessageHandler);

            _funTranslationAdapter = new FunTranslationAdapter(httpClient);
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task ShouldReturnFailure_IfTheTextIsNullOrEmpty(string? text)
        {
            var translation = await _funTranslationAdapter.GetTranslationAsync(TranslationLanguage.Yoda, text);

            Assert.That(translation.IsSuccessful, Is.False);
        }

        [Test]
        public async Task ShouldReturnFailure_IfTheApisTranslationResponseIsNotSuccessful()
        {
            SetupHttpClient(statusCode: HttpStatusCode.TooManyRequests);

            var translation = await _funTranslationAdapter.GetTranslationAsync(TranslationLanguage.Yoda, "text");

            Assert.That(translation.IsSuccessful, Is.False);
        }

        [TestCase(TranslationLanguage.Yoda, _yodaTranslateUrl)]
        [TestCase(TranslationLanguage.Shakespeare, _shakespeareTranslateUrl)]
        public async Task ShouldReturnTheTranslatedText_IfTheApisTranslationIsDone(TranslationLanguage language, string translationUrl)
        {
            var translationContent = @"{
              ""success"": {
                ""total"": 1
              },
              ""contents"": {
                ""translation"": ""yoda"",
                ""text"": ""source text"",
                ""translated"": ""translated text""
              }
            }";

            SetupHttpClient(translationContent, requestUriPart: translationUrl);

            var translation = await _funTranslationAdapter.GetTranslationAsync(language, "source text");

            Assert.That(translation.IsSuccessful, Is.True);
            Assert.That(translation.TranslatedText, Is.EqualTo("translated text"));
        }

        [Test]
        public async Task ShouldReturnFailure_IfTheApisTranslationIsNotDone()
        {
            var translationContent = @"{
              ""success"": {
                ""total"": 0
              },
              ""contents"": {}
            }";

            SetupHttpClient(translationContent, requestUriPart: _yodaTranslateUrl);

            var translation = await _funTranslationAdapter.GetTranslationAsync(TranslationLanguage.Yoda, "source text");

            Assert.That(translation.IsSuccessful, Is.False);
        }
    }
}
