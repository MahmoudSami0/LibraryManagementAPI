using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.GenereDto
{
    public class GenreDto
    {
        [JsonIgnore]
        public Guid genreId { get; set; }
        public string GenreName { get; set; }
    }
}
