using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model.Users
{
    [Table("online_user")]
    public class User
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("login")]
        public string Login { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; }

        [Required]
        [Column("password_salt")]
        public string PasswordSalt{ get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("DiscordUserName")]
        public string DiscordUserName { get; set; }

        [Column("version")]
        public int Version { get; set; }

        [Column("session_key")]
        public string SessionKey { get; set; }

        [Column("session_key_salt")]
        public string SessionKeySalt { get; set; }

        [Column("last_disconnect_time")]
        public DateTime LastDisconnectTime { get; set; }

        [Column("last_join_time")]
        public DateTime LastJoinTime { get; set; }

        public List<UserRole> UserRoles { get; set; }
    }
}
