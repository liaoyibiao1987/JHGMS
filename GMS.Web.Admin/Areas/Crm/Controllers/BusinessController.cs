using GMS.Account.Contract;
using GMS.Crm.Contract;
using GMS.Framework.Contract;
using GMS.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GMS.Web.Admin.Areas.Crm.Controllers
{

    [Permission(EnumBusinessPermission.CrmManage_VisitRecord)]
    public class BusinessController : AdminControllerBase
    {
        //
        // GET: /Crm/Business/
        public ActionResult Index(BusinessRequest rquester)
        {
            RenderMyViewData(rquester);
            rquester.StartDate = DateTime.Now.AddMonths(-1);
            rquester.EndDate = DateTime.Now.AddMonths(1);
            int currentstaffid = this.UserContext.LoginInfo.StaffID.Value;
            IEnumerable<BusinessVM> list = CrmService.GetBusinessList(rquester, new List<int> { 1, 2, currentstaffid });
            return View(list);
        }

        [HttpPost]
        public ActionResult Index(DateTime start, DateTime end)
        {
            BusinessRequest req = new BusinessRequest();
            req.StartDate = start;
            req.EndDate = end;
            this.ModelState.Clear();
            RenderMyViewData(req);
            int currentstaffid = this.UserContext.LoginInfo.StaffID.Value;
            IEnumerable<BusinessVM> list = CrmService.GetBusinessList(req, new List<int> { 1, 2, currentstaffid });
            return View(list);
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
            ViewData.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd"));
            RenderMyViewData(-1);

            return View("Edit");
        }

        public ActionResult AddByDate(DateTime CreateDate)
        {
            ViewData.Add("CreateDate", CreateDate.ToString("yyyy-MM-dd"));
            RenderMyViewData(-1);

            return View("Edit");
        }
        public ActionResult AddByCustomerId(int CustomerId)
        {
            ViewData.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd"));
            RenderMyViewData(CustomerId);
            return View("Edit");
        }

        [HttpPost]
        public ActionResult AddByDate(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                CreateBusinessEntity entity = new CreateBusinessEntity();
                this.TryUpdateModel<CreateBusinessEntity>(entity);
                entity.StaffID = this.UserContext.LoginInfo.StaffID;
                try
                {
                    this.CrmService.CreateBusiness(entity);
                }
                catch (BusinessException e)
                {
                    this.ModelState.AddModelError(e.Name, e.Message);
                    ViewData.Add("CreateDate", entity.CreateDate.ToString("yyyy-MM-dd"));
                    RenderMyViewData(-1);
                    return View("Edit");
                    //this.RenderMyViewData(model);
                }
            }
            return this.RefreshParent();
        }

        [HttpPost]
        public ActionResult EditByID(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                CreateBusinessEntity entity = new CreateBusinessEntity();
                this.TryUpdateModel<CreateBusinessEntity>(entity);
                entity.StaffID = this.UserContext.LoginInfo.StaffID;
                try
                {
                    this.CrmService.CreateBusiness(entity);
                }
                catch (BusinessException e)
                {
                    this.ModelState.AddModelError(e.Name, e.Message);
                    ViewData.Add("CreateDate", entity.CreateDate.ToString("yyyy-MM-dd"));
                    RenderMyViewData(-1);
                    return View("Edit");
                    //this.RenderMyViewData(model);
                }
            }
            return this.RefreshParent();
        }



        [HttpPost]
        public JsonResult GetBusiness(DateTime dstart, DateTime dend)
        {
            BusinessRequest rquester = new BusinessRequest();
            rquester.StartDate = dstart;
            rquester.EndDate = dend;
            IEnumerable<Business> list = CrmService.GetBusinessList(rquester, UserContext.LoginInfo.StaffID.Value);
            return Json(list);
        }

        private void RenderMyViewData(BusinessRequest model)
        {
            ViewData.Add("startDate", (model == null || model.StartDate == null) ? DateTime.Now.AddMonths(-1).Subtract(DateTime.Now.TimeOfDay) : model.StartDate.Value);
            ViewData.Add("endDate", (model == null || model.EndDate == null) ? DateTime.Now.Subtract(DateTime.Now.TimeOfDay) : model.EndDate.Value);

        }

        private void RenderMyViewData(int customid)
        {
            var request = new CustomerRequest();
            request.Customer.StaffID = this.UserContext.LoginInfo.StaffID;
            var customerList = this.CrmService.GetCustomerList(request).ToList();
            customerList.ForEach(c => c.Name = string.Format("{0}({1})", c.Name, c.Tel));
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
