using Pokedex.Services.Enum;

namespace Pokedex.Services.Interfaces
{
    public interface ITranslationAdapter
    {
        Task<TranslationResult> GetTranslationAsync(TranslationLanguage language, string? text);
    }
}
