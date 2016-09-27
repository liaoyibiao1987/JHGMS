using System;
using GMS.Framework.DAL;
using GMS.Core.Config;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using GMS.OA.Contract;
using GMS.Core.Log;
using GMS.Account.Contract;

namespace GMS.OA.DAL
{
    public class OADbContext : DbContextBase
    {
        public OADbContext()
            : base(CachedConfigContext.Current.DaoConfig.OA, new LogDbContext())
        {
        }

        ///// <summary>  
        ///// 执行原始SQL命令  
        ///// </summary>  
        ///// <param name="commandText">SQL命令</param>  
        ///// <param name="parameters">参数</param>  
        ///// <returns>影响的记录数</returns>  
        //public Object[] ExecuteSqlNonQuery<T>(string commandText, params Object[] parameters)
        //{
        //    var results = this.Database.SqlQuery<T>(commandText, parameters);
        //    results.Single();
        //    return parameters;
        //}
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<OADbContext>(null);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Branch> Branchs { get; set; }
    }
}
