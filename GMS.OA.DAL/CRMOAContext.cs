using GMS.Account.Contract;
using GMS.Core.Config;
using GMS.Core.Log;
using GMS.Crm.Contract;
using GMS.Framework.DAL;
using GMS.OA.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMS.OA.DAL
{
    public class CRMOAContext : DbContextBase
    {
        public CRMOAContext()
            : base(CachedConfigContext.Current.DaoConfig.Crm, new LogDbContext())
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<CRMOAContext>(null);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Roles)
                .WithMany(e => e.Users)
                .Map(m =>
                {
                    m.ToTable("UserRole");
                    m.MapLeftKey("UserID");
                    m.MapRightKey("RoleID");
                });
            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Cooperations)
                .WithMany(e => e.Customers)
                .Map(m =>
                {
                    m.ToTable("CustomerCooperations");
                    m.MapLeftKey("CustomerID");
                    m.MapRightKey("CooperationsID");
                });

            //modelBuilder.Entity<Province>()
            //   .w(e => e.Citys)
            //   .WithOptional(e => e.ID)
            //   .Map(m =>
            //   {
            //       m.ToTable("CustomerCooperations");
            //       m.MapLeftKey("CustomerID");
            //       m.MapRightKey("CooperationsID");
            //   });

            //modelBuilder.Entity<City>().HasKey(e => e.ProvinceID).Map(m =>
            //{
            //    m.ToTable("Province");
            //});

            //       modelBuilder.Entity<City>()
            //.HasMany(c => c.ProvinceID).WithMany(i => i.Courses)
            //.Map(t => t.MapLeftKey("CourseID")
            //    .MapRightKey("InstructorID")
            //    .ToTable("CourseInstructor"));
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Branch> Branchs { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<VisitRecord> VisitRecords { get; set; }
        public DbSet<Business> Business { get; set; }
        public DbSet<City> Citys { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Cooperations> Cooperations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<LoginInfo> LoginInfos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<VerifyCode> VerifyCodes { get; set; }
    }
}
