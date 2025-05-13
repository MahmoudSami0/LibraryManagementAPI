using API_Structure.Core.DTOs.AuthorDtos;
using API_Structure.Core.DTOs.GenereDto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.CustomDtos
{
    public class CustomBookDto
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public byte[]? cover { get; set; }
        public int publishYear { get; set; }
        public Guid authorId { get; set; }
        public string author { get; set; }
        public List<GenreDto> genres { get; set; }
    }
}
