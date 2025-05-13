using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.BookGenresDto
{
    public class CustomBookGenre
    {
        public Guid bookId { get; set; }
        public string BookTitle { get; set; }
        public string Description { get; set; }
        public int PublishYear { get; set; }
        public Guid authorId { get; set; }
        public string authorName { get; set; }
    }
}
