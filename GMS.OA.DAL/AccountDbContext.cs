using System;
using GMS.Framework.DAL;
using GMS.Core.Config;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using GMS.Account.Contract;
using GMS.Core.Log;
using GMS.Crm.Contract;

namespace GMS.Account.DAL
{
    public class AccountDbContext : DbContextBase
    {
        public AccountDbContext()
            : base(CachedConfigContext.Current.DaoConfig.Account, new LogDbContext())
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AccountDbContext>(null);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Roles)
                .WithMany(e => e.Users)
                .Map(m =>
                {
                    m.ToTable("UserRole");
                    m.MapLeftKey("UserID");
                    m.MapRightKey("RoleID");
                });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<LoginInfo> LoginInfos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<VerifyCode> VerifyCodes { get; set; }

        public DbSet<City> Citys { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Cooperations> Cooperations { get; set; }
        

    }
}
