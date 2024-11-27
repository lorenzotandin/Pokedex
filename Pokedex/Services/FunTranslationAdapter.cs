using System.Reflection.Metadata.Ecma335;
using Pokedex.Services.Enum;
using Pokedex.Services.Interfaces;

namespace Pokedex.Services
{
    public class FunTranslationAdapter : ITranslationAdapter
    {
        const string FUN_TRANSLATION_BASE_URL = "https://api.funtranslations.com/translate/";

        private readonly HttpClient _httpClient;

        public FunTranslationAdapter(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TranslationResult> GetTranslationAsync(TranslationLanguage language, string? text)
        {
            if (string.IsNullOrEmpty(text))
                return new TranslationResult();

            var funTranslationUrl = language switch
            {
                TranslationLanguage.Yoda => $"{FUN_TRANSLATION_BASE_URL}yoda.json",
                TranslationLanguage.Shakespeare => $"{FUN_TRANSLATION_BASE_URL}shakespeare.json",
            };

            return new TranslationResult();
        }
    }
}
