using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model.Users
{
    [Table("user_role")]
    public class UserRole
    {
        [ForeignKey("User.Id")]
        [Column("user_id")]
        public long UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Role.Id")]
        [Column("role_id")]
        public long RoleId { get; set; }
        public Role Role { get; set; }
    }
}
