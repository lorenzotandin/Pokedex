using System.Text.Json;
using FunTranslation = Pokedex.Models.FunTranslation;
using Pokedex.Services.Enum;
using Pokedex.Services.Interfaces;

namespace Pokedex.Services
{
    public class FunTranslationAdapter : ITranslationAdapter
    {
        const string FUN_TRANSLATION_BASE_URL = "https://api.funtranslations.com/translate/";

        private readonly HttpClient _httpClient;

        private JsonSerializerOptions _jsonSerializerOptions;

        public FunTranslationAdapter(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<TranslationApiResult> GetTranslationAsync(TranslationLanguage language, string? text)
        {
            if (string.IsNullOrEmpty(text))
                return TranslationApiResult.Failure();

            var funTranslationUrl = language switch
            {
                TranslationLanguage.Yoda => $"{FUN_TRANSLATION_BASE_URL}yoda.json",
                TranslationLanguage.Shakespeare => $"{FUN_TRANSLATION_BASE_URL}shakespeare.json",
            };

            var postData = new Dictionary<string, string>
            {
                { "text", text }
            };

            var content = new FormUrlEncodedContent(postData);

            var response = await _httpClient.PostAsync(funTranslationUrl, content);

            if (!response.IsSuccessStatusCode)
                return TranslationApiResult.Failure();

            var translation = await DeserializeContentAsync<FunTranslation.Translation>(response);

            if (translation!.Success!.Total == 1)
                return TranslationApiResult.Success(translation!.Contents!.Translated!.Replace("  ", " "));

            return TranslationApiResult.Failure();
        }

        private async Task<T> DeserializeContentAsync<T>(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(responseContent, _jsonSerializerOptions)!;
        }
    }
}
