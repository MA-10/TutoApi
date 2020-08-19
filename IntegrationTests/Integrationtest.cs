using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Test;
using Test.Data;

namespace IntegrationTests
{
    public class IntegrationTest : IDisposable
    {
        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>(options => { options.UseInMemoryDatabase("TestDb"); });
                    });
                });

            _serviceProvider = appFactory.Services;
            TestClient = appFactory.CreateClient();
        }
        protected readonly HttpClient TestClient;
        private readonly IServiceProvider _serviceProvider;
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}