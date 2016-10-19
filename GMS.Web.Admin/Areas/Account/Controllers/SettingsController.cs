using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;
using GMS.Crm.Contract;

namespace GMS.Web.Admin.Areas.Account.Controllers
{
    [Permission(EnumBusinessPermission.AccountManage_Role)]
    public class SettingsController : AdminControllerBase
    {
        //
        // GET: /Account/Settings/

        public ActionResult Index()
        {

            var provinces = this.ProvinceDic.Values.Select(c => new { Id = c.ID, Name = c.Name });
            ViewData.Add("ProvinceId", new SelectList(provinces, "Id", "Name", -1));

            var citys = this.CityDic.Values.Select(c => new { Id = c.ID, Name = c.Name });
            ViewData.Add("CityId", new SelectList(citys, "Id", "Name", -1));

            var areas = this.AreaDic.Values.Select(c => new { Id = c.ID, Name = c.Name });
            ViewData.Add("AreaId", new SelectList(areas, "Id", "Name", -1));

            this.ViewBag.Cooperations = CooperationsList;

            return View();


        }

        [HttpPost]
        public ActionResult AddOrEidtCooperation(int Id, string Name)
        {
            bool ret = AccountService.AddOrEidtCooperation(Id, Name);
            //string retstr = string.Format("修改{0}", ret == true ? "成功" : "失败");
            return new ContentResult() { Content = ret.ToString() };
        }


        [HttpPost]
        public ActionResult DeleteCooperations(List<int> ids)
        {
            bool ret = AccountService.DeleteCooperations(ids);
            return RedirectToAction("Index");
        }

        public ActionResult AddCityByPost(int ProvinceId, string Name)
        {
            bool ret = AccountService.AddCityByName(ProvinceId, Name);
            return new ContentResult() { Content = ret.ToString() };
        }

        public ActionResult AddProvinceByPost(string Name)
        {
            bool ret = AccountService.AddProvinceByName(Name);
            return new ContentResult() { Content = ret.ToString() };
        }
        public ActionResult AddAreaByPost(int CityId, string Name)
        {
            bool ret = AccountService.AddAreaByName(CityId, Name);
            return new ContentResult() { Content = ret.ToString() };
        }



        public ActionResult GetCity(int ProvinceId)
        {
            var areas = this.CityDic.Values.Where(a => a.ProvinceID == ProvinceId);
            ViewData.Add("CityId", new SelectList(areas, "Id", "Name"));

            return PartialView("CitySelect");
        }
        public ActionResult GetProvince()
        {
            var provinces = this.ProvinceDic.Values.OrderByDescending(p => p.ID);
            ViewData.Add("ProvinceId", new SelectList(provinces, "Id", "Name"));

            return PartialView("ProvinceSelect");
        }

        public Dictionary<int, City> CityDic
        {
            get
            {

                return AccountService.GetCityList().ToDictionary(a => a.ID);
            }
        }

        public Dictionary<int, Area> AreaDic
        {
            get
            {
                return AccountService.GetAreaList().ToDictionary(a => a.ID);
            }
        }

        public Dictionary<int, Province> ProvinceDic
        {
            get
            {
                return AccountService.GetProvinceList().ToDictionary(a => a.ID);
            }
        }

        public IEnumerable<Cooperations> CooperationsList
        {
            get
            {
                return AccountService.GetCooperationsList();
            }
        }

    }
}
