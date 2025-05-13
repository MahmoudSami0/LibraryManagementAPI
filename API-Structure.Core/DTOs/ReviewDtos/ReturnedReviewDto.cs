using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.ReviewDtos
{
    public class ReturnedReviewDto
    {
        public Guid reviewId { get; set; }
        public Guid userId { get; set; }
        public string userName { get; set; }
        public Guid bookId { get; set; }
        public string bookTitle { get; set; }
        public string reviewContent { get; set; }
        public DateTime addedOn { get; set; }
        public DateTime? updatedOn { get; set; }
    }
}
