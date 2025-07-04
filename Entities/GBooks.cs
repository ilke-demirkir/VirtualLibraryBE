using System.Text.Json.Serialization;

public class GBooksResponse
{
    [JsonPropertyName("items")]
    public List<GBookItem> Items { get; set; }
}

public class GBookItem
{
    [JsonPropertyName("volumeInfo")]
    public VolumeInfo VolumeInfo { get; set; }

    [JsonPropertyName("saleInfo")]
    public SaleInfo    SaleInfo    { get; set; }
}

public class VolumeInfo
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("authors")]
    public List<string> Authors { get; set; }

    [JsonPropertyName("publishedDate")]
    public string PublishedDate { get; set; }   // e.g. "2005-07-16" or just "2005"

    [JsonPropertyName("industryIdentifiers")]
    public List<IndustryIdentifier> IndustryIdentifiers { get; set; }
    
    [JsonPropertyName("categories")]
    public List<string> Categories { get; set; }

    [JsonPropertyName("imageLinks")]
    public ImageLinks ImageLinks { get; set; }
    
    [JsonPropertyName("description")]
    public string Description { get; set; }
    
    [JsonPropertyName("publisher")]
    public string Publisher { get; set; }
    
    [JsonPropertyName("language")]
    public string Language { get; set; }
    [JsonPropertyName("averageRating")]
    public decimal? AverageRating { get; set; }
    
    [JsonIgnore]
    public string? IsbnId =>
        IndustryIdentifiers?
            .FirstOrDefault(i =>
                i.Type?.Equals("ISBN_13", StringComparison.OrdinalIgnoreCase) == true ||
                i.Type?.Equals("ISBN_10", StringComparison.OrdinalIgnoreCase) == true
            )
            ?.Identifier;
}

public class IndustryIdentifier
{
    [JsonPropertyName("type")]
    public string Type { get; set; }    // "ISBN_10" or "ISBN_13"

    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }
}

public class ImageLinks
{
    [JsonPropertyName("smallThumbnail")]
    public string SmallThumbnail { get; set; }
    
    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; }
}

public class SaleInfo
{
    [JsonPropertyName("listPrice")]
    public Money ListPrice { get; set; }

    [JsonPropertyName("retailPrice")]
    public Money RetailPrice { get; set; }
}

public class Money
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; }
}