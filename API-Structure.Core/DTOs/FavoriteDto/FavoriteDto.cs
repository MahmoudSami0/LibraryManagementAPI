using API_Structure.Core.DTOs.BookDtos;
using API_Structure.Core.DTOs.UserDtos;

namespace API_Structure.Core.DTOs.FavoriteDto
{
    public class FavoriteDto
    {
        public Guid favoriteId { get; set; }
        public Guid userId { get; set; }
        public string userName { get; set; }
        public Guid boookId { get; set; }
        public string book { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
