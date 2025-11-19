using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;
using Xunit;

namespace IntegrationTests;

public class ProgramConfigBranchTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    public ProgramConfigBranchTests(TestWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public void Default_Config_Registers_PaymentService()
    {
        using var scope = _factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        Assert.NotNull(svc);
    }

    [Fact]
    public void Stripe_Config_Executes_Stripe_Branch()
    {
        var dict = new Dictionary<string, string?> { ["PaymentProvider"] = "stripe" };
        var custom = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((ctx, cfg) => cfg.AddInMemoryCollection(dict));
        });
        using var scope = custom.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        Assert.NotNull(svc);
    }
}
