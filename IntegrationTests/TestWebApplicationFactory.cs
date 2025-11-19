using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory that disables anti-forgery validation for integration tests
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing IAntiforgery registration
            var antiforgeryDescriptor = services.FirstOrDefault(d => 
                d.ServiceType == typeof(IAntiforgery));
            if (antiforgeryDescriptor != null)
            {
                services.Remove(antiforgeryDescriptor);
            }
            
            // Add a fake antiforgery service that always validates
            services.AddSingleton<IAntiforgery, FakeAntiforgery>();
        });
    }
    
    private class FakeAntiforgery : IAntiforgery
    {
        public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
        {
            return new AntiforgeryTokenSet("test", "test", "test", "test");
        }

        public AntiforgeryTokenSet GetTokens(HttpContext httpContext)
        {
            return new AntiforgeryTokenSet("test", "test", "test", "test");
        }

        public Task<bool> IsRequestValidAsync(HttpContext httpContext)
        {
            return Task.FromResult(true);
        }

        public void SetCookieTokenAndHeader(HttpContext httpContext)
        {
            // Do nothing
        }

        public Task ValidateRequestAsync(HttpContext httpContext)
        {
            return Task.CompletedTask;
        }
    }
}
