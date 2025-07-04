namespace VirtualLibraryAPI.Dtos;

// Dtos/BookDto.cs
public class BookDto
{
    public int      Id {get; set;}
    public string   Name  {get; set;}
    public int      Year  {get; set;}
    public bool     Fav {get; set;}
    public string?  Author  {get; set;}
    public string?  Description   {get; set;}
    public string?  Image {get; set;}
    public int? PublishYear  {get; set;}
    public DateTime? LastUpdate  {get; set;}
    public List<string>? Tags  {get; set;}
    public decimal  Price  {get; set;}
    public int      Discount   {get; set;}
    public bool     IsBestseller  {get; set;}
    public string   Category   {get; set;}
    public string   Publisher   {get; set;}
    public string   Language     {get; set;}
    public int Stock  {get; set;}
    public decimal? AverageRating  {get; set;}
    public bool Featured {get; set;}
    
    public BookDto() {}
    
 
}
    
  


