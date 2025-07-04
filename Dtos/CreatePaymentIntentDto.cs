using Stripe;

namespace VirtualLibraryAPI.Dtos;

public class CreatePaymentFormDto
{
    public string CallbackUrl { get; set; } = null!;
}