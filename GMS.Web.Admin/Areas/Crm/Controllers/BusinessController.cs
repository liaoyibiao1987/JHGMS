﻿using GMS.Account.Contract;
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

    [Permission(EnumBusinessPermission.CrmManage_Customer)]

    public class BusinessController : AdminControllerBase
    {
        //
        // GET: /Crm/Business/
        public ActionResult Index(BusinessRequest rquester)
        {
            RenderMyViewData(rquester);
            rquester.StartDate = DateTime.Now.AddMonths(-1);
            rquester.EndDate = DateTime.Now.AddMonths(1);
            int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
            IEnumerable<BusinessVM> list = CrmService.GetBusinessList(rquester, GetCurrentUserStaffs(currentstaffid));
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
            int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
            IEnumerable<BusinessVM> list = CrmService.GetBusinessList(req, GetCurrentUserStaffs(currentstaffid));
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

        [HttpPost]
        public JsonResult GetBusiness(DateTime dstart, DateTime dend)
        {
            BusinessRequest rquester = new BusinessRequest();
            rquester.StartDate = dstart;
            rquester.EndDate = dend;
            IEnumerable<Business> list = CrmService.GetBusinessList(rquester, UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1);
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
