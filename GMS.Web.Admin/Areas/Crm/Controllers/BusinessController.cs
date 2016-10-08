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
        public JsonResult Index(DateTime start, DateTime end)
        {
            BusinessRequest req = new BusinessRequest();
            req.StartDate = start;
            req.EndDate = end;
            RenderMyViewData(req);

            IEnumerable<BusinessVM> list = CrmService.GetBusinessList(req, new List<int> { 1, 2 });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private void RenderMyViewData(BusinessRequest model)
        {
            ViewData.Add("startDate", (model == null || model.StartDate == null) ? DateTime.Now.AddMonths(-1) : model.StartDate.Value);
            ViewData.Add("endDate", (model == null || model.EndDate == null) ? DateTime.Now : model.EndDate.Value);
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
