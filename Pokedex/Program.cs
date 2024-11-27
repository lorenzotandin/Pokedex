
using Pokedex.Services;
using Pokedex.Services.Interfaces;

namespace Pokedex
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient<IPokemonInfoAdapter, PokeAPIAdapter>();
            builder.Services.AddHttpClient<ITranslationAdapter, FunTranslationAdapter>();

            builder.Services.AddTransient<IPokemonInfoAdapter, PokeAPIAdapter>();
            builder.Services.AddTransient<ITranslationAdapter, FunTranslationAdapter>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
