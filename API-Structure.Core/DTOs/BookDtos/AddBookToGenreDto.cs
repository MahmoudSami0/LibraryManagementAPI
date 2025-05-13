using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.BookDtos
{
    public class AddBookToGenreDto
    {
        public string bookTitle { get; set; }
        public string genreName { get; set; }
    }
}
