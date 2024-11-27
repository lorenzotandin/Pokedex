namespace Pokedex.UnitTests.Services
{
    // Retrieved from https://fakeiteasy.github.io/docs/8.0.0/Recipes/faking-http-client/
    public abstract class FakeableHttpMessageHandler : HttpMessageHandler
    {
        public abstract Task<HttpResponseMessage> FakeSendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken);

        // sealed so FakeItEasy won't intercept calls to this method
        protected sealed override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            => this.FakeSendAsync(request, cancellationToken);
    }
}
