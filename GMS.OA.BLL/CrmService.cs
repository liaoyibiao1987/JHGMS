using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMS.Crm.Contract;
using GMS.Framework.Utility;
//using System.Data.Entity.Core;
using GMS.Framework.Contract;
using EntityFramework.Extensions;
using GMS.Core.Cache;
using GMS.OA.Contract;
using GMS.OA.Contract.Model;
using System.Data.Entity.Core.Objects;
using GMS.OA.DAL;
using GMS.Core.Log;

namespace GMS.Crm.BLL
{
    public class CrmService : ICrmService
    {
        #region Project CURD
        public Project GetProject(int id)
        {
            using (var dbContext = new CRMOAContext())
            {
                return dbContext.Find<Project>(id);
            }
        }

        public IEnumerable<Project> GetProjectList(ProjectRequest request = null)
        {
            return null;
            //request = request ?? new ProjectRequest();
            //using (var dbContext = new CRMOAContext())
            //{
            //    IQueryable<Project> projects = dbContext.Projects;

            //    if (!string.IsNullOrEmpty(request.Name))
            //        projects = projects.Where(u => u.Name.Contains(request.Name));

            //    return projects.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            //}
        }

        public void SaveProject(Project project)
        {
            using (var dbContext = new CRMOAContext())
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
            //using (var dbContext = new CRMOAContext())
            //{
            //    dbContext.Projects.Where(u => ids.Contains(u.ID)).Delete();
            //}
        }
        #endregion

        #region Customer CURD
        public Customer GetCustomer(int id)
        {
            using (var dbContext = new CRMOAContext())
            {
                return dbContext.Customers.Include("Cooperations").FirstOrDefault(p => p.ID == id);
            }
        }

        public IEnumerable<Customer> GetCustomerList(List<int> staffids, CustomerRequest request = null)
        {
            request = request ?? new CustomerRequest();
            using (var dbContext = new CRMOAContext())
            {
                //IQueryable<Customer> queryList = dbContext.Customers.Include("VisitRecords").Include("Staff");
                //IQueryable<Customer> queryList = dbContext.Customers.Include("Cooperations").Include("Staff").Include("City").Where(p => staffids.Contains(p.StaffID.Value));
                IQueryable<Customer> queryList = dbContext.Customers.Where(p => staffids.Contains(p.StaffID.Value));
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
            using (var dbContext = new CRMOAContext())
            {
                if (customer.ID > 0)
                {
                    Customer old = dbContext.Customers.AsNoTracking().Where(p => p.ID == customer.ID).FirstOrDefault();
                    Log4NetHelper.Debug(LoggerType.WebExceptionLog, old, null);
                    //if (dbContext.Customers.Any(c => c.Tel == customer.Tel && c.ID != customer.ID))
                    //    throw new BusinessException("Tel", "已存在此电话的客户！");
                    dbContext.Update<Customer>(customer);

                    List<Cooperations> cooperations = dbContext.Cooperations.Where(r => customer.CooperationsIds.Contains(r.ID)).ToList();
                    customer.Cooperations = cooperations;
                    dbContext.SaveChanges();
                }
                else
                {
                    //if (dbContext.Customers.Any(c => c.Tel == customer.Tel))
                    //    throw new BusinessException("Tel", "已存在此电话的客户！");

                    customer = dbContext.Insert<Customer>(customer);
                }

                Log4NetHelper.Debug(LoggerType.WebExceptionLog, customer, null);
            }
        }

        public void DeleteCustomer(List<int> ids)
        {
            using (var dbContext = new CRMOAContext())
            {
                dbContext.Customers.Where(u => ids.Contains(u.ID)).Delete();
            }
        }
        #endregion

        #region VisitRecord CURD
        public VisitRecord GetVisitRecord(int id)
        {
            using (var dbContext = new CRMOAContext())
            {
                return dbContext.Find<VisitRecord>(id);
            }
        }

        public IEnumerable<VisitRecord> GetVisitRecordList(VisitRecordRequest request = null)
        {
            request = request ?? new VisitRecordRequest();
            using (var dbContext = new CRMOAContext())
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
            using (var dbContext = new CRMOAContext())
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
            using (var dbContext = new CRMOAContext())
            {
                dbContext.VisitRecords.Where(u => ids.Contains(u.ID)).Delete();
            }
        }
        #endregion

        #region Business
        private List<int> CopKinds(string input)
        {
            List<int> output = new List<int>();
            if (string.IsNullOrEmpty(input) == false)
            {
                var tmp = input.Split(',').ToList();
                int peer = 0;
                foreach (var item in tmp)
                {
                    int.TryParse(item, out peer);
                    output.Add(peer);
                }
            }
            return output;
        }
        public List<BusinessVM> GetBusinessDownload(BusinessPostParameter parm, IEnumerable<int> staffids)
        {
            if (parm == null || parm.startdate == null || parm.enddate == null || parm == null) return null;
            string perpaymentmonth = parm.enddate.Value.ToString("yyyyMM");

            FilterLeaders(parm, ref staffids);
            using (var dbContext = new CRMOAContext())
            {
                var fristquery = dbContext.Customers.AsQueryable();
                var fristbusiness = dbContext.Business.Where(p => (p.CreateTime >= parm.startdate.Value && p.CreateTime < parm.enddate.Value && staffids.Contains(p.StaffID == null ? -1 :
                    p.StaffID.Value))).OrderBy(aa => aa.CreateTime);

                GetFilter(parm, ref fristquery);

                var query = (
                            from a in fristquery
                            join b in fristbusiness on new { Cus = a.ID } equals new { Cus = b.CustomerID == null ? 0 : b.CustomerID.Value } into t
                            join d in dbContext.Staffs on (a.StaffID) equals d.ID into y
                            join f in dbContext.Payments on a.ID equals f.CustomerID into zz
                            where staffids.Contains(a.StaffID == null ? -1 : a.StaffID.Value)
                            orderby a.ID descending
                            select new BusinessVM
                            {
                                Customer = a,
                                Business = t,
                                Staff = y.FirstOrDefault(),
                                PerPayment = (zz.FirstOrDefault(p => p.Durring == perpaymentmonth) == null && zz.FirstOrDefault(p => p.Durring == perpaymentmonth).PredictPayment.HasValue == true) ? "" : zz.FirstOrDefault(p => p.Durring == perpaymentmonth).PredictPayment.ToString()
                            });
                //这句必须加，不加不知道什么鬼了 必须用Customer.Name排序
                //query = query.OrderByDescending(p => p.Customer.Name);
                if (parm.HasBusiness.HasValue == true)
                {
                    if (parm.HasBusiness.Value == true)
                    {
                        query = query.Where(p => p.Business.Count() > 0);
                    }
                    else
                    {
                        query = query.Where(p => (p.Business.Count() == 0));
                    }
                }

                if (parm.EnumPosition.HasValue == true)
                {
                    query = query.Where(p => p.Staff.Position == (parm.EnumPosition.HasValue ? parm.EnumPosition.Value : p.Staff.Position));
                }

                return query.ToList();
            }
        }
        public PagedList<BusinessVM> GetBusinessList(BusinessPostParameter parm, IEnumerable<int> staffids)
        {
            if (parm == null || parm.startdate == null || parm.enddate == null || parm == null) return null;
            //传入的时候多算了一天
            string perpaymentmonth = parm.enddate.Value.AddDays(-1).ToString("yyyyMM");

            FilterLeaders(parm, ref staffids);
            using (var dbContext = new CRMOAContext())
            {
                var fristquery = dbContext.Customers.AsQueryable();
                var fristbusiness = dbContext.Business.Where(p => (p.CreateTime >= parm.startdate.Value && p.CreateTime < parm.enddate.Value && staffids.Contains(p.StaffID == null ? -1 :
                    p.StaffID.Value))).OrderBy(aa => aa.CreateTime);
                GetFilter(parm, ref fristquery);

                var query = (//from a in dbContext.Customers.Include("Cooperations").Include("Staff").Include("City")
                            from a in fristquery.AsQueryable<Customer>()
                            join b in fristbusiness on new { Cus = a.ID } equals new { Cus = b.CustomerID == null ? 0 : b.CustomerID.Value } into t
                            join c in dbContext.Provinces on (a.CityId == null ? 0 : a.City.ProvinceID) equals c.ID into x



                            join d in dbContext.Staffs on (a.StaffID) equals d.ID into y
                            join e in dbContext.Citys on (a.CityId) equals e.ID into z
                            join f in dbContext.Payments on a.ID equals f.CustomerID into zz
                            where staffids.Contains(a.StaffID == null ? -1 : a.StaffID.Value)
                            orderby a.ID descending
                            select new BusinessVM
                            {
                                Customer = a,
                                Business = t,
                                Provienc = x.FirstOrDefault() == null ? "" : x.FirstOrDefault().Name,
                                CityName = z.FirstOrDefault() == null ? "" : z.FirstOrDefault().Name,
                                Staff = y.FirstOrDefault(),
                                PerPayment = (zz.FirstOrDefault(p => p.Durring == perpaymentmonth) == null && zz.FirstOrDefault(p => p.Durring == perpaymentmonth).PredictPayment.HasValue == true) ? "" : zz.FirstOrDefault(p => p.Durring == perpaymentmonth).PredictPayment.ToString()
                            });

                //query = query.OrderByDescending(u => u.Customer.ID);

                GetSortting(parm, ref query);
                //GetFilter(parm, ref query);

                if (parm.HasBusiness.HasValue == true)
                {
                    if (parm.HasBusiness.Value == true)
                    {
                        query = query.Where(p => p.Business.Count() > 0);
                    }
                    else
                    {
                        query = query.Where(p => (p.Business.Count() == 0));
                    }
                }

                if (parm.EnumPosition.HasValue == true)
                {
                    query = query.Where(p => p.Staff.Position == (parm.EnumPosition.HasValue ? parm.EnumPosition.Value : p.Staff.Position));
                }



                return query.ToPagedList(parm.startpage, parm.length);
            }
        }

        private void FilterLeaders(BusinessPostParameter parm, ref IEnumerable<int> staffids)
        {
            if (parm.Leaders.HasValue == false && parm.Suboffice.HasValue == false)
            {
                return;
            }
            else
            {
                using (var dbContext = new CRMOAContext())
                {
                    if (parm.Leaders.HasValue == true)
                    {
                        var tempquery = dbContext.Branchs.Where(p => (p.ID == parm.Leaders.Value || p.ParentId == parm.Leaders.Value)).Select(x => x.ID);
                        IEnumerable<int> tempids = dbContext.Staffs.Where(p => tempquery.Contains(p.BranchId.HasValue ? p.BranchId.Value : -1)).Select(x => x.ID);
                        staffids = staffids.Where(p => tempids.Contains(p)).ToList();
                        //query = query.Where(p => tempquery.Contains(p.Staff.BranchId.HasValue ? p.Staff.BranchId.Value : -1));
                    }
                    if (parm.Suboffice.HasValue == true)
                    {
                        var tempquery = dbContext.Branchs.Where(p => (p.ID == parm.Suboffice.Value)).Select(x => x.ID).FirstOrDefault();
                        IEnumerable<int> tempids = dbContext.Staffs.Where(p => tempquery == p.BranchId).Select(x => x.ID);

                        staffids = staffids.Where(p => tempids.Contains(p)).ToList();
                        //query = query.Where(p => tempquery == p.Staff.BranchId);
                    }
                }
            }

        }
        private void GetFilter(BusinessPostParameter parm, ref IQueryable<Customer> query)
        {

            if (parm.SelectCity.HasValue == true)
            {
                query = query.Where(p => p.CityId == (parm.SelectCity.HasValue ? parm.SelectCity.Value : p.CityId));
            }
            if (parm.StaffID.HasValue == true)
            {
                query = query.Where(p => p.Staff.ID == (parm.StaffID.HasValue ? parm.StaffID.Value : p.Staff.ID));
            }
            if (parm.Category.HasValue == true)
            {
                query = query.Where(p => p.Category == (parm.Category.HasValue ? parm.Category.Value : p.Category));
            }
            if (parm.Channel.HasValue == true)
            {
                query = query.Where(p => p.Channel.Contains(parm.Channel.HasValue ? parm.Channel.ToString() : p.Channel));
            }
            if (parm.BusinessType.HasValue == true)
            {
                query = query.Where(p => p.BusinessType.Contains((parm.BusinessType.HasValue ? parm.BusinessType.Value.ToString() : p.BusinessType)));
            }
            if (parm.CustomerId.HasValue == true)
            {
                query = query.Where(p => p.ID == (parm.CustomerId.HasValue ? parm.CustomerId.Value : p.ID));
            }
            if (parm.ChainType.HasValue == true)
            {
                query = query.Where(p => p.ChainType == (parm.ChainType.HasValue ? parm.ChainType.Value : p.ChainType));
            }
            if (parm.CooperationOrNot.HasValue == true)
            {
                query = query.Where(p => p.CooperationOrNot == (parm.CooperationOrNot.HasValue ? parm.CooperationOrNot.Value : p.CooperationOrNot));
            }
            if (parm.CooperationKinds.HasValue == true)
            {
                query = query.Where(p => p.CooperationKinds.Contains(parm.CooperationKinds.Value.ToString()));
            }
        }

        private void GetSortting(BusinessPostParameter parm, ref IQueryable<BusinessVM> query)
        {
            OrderParm order = parm.order.FirstOrDefault();
            if (order != null)
            {
                switch (order.column)
                {
                    case 5:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.CityName);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.CityName);
                        }
                        break;
                    case 6:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Customer.Name);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Customer.Name);
                        }
                        break;
                    case 7:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Customer.Category);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Customer.Category);
                        }
                        break;
                    case 8:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Customer.Contacter);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Customer.Contacter);
                        }
                        break;
                    case 9:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Customer.Tel);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Customer.Tel);
                        }
                        break;
                    case 10:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Customer.Channel);
                        }
                        else
                        {
                            query.OrderByDescending(p => p.Customer.Channel);
                        }
                        break;
                    case 11:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Customer.BusinessType);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Customer.BusinessType);
                        }
                        break;
                    case 12:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Customer.ChainCount);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Customer.ChainCount);
                        }
                        break;
                    case 13:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Customer.ChainType);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Customer.ChainType);
                        }
                        break;
                    case 14:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Customer.StaffID);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Customer.StaffID);
                        }
                        break;
                    case 15:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Staff.Position);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Staff.Position);
                        }
                        break;
                    case 16:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Customer.UnitName);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Customer.UnitName);
                        }
                        break;
                    case 17:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Customer.CooperationOrNot);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Customer.CooperationOrNot);
                        }
                        break;
                    case 18:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Customer.CooperationKinds);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Customer.CooperationKinds);
                        }
                        break;
                    case 19:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.Business.Count());
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.Business.Count());
                        }
                        break;
                    case 20:
                        if (order.dir == "asc")
                        {
                            query = query.OrderBy(p => p.PerPayment);
                        }
                        else
                        {
                            query = query.OrderByDescending(p => p.PerPayment);
                        }
                        break;
                    default:
                        break;
                }
            }

        }
        //private Branch GetParentBranch(CRMOAContext dbContext, int? staffid)
        //{
        //    var query = from a in dbContext.Staffs
        //                join b in dbContext.Branchs on a.BranchId equals b.ID
        //                where a.ID == staffid
        //                select b;

        //    if (query == null || query.Count() == 0)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        int curID = query.FirstOrDefault().ID;
        //        Branch val = dbContext.Branchs.FirstOrDefault(p => p.ID == curID);
        //        return val;
        //    }
        //}
        //private Branch GetRootBranch(CRMOAContext dbContext, Branch branch)
        //{
        //    if (branch != null)
        //    {
        //        return dbContext.Branchs.FirstOrDefault(p => p.ID == branch.ParentId);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public IEnumerable<Business> GetBusinessList(BusinessRequest request, int staffID)
        {
            if (request == null || request.StartDate == null || request.EndDate == null || staffID < 0) return null;
            using (var dbContext = new CRMOAContext())
            {
                return dbContext.Business.Include("Customer").Where(p => (p.CreateTime > request.StartDate && p.CreateTime < request.EndDate && p.StaffID == staffID)).ToList();
            }
        }
        public void CreateBusiness(CreateBusinessEntity entity)
        {
            using (var dbContext = new CRMOAContext())
            {
                Business business = new Business { CreateTime = entity.CreateTime, StaffID = entity.StaffID, CustomerID = entity.CustomerID, Message = entity.Message, IsSpecial = entity.IsSpecial, PredictPayment = entity.PredictPayment, CurrentPayment = entity.PredictPayment };
                UpdateOrCreatePayment(entity.CustomerID.Value, business.CreateTime.ToString("yyyyMM"), entity.CurrentPayment, entity.PredictPayment);
                if (string.IsNullOrEmpty(entity.Message) == false)
                {
                    dbContext.Business.Where(p => (p.CreateTime == entity.CreateTime
                        && p.StaffID == entity.StaffID
                        && p.CustomerID == entity.CustomerID)).Delete();
                    dbContext.Insert<Business>(business);
                }
            }
        }

        private void UpdateOrCreatePayment(int customerid, string during, double? curt, double? predict)
        {
            using (var dbContext = new CRMOAContext())
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
            using (var dbContext = new CRMOAContext())
            {
                if (string.IsNullOrEmpty(entity.Message) == false)
                {
                    dbContext.Business.Where(p => (p.CreateTime == entity.CreateTime
                        && p.StaffID == entity.StaffID
                        && p.CustomerID == entity.CustomerID && p.ID != entity.ID)).Delete();

                    Business bss = dbContext.Update<Business>(entity);
                    UpdateOrCreatePayment(bss.CustomerID.Value, bss.CreateTime.ToString("yyyyMM"), bss.CurrentPayment, bss.PredictPayment);
                    ret = bss != null;
                }
                else
                {
                    UpdateOrCreatePayment(entity.CustomerID.Value, entity.CreateTime.ToString("yyyyMM"), entity.CurrentPayment, entity.PredictPayment);
                    return dbContext.Business.Where(p => (p.ID == entity.ID)).Delete() > 0;
                }

            }
            return ret;
        }

        public List<V_MonthBusiness> GetMonthAnalysis()
        {
            using (var dbContext = new CRMOAContext())
            {
                return dbContext.V_MonthBusiness.ToList();
            }
        }
        public List<V_QuarterBusiness> GetQuarterAnalysis()
        {
            using (var dbContext = new CRMOAContext())
            {
                return dbContext.V_QuarterBusiness.ToList();
            }
        }
        public List<V_YearBusiness> GetYearAnalysis()
        {
            using (var dbContext = new CRMOAContext())
            {
                return dbContext.V_YearBusiness.ToList();
            }
        }


        public bool ModifyStaffs(List<int> customersID, int newstaffID)
        {
            bool ret = false;
            using (var dbContext = new CRMOAContext())
            {
                try
                {
                    var query = dbContext.Customers.Where(p => customersID.Contains(p.ID)).Update(c => new Customer { StaffID = newstaffID });
                    string operaterName = WCFContext.Current.Operater.Name;
                    var tempEntity = new { OlderStaff = customersID, NewStaff = newstaffID };
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(tempEntity);
                    dbContext.AuditLogger.WriteLog(0, operaterName, "ModifyStaffs", "Staffs", "ModifyStaffs", json);
                    dbContext.SaveChanges();
                    ret = true;
                }
                catch (Exception ss)
                {
                    ret = false;
                }

            }
            return ret;
        }


        public Business GetBusinessById(int businessID)
        {
            if (businessID > 0)
            {
                using (var dbContext = new CRMOAContext())
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
            using (var dbContext = new CRMOAContext())
            {
                IEnumerable<Payment> cooperations = dbContext.FindAll<Payment>().ToList();
                return cooperations.FirstOrDefault(p => (p.CustomerID == customerid && p.Durring == durring));
            }
        }
        public IEnumerable<City> GetCityList(Request request = null)
        {
            request = request ?? new Request();
            using (var dbContext = new CRMOAContext())
            {
                IQueryable<City> citys = dbContext.Citys.Include("Province");
                return citys.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public IEnumerable<Area> GetAreaList(Request request = null)
        {
            request = request ?? new Request();
            using (var dbContext = new CRMOAContext())
            {
                IQueryable<Area> areas = dbContext.Areas;
                return areas.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }
        public IEnumerable<Province> GetProvinceList(Request request = null)
        {
            request = request ?? new Request();
            using (var dbContext = new CRMOAContext())
            {
                IQueryable<Province> areas = dbContext.Provinces.Include("Citys");
                return areas.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }
        public IEnumerable<UserAnalysis> GetUserAnalysis(DateTime startDate, DateTime endDate)
        {
            using (var dbContext = new CRMOAContext())
            {
                var result = dbContext.VisitRecords.Where(d => d.VisitTime > startDate && d.VisitTime < endDate).GroupBy(r => r.Username).Select(g => new UserAnalysis { UserName = g.Key, VisitRecordCount = g.Count(), CustomerCount = g.Select(c => c.CustomerId).Distinct().Count() }).ToList();
                return result;
            }
        }

        public IEnumerable<VisitStatistics> GetVisitStatistics(DateTime startDate, DateTime endDate)
        {
            using (var dbContext = new CRMOAContext())
            {
                var result = dbContext.VisitRecords.Where(d => d.VisitTime > startDate && d.VisitTime < endDate).GroupBy(r => r.VisitTime.Hour).Select(g => new VisitStatistics { Hour = g.Key, VisitRecordCount = g.Count(), VisitCount = g.Where(c => c.VisitWay == 2).Count(), TelCount = g.Where(c => c.VisitWay == 1).Count() }).ToList();
                return result;
            }
        }
    }
}
