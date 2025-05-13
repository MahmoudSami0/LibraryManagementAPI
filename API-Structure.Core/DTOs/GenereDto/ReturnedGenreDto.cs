using API_Structure.Core.DTOs.BookGenresDto;

namespace API_Structure.Core.DTOs.GenereDto
{
    public class ReturnedGenreDto
    {
        public Guid genreId { get; set; }
        public string genreName { get; set; }
        public List<CustomBookGenre> books { get; set; }
    }

    
}
