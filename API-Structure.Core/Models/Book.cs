namespace BookManagementSystem.Core.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[]? Cover { get; set; }
        public Guid AuthorId { get; set; }
        public Author Author { get; set; }
        public int PublishYear { get; set; }
        public List<BookGenres> BookGenres { get; set; }
        public IEnumerable<Review>? Reviews { get; set; }
        public IEnumerable<Favorite>? Favorites { get; set; }
        public IEnumerable<UserBooks>? UserBooks { get; set; }
        public bool IsDeleted { get; set; }

    }
}
