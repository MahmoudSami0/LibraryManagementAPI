namespace BookManagementSystem.Core.Models
{
    public class BookGenres
    {
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public Guid GenreId { get; set; }
        public Genre Genre { get; set; }
        public bool IsDeleted { get; set; }
    }
}
