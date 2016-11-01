using GMS.Account.Contract;
using GMS.Crm.Contract;
using GMS.Framework.Contract;
using GMS.Framework.Utility;
using GMS.OA.Contract;
using GMS.OA.Contract.Model;
using GMS.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GMS.Web.Admin.Areas.Crm.Controllers
{

    [Permission(EnumBusinessPermission.CrmManage_Customer)]

    public class BusinessController : AdminControllerBase
    {
        //
        // GET: /Crm/Business/
        public ActionResult Index()
        {
            //RenderMyViewData(rquester);
            RenderMyViewData();
            //rquester.StartDate = DateTime.Now.AddDays(-7);
            //rquester.EndDate = DateTime.Now;
            //int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
            //IEnumerable<BusinessVM> list = CrmService.GetBusinessList(rquester, GetCurrentUserStaffs(currentstaffid));
            //return View(list);
            return View();
        }
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
        [HttpPost]
        public JsonResult GetAllBranch()
        {
            IEnumerable<Branch> all = OAService.GetBranchList();
            return Json(all);
        }
        [HttpPost]
        public JsonResult GetBusinessByAjax(BusinessPostParameter aoData)
        {
            //Fetch.Post("");
            int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
            PagedList<BusinessVM> list = CrmService.GetBusinessList(aoData, GetCurrentUserStaffs(currentstaffid));
            return new JsonResult()
            {
                Data = new
                {
                    iDraw = aoData.draw,
                    iTotalRecords = list.TotalItemCount,
                    iTotalDisplayRecords = list.TotalItemCount,
                    Data = list
                },
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        private void RenderMyViewData()
        {
            ViewData.Add("Category", new SelectList(EnumHelper.GetItemValueList<EnumCategory>(), "Key", "Value", 0));
            ViewData.Add("Channel", new SelectList(EnumHelper.GetItemValueList<EnumChannel>(), "Key", "Value", 0));
            ViewData.Add("BusinessType", new SelectList(EnumHelper.GetItemValueList<EnumBusinessType>(), "Key", "Value", 0));
            ViewData.Add("EnumPosition", new SelectList(EnumHelper.GetItemValueList<EnumPosition>(), "Key", "Value", 0));

            int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
            var customerList = this.CrmService.GetCustomerList(GetCurrentUserStaffs(currentstaffid)).ToList();
            customerList.ForEach(c => c.Name = string.Format("{0}({1})", c.Name, c.Contacter));
            ViewData.Add("CustomerId", new SelectList(customerList, "Id", "Name"));

            List<Staff> liststaff = GetCurrentUserStaffs();
            ViewData.Add("Staffs", new SelectList(liststaff.Select(c => new { Id = c.ID, Name = c.Name }), "Id", "Name", UserContext.LoginInfo.StaffID));
            RenderLeader();
        }
        private void RenderLeader()
        {
            BranchRequest br = new BranchRequest { PageIndex = 0, PageSize = 64 };
            IEnumerable<Branch> selected = OAService.GetBranchList(br);
            List<Branch> leader = new List<Branch>();
            List<Branch> suboffice = new List<Branch>();
            if (selected != null && selected.Count() > 0)
            {
                foreach (var item in selected)
                {
                    if (item.ID == 0) continue;
                    if (selected.Where(p => p.ParentId == item.ID).Count() > 0)
                    {
                        leader.Add(item);
                    }
                    else
                    {
                        suboffice.Add(item);
                    }
                }
            }
            ViewData.Add("Leaders", new SelectList(leader.Select(c => new { Id = c.ID, Name = c.Name }), "Id", "Name"));
            ViewData.Add("Suboffice", new SelectList(suboffice.Select(c => new { Id = c.ID, Name = c.Name }), "Id", "Name"));
        }

        /// <summary>
        /// 日历菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult EditByID(int id)
        {
            ModelState.Clear();
            Business business = CrmService.GetBusinessById(id);
            if (business != null)
            {
                RenderMyViewData(business.CustomerID.Value);
            }
            else
            {
                RenderMyViewData(-1);
            }
            return View("Edit", business);
        }

        public ActionResult AddByDate(DateTime CreateDate)
        {
            Business business = new Business();
            business.CreateTime = CreateDate;
            RenderMyViewData(-1);
            return View("Edit", business);
        }
        public ActionResult AddByCustomerId(int CustomerId)
        {
            Business business = new Business();
            business.CreateTime = DateTime.Now;
            RenderMyViewData(CustomerId);
            return View("Edit", business);
        }

        [HttpPost]
        public ActionResult AddByDate(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                CreateBusinessEntity entity = new CreateBusinessEntity();
                this.TryUpdateModel<CreateBusinessEntity>(entity);
                entity.StaffID = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
                try
                {
                    this.CrmService.CreateBusiness(entity);
                }
                catch (BusinessException e)
                {
                    this.ModelState.AddModelError(e.Name, e.Message);
                    RenderMyViewData(-1);
                    return View("Edit");
                }
            }
            return this.RefreshParent();
        }

        [HttpPost]
        public ActionResult EditByID(int ID, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                Business business = CrmService.GetBusinessById(ID);
                if (business != null)
                {
                    this.TryUpdateModel<Business>(business);
                    business.StaffID = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
                    try
                    {
                        this.CrmService.UpdateBusiness(business);
                    }
                    catch (BusinessException e)
                    {
                        this.ModelState.AddModelError(e.Name, e.Message);
                        RenderMyViewData(-1);
                        return View("Edit", business);
                    }
                }

            }
            return this.RefreshParent();
        }

        [HttpPost]
        public ActionResult AddByCustomerId(int CustomerId, FormCollection collection)
        {
            Customer customer = CrmService.GetCustomer(CustomerId);

            if (ModelState.IsValid && customer != null)
            {
                CreateBusinessEntity business = new CreateBusinessEntity();
                this.TryUpdateModel<CreateBusinessEntity>(business);
                business.StaffID = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
                try
                {
                    this.CrmService.CreateBusiness(business);
                }
                catch (BusinessException e)
                {
                    this.ModelState.AddModelError(e.Name, e.Message);
                    RenderMyViewData(-1);
                    return View("Edit");
                }
            }

            return this.RefreshParent();
        }
        [HttpPost]
        public ActionResult MoveEvent(int ID, DateTime to)
        {
            Business business = CrmService.GetBusinessById(ID);
            business.CreateTime = to;
            CrmService.UpdateBusiness(business);
            return new EmptyResult();
        }
        /// <summary>
        /// 业务员获取自己一个月的业务信息。
        /// </summary>
        /// <param name="dstart"></param>
        /// <param name="dend"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetBusiness(DateTime dstart, DateTime dend)
        {
            BusinessRequest rquester = new BusinessRequest();
            rquester.StartDate = dstart;
            rquester.EndDate = dend;
            IEnumerable<Business> list = CrmService.GetBusinessList(rquester, UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1);
            return Json(list);
        }
        private void RenderMyViewData(int customid)
        {
            var request = new CustomerRequest();
            int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
            request.Customer.StaffID = currentstaffid;

            var customerList = this.CrmService.GetCustomerList(GetCurrentUserStaffs(currentstaffid), request).ToList();

            customerList.ForEach(c => c.Name = string.Format("{0}({1})", c.Name, c.Tel));
            customid = customerList.Exists(p => p.ID == customid) ? customid : -1;

            ViewData.Add("CustomerId", new SelectList(customerList, "Id", "Name", customid));
        }


        public Dictionary<int, City> CityDic
        {
            get
            {
                return AdminCacheContext.Current.CityDic;
            }
        }

        public Dictionary<int, Area> AreaDic
        {
            get
            {
                return AdminCacheContext.Current.AreaDic;
            }
        }
    }
}
