using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ApiTest
{
    public class BasicApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public BasicApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetPortfolioEndpoint_ReturnsSuccessAndCorrectMessage()
        {
            var url = "/portfolio";

            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Portfolio API is running successfully.", responseString);
        }

        [Fact]
        public async Task GetProjectsEndpoint_ReturnsSuccess()
        {
            var url = "/api/Projects";

            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
