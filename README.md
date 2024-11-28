
# Pokedex

This is an exercise project built to study REST API development.  
It exposes two GET endpoints that return Pokemon information:
- HTTP/GET /pokemon/&lt;pokemon name&gt;
- HTTP/GET /pokemon/translated/&lt;pokemon name&gt;

## Technology

There are two projects:
- `Pokedex`: the main project, an ASP.NET Core Web API project targeting .NET 8.0.  
- `Pokedex.UnitTests`: the unit tests project. It uses `NUnit` as a testing framework and `FakeItEasy` as a mocking framework.

## How to run it

The solution needs to be opened in an IDE like Visual Studio 2022 to be executed.
- open in Visual Studio 2022
- ensure project 'Pokedex' is set as Startup project
- start the solution (Debug \ Start debugging or press F5)
- make GET requests with a tool like `HTTPie` or `Postman`, pointing to the correct localhost port
    - disable SSL verification if needed

### Endpoint 1: GET `/pokemon/<pokemon name>`

This endpoint returns basic information about a pokemon given its name.  
Example calls:
- GET `https://localhost:<port>/pokemon/`
    - response is 400 - bad request
- GET `https://localhost:<port>/pokemon/zzz`
    - response is 404 - not found
- GET `https://localhost:<port>/pokemon/mewtwo`
    - response is 200 - ok

### Endpoint 2: GET `/pokemon/translated/<pokemon name>`

Similarly to the previous endpoint, this endpoint returns basic information about a pokemon given its name.  
If the pokemon's habitat is 'cave' or if it's legendary, its description in the response will be returned with a Yoda-like translation, otherwise it will be a Shakespeare-like translation. If the translation is not available (e.g. when the daily limit of requests to the underlying translation service is passed), the normal description will be returned.  
Example calls:
- GET `https://localhost:<port>/pokemon/translated/`
    - response is 400 - bad request
- GET `https://localhost:<port>/pokemon/translated/zzz`
    - response is 404 - not found
- GET `https://localhost:<port>/pokemon/translated/mewtwo`
    - response is 200 - ok. Description is translated as Yoda since mewtwo is legendary
- GET `https://localhost:<port>/pokemon/translated/onix`
    - response is 200 - ok. Description is translated as Yoda since onix's habitat is 'cave'
- GET `https://localhost:<port>/pokemon/translated/pikachu`
    - response is 200 - ok. Description is translated as Shakespeare

## Changes for a production API

This project is an exercise.  
For a production environment I would consider these changes:

  - add authentication and authorization
  - instrument code to gather metrics and performance data on critical code fragments
  - add caching
  - limit number of requests that can be processed every minute
  - support API versioning
  - add exceptions Data to be logged where needed (while also ensuring these details do not surface to the caller)
  - provide localization for error messages
  - instead of wrapping the controller's action methods bodies in big try-catch blocks, a better approach would be to set up a middleware to handle all unhandled exceptions and return status code 500.
  - use `AutoMapper` to copy fields from one model type to another
  - explore the GraphQL v1beta version of https://pokeapi.co/ to only make one call and only retrieve the information needed
  - work in git using different branches, following the gitflow workflow
  - remove deserialization code duplication in the two adapter classes by introducing a new `JsonSerializationService` class

## Development choices

- I tried to keep a balance between having "generic" methods that help to reduce code duplication and having some duplication in methods to keep their logic explicit.  
For example, the two action methods in `PokemonController` have some code duplication, but I feel that moving this duplicated code to a new method that is then used by both action methods would hinder their readability. If more code were added that would result in more duplication, I would reconsider.

- Regarding error messages returned by the API, I wanted to provide simple but meaningful messages in case of errors, without giving away too much details for security reasons.  
And I wanted to consistently return the `application/json` content type (just like with the 200 status code case) instead of returning `text/plain` for some errors.

- Since there are different layers in the application, I decided to have three model groups to keep boundaries between them:
	- one model `Pokemon` for the response of this API
	- one model `PokemonDto` for the communication between the two Adapters and the Controller
    - some models to deserialize the data from the external APIs into the two Adapters

- I created `PokemonApiResult` and `TranslationApiResult` result classes, returned by the two Adapters to the Controller, to pass more meaningful results than just null values in case of errors.