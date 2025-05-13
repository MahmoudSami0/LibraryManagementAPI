using Newtonsoft.Json;

namespace API_Structure.Core.DTOs.UserDtos
{
    public class UserAuthDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public Guid userId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public List<string>? Roles { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
