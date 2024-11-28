using Pokedex.Services.Enum;

namespace Pokedex.Services.Interfaces
{
    public interface ITranslationAdapter
    {
        Task<TranslationApiResult> GetTranslationAsync(TranslationLanguage language, string? text);
    }
}
