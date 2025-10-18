using System.Net;
using DotNet_portfolio.Data;
using DotNet_portfolio.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTest
{
    public class BasicApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BasicApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var producerDescriptor = services.SingleOrDefault(d =>
                            d.ServiceType == typeof(IMessageProducer)
                        );
                        if (producerDescriptor != null)
                        {
                            services.Remove(producerDescriptor);
                        }

                        var consumerDescriptor = services.SingleOrDefault(d =>
                            d.ImplementationType == typeof(RabbitMQConsumer)
                        );
                        if (consumerDescriptor != null)
                        {
                            services.Remove(consumerDescriptor);
                        }
                        services.AddSingleton<IMessageProducer, FakeMessageProducer>();

                        var dbContextDescriptor = services.SingleOrDefault(d =>
                            d.ServiceType == typeof(PortfolioDbContext)
                        );
                        if (dbContextDescriptor != null)
                        {
                            services.Remove(dbContextDescriptor);
                        }

                        var dbOptionsDescriptor = services.SingleOrDefault(d =>
                            d.ServiceType == typeof(DbContextOptions<PortfolioDbContext>)
                        );
                        if (dbOptionsDescriptor != null)
                        {
                            services.Remove(dbOptionsDescriptor);
                        }

                        services.AddDbContext<PortfolioDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryDbForApiTesting");
                        });
                    });
                })
                .CreateClient();
        }

        private class FakeMessageProducer : IMessageProducer
        {
            public void SendMessage<T>(T message) { }
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
