﻿using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Pokedex.Models.PokemonApi
{
    [DebuggerDisplay("{Name}")]
    public class Pokemon
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("species")]
        public NameAndUrl? Species { get; set; }
    }
}
