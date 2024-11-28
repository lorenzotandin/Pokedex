using FakeItEasy;
using System.Net;

namespace Pokedex.UnitTests.Services
{
    public abstract class BaseAdapterTests
    {
        protected FakeableHttpMessageHandler _httpMessageHandler;

        [SetUp]
        public void BaseSetup()
        {
            _httpMessageHandler = A.Fake<FakeableHttpMessageHandler>();
        }

        [TearDown]
        public void BaseTearDown()
        {
            _httpMessageHandler.Dispose();
        }

        protected void SetupHttpClient(string? content = null, HttpStatusCode statusCode = HttpStatusCode.OK, string? requestUriPart = null)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode
            };

            if (!string.IsNullOrEmpty(content))
                response.Content = new StringContent(content);

            Func<HttpRequestMessage, bool> requestMatcher = requestUriPart == null
                ? r => true
                : r => r.RequestUri!.ToString().StartsWith(requestUriPart);

            A.CallTo(() => _httpMessageHandler.FakeSendAsync(
                A<HttpRequestMessage>.That.Matches(r => requestMatcher(r)), A<CancellationToken>.Ignored))
                .Returns(response);
        }
    }
}
