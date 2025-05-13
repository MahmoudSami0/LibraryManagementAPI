using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.FavoriteDto
{
    public class UserFavoriteDto
    {
        public Guid userId { get; set; }
        public Guid BookId { get; set; }
        //public DateTime AddedOn { get; set; } = DateTime.UtcNow;
    }
}
