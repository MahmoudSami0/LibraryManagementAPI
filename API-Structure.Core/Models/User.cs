

using API_Structure.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace BookManagementSystem.Core.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? Phone { get; set; }
        public byte[]? Photo { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public DateTime? EmailConfirmationTokenExpiration { get; set; }
        public bool IsDeleted { get; set; }
        public IEnumerable<UserRoles> UserRoles { get; set; }
        public IEnumerable<Review>? Reviews { get; set; }
        public IEnumerable<Favorite>? Favorites { get; set; }
        public IEnumerable<UserBooks>? UserBooks { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
