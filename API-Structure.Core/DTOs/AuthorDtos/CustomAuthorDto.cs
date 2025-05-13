using API_Structure.Core.DTOs.BookDtos;
using BookManagementSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.AuthorDtos
{
    public class CustomAuthorDto
    {
        public Guid authorId { get; set; }
        public string name { get; set; }
        public List<BooksForAuthorDto>? Books { get; set; }
    }
}
