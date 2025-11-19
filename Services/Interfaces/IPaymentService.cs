namespace Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentResult> ChargeAsync(decimal amount, PaymentRequest request, CancellationToken ct = default);
}

public record PaymentRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
}

public record PaymentResult(bool Success, string? TransactionId = null, string? ErrorMessage = null);
