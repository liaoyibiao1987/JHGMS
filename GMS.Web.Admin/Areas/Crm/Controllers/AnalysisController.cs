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
using GMS.OA.Contract.Model;

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




            List<V_MonthBusiness> month = CrmService.GetMonthAnalysis();
            List<V_QuarterBusiness> quarter = CrmService.GetQuarterAnalysis();
            List<V_YearBusiness> year = CrmService.GetYearAnalysis();

            var x1 = from a in branchs
                     join b in month on a.ID equals b.BranchID into prodGroup
                     from item in prodGroup.DefaultIfEmpty()
                     select new { per = ((item == null || item.SumPredictPayment.HasValue == false) ? 0 : item.SumPredictPayment) };

            var x2 = from a in branchs
                     join b in quarter on a.ID equals b.BranchID into prodGroup
                     from item in prodGroup.DefaultIfEmpty()
                     select new { per = ((item == null || item.SumPredictPayment.HasValue == false) ? 0 : item.SumPredictPayment) };

            var x3 = from a in branchs
                     join b in year on a.ID equals b.BranchID into prodGroup
                     from item in prodGroup.DefaultIfEmpty()
                     select new { per = ((item == null || item.SumPredictPayment.HasValue == false) ? 0 : item.SumPredictPayment) };



            ViewData.Add("Branchs", string.Join(",", branchs.Select(p => "'" + p.Name + "'")) + ",'汇总'");

            ViewData.Add("Month", string.Join(",", x1.Select(p => p.per)) + "," + x1.Sum(q => q.per));
            ViewData.Add("Quarter", string.Join(",", x2.Select(p => p.per)) + "," + x2.Sum(q => q.per));
            ViewData.Add("Year", string.Join(",", x3.Select(p => p.per)) + "," + x3.Sum(q => q.per));

            return View();
        }

    }
}
