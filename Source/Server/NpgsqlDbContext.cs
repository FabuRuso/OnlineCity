using Server.Model.FleaMarket;
using Server.Model.Users;
using ServerCore.Model;
using ServerOnlineCity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [DbConfigurationType(typeof(NpgSqlConfiguration))]
    public class NpgsqlDbContext : DbContext
    {
        //public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<ServiceFleaMarketItem> Items { get; set; }

        public NpgsqlDbContext() : base("NpgsqlDbContext")
        {

        }


        protected override void OnModelCreating(DbModelBuilder builder)
        {
            //builder.Entity<User>().HasIndex(u => u.Login).IsUnique();

            //builder.Entity<UserRole>().HasKey(ur => new { ur.RoleId, ur.UserId });
            //builder.Entity<UserRole>().HasRequired(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
            //builder.Entity<UserRole>().HasRequired(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);

            builder.Entity<ServiceFleaMarketItem>().HasIndex(i => i.Id).IsUnique();
            builder.Entity<ServiceFleaMarketItem>().HasKey(i => new { i.FactionDefName, i.ItemDefName, i.CurrencyDefName });
        }
    }
}
