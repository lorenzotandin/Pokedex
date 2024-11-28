namespace Pokedex.Services
{
    public class TranslationApiResult
    {
        private TranslationApiResult()
        {
            IsSuccessful = false;
        }

        public static TranslationApiResult Failure()
        { 
            return new TranslationApiResult();
        }

        public static TranslationApiResult Success(string? translatedText)
        {
            return new TranslationApiResult
            { 
                TranslatedText = translatedText,
                IsSuccessful = true
            };
        }

        public string? TranslatedText { get; set; }

        public bool IsSuccessful { get; set; }
    }
}
