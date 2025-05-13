namespace BookManagementSystem.Core.Models
{
    public class Genre
    {
        public Guid Id { get; set; }
        public string GenreName { get; set; }
        public IEnumerable<BookGenres> BookGenres { get; set; }
        public bool IsDeleted { get; set; }

    }
}
