using BookManagementSystem.Core.Models;


namespace API_Structure.Core.Models
{
    public class UserRoles
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public bool IsDeleted { get; set; }
    }
}
