namespace BookManagementSystem.Core.Models
{
    public class Author
    {
        public Guid Id { get; set; }
        public string AuthorName { get; set; }
        public IEnumerable<Book>? Books { get; set; }
        public bool IsDeleted { get; set; }
    }
}
