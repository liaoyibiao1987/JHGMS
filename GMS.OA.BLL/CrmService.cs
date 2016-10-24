using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMS.Crm.Contract;
using GMS.Crm.DAL;
using GMS.Framework.Utility;
using System.Data.Objects;
using GMS.Framework.Contract;
using EntityFramework.Extensions;
using GMS.Core.Cache;
using GMS.Account.DAL;
using GMS.OA.Contract;

namespace GMS.Crm.BLL
{
    public class CrmService : ICrmService
    {
        #region Project CURD
        public Project GetProject(int id)
        {
            using (var dbContext = new CrmDbContext())
            {
                return dbContext.Find<Project>(id);
            }
        }

        public IEnumerable<Project> GetProjectList(ProjectRequest request = null)
        {
            request = request ?? new ProjectRequest();
            using (var dbContext = new CrmDbContext())
            {
                IQueryable<Project> projects = dbContext.Projects;

                if (!string.IsNullOrEmpty(request.Name))
                    projects = projects.Where(u => u.Name.Contains(request.Name));

                return projects.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveProject(Project project)
        {
            using (var dbContext = new CrmDbContext())
            {
                if (project.ID > 0)
                {
                    dbContext.Update<Project>(project);
                }
                else
                {
                    dbContext.Insert<Project>(project);
                }
            }
        }

        public void DeleteProject(List<int> ids)
        {
            using (var dbContext = new CrmDbContext())
            {
                dbContext.Projects.Where(u => ids.Contains(u.ID)).Delete();
            }
        }
        #endregion

        #region Customer CURD
        public Customer GetCustomer(int id)
        {
            using (var dbContext = new CrmDbContext())
            {
                return dbContext.Customers.Include("Cooperations").FirstOrDefault(p => p.ID == id);
            }
        }

        public IEnumerable<Customer> GetCustomerList(List<int> staffids, CustomerRequest request = null)
        {
            request = request ?? new CustomerRequest();
            using (var dbContext = new CrmDbContext())
            {
                //IQueryable<Customer> queryList = dbContext.Customers.Include("VisitRecords").Include("Staff");
                IQueryable<Customer> queryList = dbContext.Customers.Include("Cooperations").Include("Staff").Include("City").Where(p => staffids.Contains(p.StaffID.Value));
                //var rec = from a in queryList
                //          join b in dbContext.Provinces on a.City.ProvinceID equals b.ID
                //          select a;


                if (!string.IsNullOrEmpty(request.Customer.Name))
                    queryList = queryList.Where(d => d.Name.Contains(request.Customer.Name));
                if (!string.IsNullOrEmpty(request.Customer.Tel))
                    queryList = queryList.Where(d => d.Tel.Contains(request.Customer.Tel));

                if (request.Customer.Gender > 0)
                    queryList = queryList.Where(d => d.Gender == request.Customer.Gender);

                if (request.Customer.Category > 0)
                    queryList = queryList.Where(d => d.Category == request.Customer.Category);

                return queryList.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveCustomer(Customer customer)
        {
            using (var dbContext = new CrmDbContext())
            {
                if (customer.ID > 0)
                {
                    if (dbContext.Customers.Any(c => c.Tel == customer.Tel && c.ID != customer.ID))
                        throw new BusinessException("Tel", "已存在此电话的客户！");


                    dbContext.Update<Customer>(customer);
                    List<Cooperations> customercooperations = dbContext.Cooperations.Where(r => customer.CustomerCooperationsIds.Contains(r.ID)).ToList();
                    customer.Cooperations = customercooperations;
                    dbContext.SaveChanges();
                }
                else
                {
                    if (dbContext.Customers.Any(c => c.Tel == customer.Tel))
                        throw new BusinessException("Tel", "已存在此电话的客户！");

                    customer = dbContext.Insert<Customer>(customer);
                }
            }
        }

        public void DeleteCustomer(List<int> ids)
        {
            using (var dbContext = new CrmDbContext())
            {
                dbContext.Customers.Where(u => ids.Contains(u.ID)).Delete();
            }
        }
        #endregion

        #region VisitRecord CURD
        public VisitRecord GetVisitRecord(int id)
        {
            using (var dbContext = new CrmDbContext())
            {
                return dbContext.Find<VisitRecord>(id);
            }
        }

        public IEnumerable<VisitRecord> GetVisitRecordList(VisitRecordRequest request = null)
        {
            request = request ?? new VisitRecordRequest();
            using (var dbContext = new CrmDbContext())
            {
                IQueryable<VisitRecord> queryList = dbContext.VisitRecords.Include("Project").Include("Customer");

                var model = request.VisitRecord;

                if (!string.IsNullOrEmpty(model.Username))
                    queryList = queryList.Where(d => d.Username.Contains(model.Username));

                if (model.Customer != null && !string.IsNullOrEmpty(model.Customer.Name))
                    queryList = queryList.Where(d => d.Customer.Name.Contains(model.Customer.Name));

                if (model.Customer != null && !string.IsNullOrEmpty(model.Customer.Tel))
                    queryList = queryList.Where(d => d.Customer.Tel.Contains(model.Customer.Tel));

                if (request.StartHour != null && request.EndHour != null)
                {
                    var startHour = request.StartHour.Value - 1;
                    var endHour = request.EndHour.Value - 1;

                    queryList = queryList.Where(d => d.VisitTime.Hour >= startHour && d.VisitTime.Hour <= endHour);
                }

                var startDate = request.StartDate == null ? DateTime.Now.AddMonths(-3) : request.StartDate.Value;
                queryList = queryList.Where(d => d.VisitTime > startDate);

                var endDate = request.EndDate == null ? DateTime.Now.AddDays(1) : request.EndDate.Value;
                queryList = queryList.Where(d => d.VisitTime < endDate);

                if (model.FollowStep > 0)
                    queryList = queryList.Where(d => d.FollowStep == model.FollowStep);

                if (model.FollowLevel > 0)
                    queryList = queryList.Where(d => d.FollowLevel == model.FollowLevel);

                if (model.ProjectId > 0)
                    queryList = queryList.Where(d => d.ProjectId == model.ProjectId);

                if (model.Motivation > 0)
                    queryList = queryList.Where(d => d.Motivation == model.Motivation);

                if (model.AreaDemand > 0)
                    queryList = queryList.Where(d => d.AreaDemand == model.AreaDemand);

                if (model.PriceResponse > 0)
                    queryList = queryList.Where(d => d.PriceResponse == model.PriceResponse);

                if (model.Focus > 0)
                    queryList = queryList.Where(d => (d.Focus & model.Focus) != 0);

                if (model.CognitiveChannel > 0)
                    queryList = queryList.Where(d => (d.CognitiveChannel & model.CognitiveChannel) != 0);

                if (model.VisitWay > 0)
                    queryList = queryList.Where(d => d.VisitWay == model.VisitWay);

                if (model.AreaId > 0)
                    queryList = queryList.Where(d => d.AreaId == model.AreaId);

                return queryList.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveVisitRecord(VisitRecord visitRecord)
        {
            using (var dbContext = new CrmDbContext())
            {
                if (visitRecord.ID > 0)
                {
                    dbContext.Update<VisitRecord>(visitRecord);
                }
                else
                {
                    dbContext.Insert<VisitRecord>(visitRecord);
                }
            }
        }

        public void DeleteVisitRecord(List<int> ids)
        {
            using (var dbContext = new CrmDbContext())
            {
                dbContext.VisitRecords.Where(u => ids.Contains(u.ID)).Delete();
            }
        }
        #endregion

        #region Business

        public IEnumerable<BusinessVM> GetBusinessList(BusinessRequest request, List<int> staffIDs)
        {
            if (request == null || request.StartDate == null || request.EndDate == null || staffIDs == null) return null;
            using (var dbContext = new CrmDbContext())
            {
                //IQueryable<Customer> queryList = dbContext.Customers.Include("Staff").Include("Business");
                //return queryList.Where(p => (staffIDs.Contains(p.StaffID ?? 0) && p.Business.Count > 0)).ToList();

                //var list = from t1 in dbContext.Businesies
                //           join t2 in dbContext.Customers on new { Cus = t1.CustomerID == null ? 0 : t1.CustomerID.Value, Stf = t1.StaffID } equals new { Cus = t2.ID, Stf = t2.StaffId } //into left
                //           //from c in left.DefaultIfEmpty()
                //           where staffIDs.Contains(t2.StaffId == null ? -1 : t2.StaffId.Value)
                //                    && t1.CreateTime > request.StartDate.Value
                //                    && t1.CreateTime < request.EndDate.Value
                //           select new BusinessVM { Customer = t2, Business = t1 };
                var query = from a in dbContext.Customers.Include("Cooperations").Include("Staff").Include("City")
                            join b in dbContext.Business
                            on new { Cus = a.ID, Stf = a.StaffID } equals new { Cus = b.CustomerID == null ? 0 : b.CustomerID.Value, Stf = b.StaffID } into t
                            join c in dbContext.Provinces on (a.CityId == null ? 0 : a.City.ProvinceID) equals c.ID into x

                            where staffIDs.Contains(a.StaffID == null ? -1 : a.StaffID.Value) orderby a.ID descending
                            select new BusinessVM
                            {
                                //ParentBranch = GetParentBranch(dbContext, a.StaffID),
                                //RootBranch = GetRootBranch(dbContext, GetParentBranch(dbContext, a.StaffID)),
                                Customer = a,
                                Business = t.Where(p => (p.CreateTime > request.StartDate.Value && p.CreateTime < request.EndDate.Value)),
                                //Provienc = x.FirstOrDefault() == null ? "" : x.First().Name
                            };

                return query.ToPagedList(request.PageIndex, request.PageSize); ;
                //return list.OrderByDescending(u => u.Customer.ID).ToList();
                //var query=dbContext.Customers.GroupJoin(dbContext.Business,
                //                                        a=>new { Cus = a.ID , Stf = a.StaffId },
                //                                        b=>new { Cus = b.CustomerID == null ? 0 : b.CustomerID.Value, Stf = b.StaffID },
                //                                        (a,t)=>new 
                //                                            {
                //                                                ID=a.ID,
                //                                                Content=a.Name,
                //                                                UserIDs=t
                //                                            }).Where(p=>p.ID);

            }
        }
        private Branch GetParentBranch(CrmDbContext dbContext, int? staffid)
        {
            var query = from a in dbContext.Staffs
                        join b in dbContext.Branchs on a.BranchId equals b.ID
                        where a.ID == staffid
                        select b;

            if (query == null || query.Count() == 0)
            {
                return null;
            }
            else
            {
                int curID = query.FirstOrDefault().ID;
                Branch val = dbContext.Branchs.FirstOrDefault(p => p.ID == curID);
                return val;
            }
        }
        private Branch GetRootBranch(CrmDbContext dbContext, Branch branch)
        {
            if (branch != null)
            {
                return dbContext.Branchs.FirstOrDefault(p => p.ID == branch.ParentId);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<Business> GetBusinessList(BusinessRequest request, int staffID)
        {
            if (request == null || request.StartDate == null || request.EndDate == null || staffID < 0) return null;
            using (var dbContext = new CrmDbContext())
            {
                return dbContext.Business.Include("Customer").Where(p => (p.CreateTime > request.StartDate && p.CreateTime < request.EndDate && p.StaffID == staffID)).ToList();
            }
        }
        public void CreateBusiness(CreateBusinessEntity entity)
        {
            using (var dbContext = new CrmDbContext())
            {
                Business business = new Business { CreateTime = entity.CreateTime, StaffID = entity.StaffID, CustomerID = entity.CustomerID, Message = entity.Message, IsSpecial = entity.IsSpecial, PredictPayment = entity.PredictPayment, CurrentPayment = entity.PredictPayment };
                dbContext.Business.Where(p =>
                    (p.CreateTime == entity.CreateTime
                    && p.StaffID == entity.StaffID
                    && p.CustomerID == entity.CustomerID)).Delete();
                UpdateOrCreatePayment(-1, business.CreateTime.ToString("yyyyMM"), entity.CurrentPayment, entity.PredictPayment);
                dbContext.Insert<Business>(business);
            }
        }

        private void UpdateOrCreatePayment(int customerid, string during, double? curt, double? predict)
        {
            using (var dbContext = new CrmDbContext())
            {
                if (dbContext.Payments.Where(p => (p.CustomerID == customerid && p.Durring == during)).Count() > 0)
                {
                    Payment old = dbContext.Payments.FirstOrDefault(p => (p.CustomerID == customerid && p.Durring == during));
                    old.PredictPayment = predict;
                    old.CurrentPayment = curt;

                    dbContext.Update<Payment>(old);
                }
                else
                {
                    dbContext.Insert<Payment>(new Payment { Durring = during, CustomerID = customerid, CurrentPayment = curt, PredictPayment = predict });
                }
            }
        }
        public bool UpdateBusiness(Business entity)
        {
            bool ret = false;
            using (var dbContext = new CrmDbContext())
            {
                Business bss = dbContext.Update<Business>(entity);
                UpdateOrCreatePayment(bss.CustomerID.Value, bss.CreateTime.ToString("yyyyMM"), bss.CurrentPayment, bss.PredictPayment);
                ret = bss != null;
            }
            return ret;
        }
        public Business GetBusinessById(int businessID)
        {
            if (businessID > 0)
            {
                using (var dbContext = new CrmDbContext())
                {
                    Business business = dbContext.Business.Where(p => p.ID == businessID).FirstOrDefault();
                    if (business != null)
                    {
                        Payment pay = GetPayment(business.CustomerID.Value, business.CreateTime.ToString("yyyyMM"));
                        business.UpatePayment(pay);
                    }
                    return business;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        public Payment GetPayment(int customerid, string durring)
        {
            using (var dbContext = new CrmDbContext())
            {
                IEnumerable<Payment> cooperations = dbContext.FindAll<Payment>().ToList();
                return cooperations.FirstOrDefault(p => (p.CustomerID == customerid && p.Durring == durring));
            }
        }
        public IEnumerable<City> GetCityList(Request request = null)
        {
            request = request ?? new Request();
            using (var dbContext = new CrmDbContext())
            {
                IQueryable<City> citys = dbContext.Citys;
                return citys.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public IEnumerable<Area> GetAreaList(Request request = null)
        {
            request = request ?? new Request();
            using (var dbContext = new CrmDbContext())
            {
                IQueryable<Area> areas = dbContext.Areas;
                return areas.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }
        public IEnumerable<Province> GetProvinceList(Request request = null)
        {
            request = request ?? new Request();
            using (var dbContext = new CrmDbContext())
            {
                IQueryable<Province> areas = dbContext.Provinces;
                return areas.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }
        public IEnumerable<UserAnalysis> GetUserAnalysis(DateTime startDate, DateTime endDate)
        {
            using (var dbContext = new CrmDbContext())
            {
                var result = dbContext.VisitRecords.Where(d => d.VisitTime > startDate && d.VisitTime < endDate).GroupBy(r => r.Username).Select(g => new UserAnalysis { UserName = g.Key, VisitRecordCount = g.Count(), CustomerCount = g.Select(c => c.CustomerId).Distinct().Count() }).ToList();
                return result;
            }
        }

        public IEnumerable<VisitStatistics> GetVisitStatistics(DateTime startDate, DateTime endDate)
        {
            using (var dbContext = new CrmDbContext())
            {
                var result = dbContext.VisitRecords.Where(d => d.VisitTime > startDate && d.VisitTime < endDate).GroupBy(r => r.VisitTime.Hour).Select(g => new VisitStatistics { Hour = g.Key, VisitRecordCount = g.Count(), VisitCount = g.Where(c => c.VisitWay == 2).Count(), TelCount = g.Where(c => c.VisitWay == 1).Count() }).ToList();
                return result;
            }
        }
    }
}
