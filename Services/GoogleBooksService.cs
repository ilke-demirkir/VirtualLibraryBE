
using VirtualLibraryAPI.Entities;
public class GoogleBooksService
{
    private readonly HttpClient   _http;
    private readonly string       _apiKey;
    
    public GoogleBooksService(HttpClient http, IConfiguration config)
    {
        _http   = http;
        _apiKey = config["GoogleBooks:ApiKey"];
    }

    public async Task<List<Book>> SearchAndImportAsync(string title)
    {
        // 1. hit the volumes endpoint
        var url = $"volumes?q=intitle:{Uri.EscapeDataString(title)}&key={_apiKey}";
        var resp = await _http.GetFromJsonAsync<GBooksResponse>(url);

        if (resp?.Items == null) return new();

        // 2. map to your Book entity
        return resp.Items.Select(item =>
            {
                var vi = item.VolumeInfo;

                // parse year if possible
                int year = 0;
                if (!string.IsNullOrEmpty(vi.PublishedDate) &&
                    vi.PublishedDate.Length >= 4 &&
                    int.TryParse(vi.PublishedDate.Substring(0,4), out var y))
                {
                    year = y;
                }

                // pick an ISBN
                
                var imageUrl = vi.ImageLinks?.Thumbnail
                               ?? vi.ImageLinks?.SmallThumbnail;
                // choose price: retailPrice falls back to listPrice
                var money = item.SaleInfo?.RetailPrice ?? item.SaleInfo?.ListPrice;

                var isbn = item.VolumeInfo.IsbnId;
                var category = vi.Categories?.FirstOrDefault();
                
                Console.WriteLine(
                    $"[GoogleBooks] '{vi.Title}' → Category: '{category}', Image: '{imageUrl}'"
                );
                return new Book
                {
                    Name         = vi.Title,
                    Year         = year,
                    Fav          = false,                   // default
                    Author       = vi.Authors?.FirstOrDefault(),
                    Description  = vi.Description,           // map description
                    Image        = imageUrl ?? vi.ImageLinks?.SmallThumbnail ?? "/assets/placeholder.png",
                    PublishYear  = year,
                    LastUpdate   = DateTime.UtcNow,
                    Tags         = vi.Categories ?? new List<string>(),
                    Price        = money?.Amount ?? 10, 
                    Isbn        = isbn,
                    Discount     = null,
                    IsBestseller = null,
                    Category     = category ?? "Unknown",                 // single category
                    Publisher    = vi.Publisher,             // map publisher
                    Reviews      = null,                     // skip for now
                    Language     = vi.Language ?? "en",      // map if available
                    Stock        = 1,                        // default stock
                    AverageRating= vi.AverageRating 
                };
            })
            .ToList();
    }
}