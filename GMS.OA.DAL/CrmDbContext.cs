using System;
using GMS.Framework.DAL;
using GMS.Core.Config;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using GMS.Crm.Contract;
using GMS.Core.Log;
using GMS.OA.Contract;

namespace GMS.Crm.DAL
{
    public class CrmDbContext : DbContextBase
    {
        public CrmDbContext()
            : base(CachedConfigContext.Current.DaoConfig.Crm, new LogDbContext())
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<CrmDbContext>(null);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Cooperations)
                .WithMany(e => e.Customers)
                .Map(m =>
                {
                    m.ToTable("CustomerCooperations");
                    m.MapLeftKey("CustomerID");
                    m.MapRightKey("CooperationsID");
                });
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

        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<VisitRecord> VisitRecords { get; set; }
        public DbSet<Business> Business { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<City> Citys { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Cooperations> Cooperations { get; set; }
        public DbSet<Payment> Payments { get; set; }

    }
}
