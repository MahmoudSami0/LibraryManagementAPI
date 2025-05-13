using API_Structure.Core.DTOs.GenereDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.BookDtos
{
    public class BooksForAuthorDto
    {
        public Guid bookId { get; set; }
        public string title { get; set; }
        public List<GenreDto> Genres { get; set; }
    }
}
