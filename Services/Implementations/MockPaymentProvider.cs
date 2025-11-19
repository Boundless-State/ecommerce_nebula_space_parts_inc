using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace Services.Implementations;

public class MockPaymentProvider(ILogger<MockPaymentProvider> logger) : IPaymentService
{
    private readonly ILogger<MockPaymentProvider> _logger = logger;

    public Task<PaymentResult> ChargeAsync(decimal amount, PaymentRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Mock payment processed for {Name} amount {Amount}", request.CustomerName, amount);
        return Task.FromResult(new PaymentResult(true, Guid.NewGuid().ToString("N")));
    }
}
