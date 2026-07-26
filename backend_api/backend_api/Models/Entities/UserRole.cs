using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("UserRoles")]
    public class UserRole
    {
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
    }
}
