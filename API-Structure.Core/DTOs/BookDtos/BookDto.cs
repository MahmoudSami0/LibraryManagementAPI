using API_Structure.Core.DTOs.GenereDto;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace API_Structure.Core.DTOs.BookDtos
{
    public class BookDto
    {
        public string BookTitle { get; set; }
        public string Description { get; set; }
        public IFormFile? BookCover { get; set; }
        public int PublishYear { get; set; }
        //[JsonIgnore]
        //public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public List<GenreDto> Genres { get; set; }
    }
}
