using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.ReviewDtos
{
    public class ReviewDto
    {
        public Guid userId { get; set; }
        public Guid bookId { get; set; }
        public string reviewContent { get; set; }
    }
}
