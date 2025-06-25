namespace VirtualLibraryAPI.Models{
    public class Book{
        public int Id {get; set;}
        public string Name { get; set;} = string.Empty;
        public int Year{get; set;}
        public bool Fav {get; set;}
    }
}