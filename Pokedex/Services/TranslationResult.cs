namespace Pokedex.Services
{
    public class TranslationResult
    {
        public TranslationResult()
        {
            TranslationSuccessful = false;
        }

        public string? TranslatedText { get; set; }

        public bool TranslationSuccessful { get; set; }
    }
}
