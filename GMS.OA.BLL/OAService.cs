using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMS.OA.Contract;
using GMS.OA.DAL;
using GMS.Framework.Utility;
using GMS.Framework.Contract;
using GMS.Core.Cache;
using GMS.Account.Contract;

namespace GMS.OA.BLL
{
    public class OAService : IOAService
    {
        #region Staff CURD
        public Staff GetStaff(int id)
        {
            using (var dbContext = new OADbContext())
            {
                return dbContext.Staffs.Include("Branch").Include("User").FirstOrDefault(a => a.ID == id);
            }
        }

        public IEnumerable<Staff> GetStaffList(StaffRequest request = null)
        {
            request = request ?? new StaffRequest();
            using (var dbContext = new OADbContext())
            {
                IQueryable<Staff> staffs = dbContext.Staffs.Include("Branch").Include("User");
                if (!string.IsNullOrEmpty(request.Name))
                    staffs = staffs.Where(u => u.Name.Contains(request.Name));

                if (request.BranchId > 0)
                    staffs = staffs.Where(u => u.BranchId == request.BranchId);

                return staffs.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public IEnumerable<Staff> GetAllStaffList()
        {
            using (var dbContext = new OADbContext())
            {
                IEnumerable<Staff> staffs = dbContext.Staffs.Include("Branch").Include("User").ToList();
                return staffs.OrderByDescending(u => u.ID);
            }
        }

        public void SaveStaff(Staff staff)
        {
            using (var dbContext = new OADbContext())
            {

                if (staff.ID > 0)
                {
                    dbContext.Update<Staff>(staff);
                }
                else
                {
                    Staff newstaff = dbContext.Insert<Staff>(staff);
                    if (newstaff != null)
                    {
                        if (newstaff.UserID != null)
                        {
                            var user = dbContext.Users.FirstOrDefault(p => p.ID == newstaff.UserID);
                            user.StaffID = newstaff.ID;
                            dbContext.Update<User>(user);
                        }
                    }

                }
            }
        }

        public void DeleteStaff(List<int> ids)
        {
            using (var dbContext = new OADbContext())
            {
                Func<User, User> fun = new Func<User, User>((p) =>
                {
                    if (ids.Contains(p.ID))
                    {
                        p.StaffID = null;
                    }
                    return p;
                });
                //dbContext.Users.SqlQuery("update [user] set StaffID in'{0}' ", string.Join(",", ids)).;
                //dbContext.Users.Update<User>(p => ids.Contains(p.ID), q => q.UpdateStaffID(9));
                //dbContext.Users.ToList().ForEach((u) =>
                //    {
                //        if (ids.Contains(u.ID))
                //        {
                //            u.StaffID = null;
                //        }
                //    });
                // var staffID = new System.Data.SqlClient.SqlParameter
                //{
                //    ParameterName = "@id",
                //    Value = 1
                //};
                //var votesParam = new System.Data.SqlClient.SqlParameter
                //{
                //    ParameterName = "@voteCount",
                //    Value = 0,
                //    Direction = ParameterDirection.Output
                //};
                //dbContext.Database.ExecuteSqlCommand("update User set StaffID = @StaffID on ID IN @IDS",,string.Join(",", ids));
                dbContext.Users.Update(p => ids.Contains(p.StaffID.Value), u => new User { StaffID = null });
                dbContext.Staffs.Where(u => ids.Contains(u.ID)).Delete();
            }
        }


        public List<int> GetBelongsStaff(int id)
        {
            List<Staff> staffs = GetBelongsStaffEntity(id);
            if (staffs != null)
            {
                return staffs.Select(p => p.ID).ToList();
            }
            else
            {
                return null;
            }
        }
        public List<Staff> GetBelongsStaffEntity(int id)
        {
            using (var dbContext = new OADbContext())
            {
                var staff = dbContext.Staffs.FirstOrDefault(a => a.ID == id);

                if (staff != null && staff.BranchId.HasValue)
                {
                    List<int> branchs = GetALLBranch(staff.BranchId.Value);
                    if (branchs != null && branchs.Count > 0)
                    {
                        return dbContext.Staffs.Where(p => branchs.Contains(p.BranchId.Value)).ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        private List<int> GetALLBranch(int BranchID)
        {
            List<int> ret = new List<int> { BranchID };
            IEnumerable<Branch> sonBranch = GetSonBranch(BranchID);
            if (sonBranch.Count() > 0)
            {
                ret.AddRange(sonBranch.Select(p => p.ID));
            }

            return ret;
        }
        public List<Branch> GetBelongsToBranch(int BranchID)
        {
            List<Branch> ret = new List<Branch> { GetBranch(BranchID) };
            IEnumerable<Branch> parentsBranch = GetParentBranch(BranchID);
            if (parentsBranch.Count() > 0)
            {
                ret.AddRange(parentsBranch);
            }

            return ret;
        }

        private IEnumerable<Branch> GetSonBranch(int BranchID)
        {
            using (var dbContext = new OADbContext())
            {
                var query = from c in dbContext.Branchs
                            where c.ParentId.Equals(BranchID)
                            select c;
                return query.ToList().Concat(query.ToList().SelectMany(t => GetSonBranch(t.ID)));
            }
        }
        private IEnumerable<Branch> GetParentBranch(int BranchID)
        {
            using (var dbContext = new OADbContext())
            {
                Branch branch = dbContext.Branchs.FirstOrDefault(p => p.ID == BranchID);
                var query = from c in dbContext.Branchs
                            where c.ID.Equals(branch.ParentId)
                            select c;
                return query.ToList().Concat(query.ToList().SelectMany(t => GetParentBranch(t.ID)));
            }
        }
        #endregion


        #region Branch CURD
        public Branch GetBranch(int id)
        {
            using (var dbContext = new OADbContext())
            {
                return dbContext.Find<Branch>(id);
            }
        }

        public IEnumerable<Branch> GetBranchList(BranchRequest request = null)
        {
            request = request ?? new BranchRequest();
            using (var dbContext = new OADbContext())
            {
                IQueryable<Branch> branchs = dbContext.Branchs;

                if (!string.IsNullOrEmpty(request.Name))
                    branchs = branchs.Where(u => u.Name.Contains(request.Name));

                return branchs.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveBranch(Branch branch)
        {
            using (var dbContext = new OADbContext())
            {
                if (branch.ID > 0)
                {
                    dbContext.Update<Branch>(branch);
                }
                else
                {
                    dbContext.Insert<Branch>(branch);
                }
            }
        }

        public void DeleteBranch(List<int> ids)
        {
            using (var dbContext = new OADbContext())
            {
                dbContext.Branchs.Where(u => ids.Contains(u.ID)).Delete();
            }
        }
        #endregion

        /// <summary>
        /// 返回树形组织结构，根节点RootBranch(id=0, ParentId=0)，其下Staff和Branch为未分配的，根节点下的第一个节点为TopBranch(id=1, ParantId=0)，其他ParentId=0的归为RootBranch下
        /// </summary>
        /// <returns></returns>
        public Branch GetOrg()
        {
            using (var dbContext = new OADbContext())
            {
                var branchs = dbContext.Branchs.ToList();
                var staffs = dbContext.Staffs.ToList();

                var branch = new Branch();
                AppendBranch(branchs, staffs, branch);

                return branch;
            }
        }

        private void AppendBranch(IEnumerable<Branch> allBranch, IEnumerable<Staff> allStaff, Branch branch)
        {
            branch.Embranchment = allBranch.Where(b => b.ParentId == branch.ID).Select(b => new Branch() { ID = b.ID, Name = b.Name }).ToList();
            branch.Staffs = allStaff.Where(s => s.BranchId.Value == branch.ID).Select(b => new Staff() { ID = b.ID, Name = b.Name }).ToList();
            branch.Embranchment.ForEach(b => AppendBranch(allBranch, allStaff, b));
        }

        /// <summary>
        /// 保存树形组织结构（刷全表数据/BatchUpdate），先取出来所有的Branch,Staff的ParentId为0，然后在根据RootBranch的树形结构赋对应的值
        /// </summary>
        /// <param name="rootBranch"></param>
        public void SaveOrg(Branch rootBranch)
        {
            using (var dbContext = new OADbContext())
            {
                var branchs = dbContext.Branchs.ToList();
                branchs.ForEach(b => b.ParentId = 0);

                var staffs = dbContext.Staffs.ToList();
                staffs.ForEach(s => s.BranchId = 0);

                UpdateOrg(branchs, staffs, rootBranch);

                dbContext.SaveChanges();
            }
        }

        private void UpdateOrg(List<Branch> allBranch, List<Staff> allStaff, Branch currentBranch)
        {
            if (currentBranch.Staffs != null)
            {
                foreach (var staff in currentBranch.Staffs)
                {
                    var existStaff = allStaff.SingleOrDefault(s => s.ID == staff.ID);
                    if (existStaff != null)
                        existStaff.BranchId = currentBranch.ID;
                }
            }

            if (currentBranch.Embranchment != null)
            {
                foreach (var branch in currentBranch.Embranchment)
                {
                    var existBranch = allBranch.SingleOrDefault(b => b.ID == branch.ID);
                    if (existBranch != null)
                        existBranch.ParentId = currentBranch.ID;

                    UpdateOrg(allBranch, allStaff, branch);
                }
            }
        }
    }
}
