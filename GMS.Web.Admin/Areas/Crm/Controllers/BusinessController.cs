using GMS.Account.Contract;
using GMS.Crm.Contract;
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
            IEnumerable<BusinessVM> list = CrmService.GetBusinessList(rquester, new List<int> { 1, 2 });
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
            IEnumerable<BusinessVM> list = CrmService.GetBusinessList(req, new List<int> { 1, 2 });
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

        public ActionResult Edit(DateTime start)
        {
            ViewData.Add("start", start.ToString("yyyy-MM-dd"));

            var request = new CustomerRequest();
            request.Customer.StaffID = this.UserContext.LoginInfo.StaffID;
            var customerList = this.CrmService.GetCustomerList(request).ToList();
            customerList.ForEach(c => c.Name = string.Format("{0}({1})", c.Name, c.Tel));
            ViewData.Add("CustomerId", new SelectList(customerList, "Id", "Name", ""));

            return View();
        }

        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            //var model = this.CrmService.GetCustomer(id);
            //this.TryUpdateModel<Customer>(model);

            //try
            //{
            //    this.CrmService.SaveCustomer(model);
            //}
            //catch (BusinessException e)
            //{
            //    this.ModelState.AddModelError(e.Name, e.Message);
            //    this.RenderMyViewData(model);
            //    return View("Edit", model);
            //}

            return this.RefreshParent();
        }



        [HttpPost]
        public JsonResult GetBusiness(DateTime dstart, DateTime dend)
        {
            BusinessRequest rquester = new BusinessRequest();
            rquester.StartDate = dstart;
            rquester.EndDate = dend;
            IEnumerable<Business> list = CrmService.GetBusinessList(rquester, 1);
            return Json(list);
        }

        private void RenderMyViewData(BusinessRequest model)
        {
            ViewData.Add("startDate", (model == null || model.StartDate == null) ? DateTime.Now.AddMonths(-1).Subtract(DateTime.Now.TimeOfDay) : model.StartDate.Value);
            ViewData.Add("endDate", (model == null || model.EndDate == null) ? DateTime.Now.Subtract(DateTime.Now.TimeOfDay) : model.EndDate.Value);

        }

        private void RenderMyViewData()
        {

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
