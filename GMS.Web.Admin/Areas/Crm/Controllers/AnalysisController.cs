using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GMS.Crm.Contract;
using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using GMS.Framework.Utility;
using GMS.OA.Contract;

namespace GMS.Web.Admin.Areas.Crm.Controllers
{
    [Permission(EnumBusinessPermission.CrmManage_Analysis)]
    public class AnalysisController : AdminControllerBase
    {
        public ActionResult Index()
        {
            var startDate = Request["startDate"].ToDateTime(DateTime.Now.AddMonths(-6));
            ViewData.Add("startDate", startDate.ToCnDataString());

            var endDate = Request["endDate"].ToDateTime(DateTime.Now.AddDays(1));
            ViewData.Add("endDate", endDate.ToCnDataString());

            var userAnalysis = this.CrmService.GetUserAnalysis(startDate, endDate);

            var userNames = "'" + string.Join("','", userAnalysis.Select(g => g.UserName)) + "'";
            var visitRecordCount = string.Join(",", userAnalysis.Select(g => g.VisitRecordCount));
            var customerCount = string.Join(",", userAnalysis.Select(g => g.CustomerCount));

            ViewData.Add("userNames", userNames);
            ViewData.Add("visitRecordCount", visitRecordCount);
            ViewData.Add("customerCount", customerCount);

            return View();
        }

        public ActionResult VisitStatistics()
        {
            //var startDate = Request["startDate"].ToDateTime(DateTime.Now.AddMonths(-6));
            //ViewData.Add("startDate", startDate.ToCnDataString());

            //var endDate = Request["endDate"].ToDateTime(DateTime.Now.AddDays(1));
            //ViewData.Add("endDate", endDate.ToCnDataString());

            //var statistics = this.CrmService.GetVisitStatistics(startDate, endDate).ToDictionary(c => c.Hour);

            //var hours = new List<int>();
            //var visitRecordCount = new List<int>();
            //var visitCount = new List<int>();
            //var telCount = new List<int>();


            //for (int i = 0; i < 24; i++)
            //{
            //    hours.Add(i);

            //    if (statistics.ContainsKey(i))
            //    {
            //        visitRecordCount.Add(statistics[i].VisitRecordCount);
            //        visitCount.Add(statistics[i].VisitCount);
            //        telCount.Add(statistics[i].TelCount);
            //    }
            //    else
            //    {
            //        visitRecordCount.Add(0);
            //        visitCount.Add(0);
            //        telCount.Add(0);
            //    }
            //}

            //ViewData.Add("visitRecordCount", string.Join(",", visitRecordCount));
            //ViewData.Add("visitCount", string.Join(",", visitCount));
            //ViewData.Add("telCount", string.Join(",", telCount));

            //ViewData.Add("hours", "'" + string.Join("','", hours.Select(h => h.ToString() + ":00")) + "'");
            int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
            Staff current = OAService.GetStaff(currentstaffid);
            List<Branch> branchs = new List<Branch>();
            if (current.BranchId.HasValue)
            {
                branchs = OAService.GetBelongsBranch(current.BranchId.Value);
            }
            ViewData.Add("Branchs", string.Join(",", branchs.Select(p => "'" + p.Name + "'")));


            ViewData.Add("Month", string.Join(",", branchs.Select(p => p.ID)));
            ViewData.Add("Quarter", string.Join(",", branchs.Select(p => p.ID)));
            ViewData.Add("Year", string.Join(",", branchs.Select(p => p.ID)));

            return View();
        }

    }
}
