using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.BookDtos
{
    public class UpdateBookDto
    {
        public string BookTitle { get; set; }
        public string Description { get; set; }
        public IFormFile? BookCover { get; set; }
        public int PublishYear { get; set; }
        public string AuthorName { get; set; }
    }
}
