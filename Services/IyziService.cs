// Services/IyziService.cs

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Iyzipay;                     // for Options
using Iyzipay.Model;               // for CheckoutFormInitialize, CheckoutForm, BasketItem, Buyer
using Iyzipay.Request;             // for CreateCheckoutFormInitializeRequest, RetrieveCheckoutFormRequest
using Microsoft.EntityFrameworkCore;
using VirtualLibraryAPI.Data;      // for LibraryDbContext
using VirtualLibraryAPI.Entities;  // for your CartItem & Book entities
using System.Text.Json.Serialization;
using System.Text.Json;

namespace VirtualLibraryAPI.Services
{
    public class IyziService
    {
        private readonly Options           _opts;
        private readonly LibraryDbContext  _ctx;
        private readonly CartService       _cart;

        public IyziService(
            Options opts,
            LibraryDbContext ctx,
            CartService cart
        ) {
            _opts = opts;
            _ctx  = ctx;
            _cart = cart;
        }

        /// <summary>
        /// Returns the sandbox HTML form that will auto‐post into İyzico.
        /// </summary>
        public async Task<string> CreatePaymentFormAsync(string callbackUrl)
        {
            // 1) Load cart items and their Book nav props
            var userId = _cart.CurrentUserId;
            var fullCallback = $"{callbackUrl}?userId={userId}";
            
            
            var items = await _ctx.CartItems
                                  .Include(ci => ci.Book)
                                  .Where(ci => ci.UserId == userId)
                                  .ToListAsync();

            // 2) Compute total as decimal
            decimal? totalmidValue = items.Sum(ci =>
                (ci.Book.Price) * ci.Quantity
            );
            decimal totalValue = totalmidValue.HasValue ? totalmidValue.Value : 0;
            // Format as e.g. "123.45"
            string total = totalValue
                .ToString("F2", CultureInfo.InvariantCulture);

            // 3) Build the checkout‐form initialize request
            var req = new CreateCheckoutFormInitializeRequest
            {
                Locale         = Locale.TR.ToString(),
                ConversationId = Guid.NewGuid().ToString(),
                Price          = total,
                PaidPrice      = total,
                Currency       = Currency.TRY.ToString(),
                CallbackUrl    = callbackUrl,
                PaymentGroup   = PaymentGroup.PRODUCT.ToString(),
                BasketId = Guid.NewGuid().ToString(),
 
                Buyer = new Buyer
                {
                    Id                  = userId.ToString(),
                    Name                = "Test",
                    Surname             = "Buyer",
                    GsmNumber           = "+905350000000",
                    Email               = "email@example.com",
                    IdentityNumber      = "11111111111",
                    RegistrationAddress = "Test Address",
                    Ip                  = "127.0.0.1",
                    City                = "Istanbul",
                    Country             = "Turkey"
                },
                ShippingAddress = new Address
                {
                    ContactName = "Test-Buyer",
                    City = "Istanbul",
                    Country = "Turkey",
                    Description = "Test Description",
                    ZipCode = "12345"
                },
                BillingAddress = new Address
                {
                    ContactName = "Test-Buyer",
                    City = "Istanbul",
                    Country = "Turkey",
                    Description = "Test Description",
                    ZipCode = "12345"
                },
                BasketItems = items.Select(ci => {
                    // coalesce Price to 0m if it were ever null
                    var lineTotal = (ci.Book.Price ?? 0m) * ci.Quantity;
                    return new BasketItem {
                        Id        = ci.Book.Id.ToString(),
                        Name      = ci.Book.Name,
                        Category1 = string.IsNullOrWhiteSpace(ci.Book.Category)
                            ? "General"
                            : ci.Book.Category!,
                        ItemType  = BasketItemType.PHYSICAL.ToString(),
                        // now lineTotal is a plain decimal
                        Price     = lineTotal.ToString("F2", CultureInfo.InvariantCulture)
                    };
                }).ToList()
            };
            var jsonOptions = new JsonSerializerOptions {
                WriteIndented            = true,
                PropertyNamingPolicy     = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition   = JsonIgnoreCondition.Never
            };
            
            var payloadJson = JsonSerializer.Serialize(req, jsonOptions);
            Console.WriteLine("=== Iyzico Init Request JSON ===");
            Console.WriteLine(payloadJson);
            req.CallbackUrl = fullCallback;

            // 4) Call the checkout‐form initialize API
            var init = await CheckoutFormInitialize.Create(req, _opts);
            Console.WriteLine("=== Iyzi Initialize Response ===");
            Console.WriteLine($"Status      : {init.Status}");
            Console.WriteLine($"ErrorCode   : {init.ErrorCode}");
            Console.WriteLine($"ErrorMessage: {init.ErrorMessage}");
            Console.WriteLine($"HtmlContent : {(init.CheckoutFormContent == null ? "<null>" : init.CheckoutFormContent.Substring(0, Math.Min(100, init.CheckoutFormContent.Length)) + "...")}");
            // 5) Return the raw HTML to render via [innerHTML]
            return init.CheckoutFormContent;
        }

        /// <summary>
        /// Called by your /api/iyzi/callback endpoint with the form’s token.
        /// </summary>
        public async Task HandleCallbackAsync(string token, int userId)
        {
            // 1) Retrieve the form status
            var retrieveReq = new RetrieveCheckoutFormRequest { Token = token };
            var form =await  CheckoutForm.Retrieve(retrieveReq, _opts);

            // 2) On success, decrement stock + clear cart
            if (form.Status == Status.SUCCESS.ToString())
            {
                await _cart.CheckoutAsync(userId);
            }
        }
    }
}
