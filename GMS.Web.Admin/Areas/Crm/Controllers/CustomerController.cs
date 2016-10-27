using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GMS.Crm.Contract;
using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using GMS.Framework.Utility;
using GMS.Framework.Contract;
using GMS.OA.Contract;

namespace GMS.Web.Admin.Areas.Crm.Controllers
{
    [Permission(EnumBusinessPermission.CrmManage_Customer)]
    public class CustomerController : AdminControllerBase
    {
        //
        // GET: /Crm/Customer/

        public ActionResult Index(CustomerRequest request)
        {
            this.TryUpdateModel<Customer>(request.Customer);

            this.ModelState.Clear();

            this.RenderMyViewData(request.Customer, true);

            int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
            var result = this.CrmService.GetCustomerList(GetCurrentUserStaffs(currentstaffid), request);
            return View(result);
        }

        //
        // GET: /Crm/Customer/Create

        public ActionResult Create()
        {
            var model = new Customer();

            this.RenderMyViewData(model);
            this.ViewBag.CustomerCooperationsIds = new SelectList(this.Cooperations, "ID", "Name", string.Join(",", model.Cooperations.Select(p => p.ID)));
            return View("Edit", model);
        }

        //
        // POST: /Crm/Customer/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var model = new Customer();
            this.TryUpdateModel<Customer>(model);
            model.StaffID = model.StaffID == null ? (UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1) : model.StaffID;
            model.Channel = collection["Channel"];
            model.BusinessType = collection["BusinessType"];
            try
            {
                this.CrmService.SaveCustomer(model);
            }
            catch (BusinessException e)
            {
                this.ModelState.AddModelError(e.Name, e.Message);
                this.RenderMyViewData(model);
                return View("Edit", model);
            }


            return this.RefreshParent();
        }

        //
        // GET: /Crm/Customer/Edit/5

        public ActionResult Edit(int id)
        {
            var model = this.CrmService.GetCustomer(id);

            this.ViewBag.CustomerCooperationsIds = new SelectList(this.Cooperations, "ID", "Name", string.Join(",", model.Cooperations.Select(p => p.ID)));
            this.RenderMyViewData(model);
            return View(model);
        }

        //
        // POST: /Crm/Customer/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var model = this.CrmService.GetCustomer(id);
            this.TryUpdateModel<Customer>(model);
            model.Channel = collection["Channel"];
            model.BusinessType = collection["BusinessType"];
            model.StaffID = this.LoginInfo.StaffID;
            if (collection["ChainType"] != null)
            {
                int chain = 0;
                int.TryParse(collection["ChainType"], out chain);
                model.ChainType = chain;
            }
            else
            {
                model.ChainType = null;
            }
            try
            {
                this.CrmService.SaveCustomer(model);
            }
            catch (BusinessException e)
            {
                this.ModelState.AddModelError(e.Name, e.Message);
                this.RenderMyViewData(model);
                return View("Edit", model);
            }

            return this.RefreshParent();
        }

        // POST: /Crm/Customer/Delete/5

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                this.CrmService.DeleteCustomer(ids);
            }
            return RedirectToAction("Index");
        }
        private void RenderEditViewData(Customer model)
        {


        }
        private void RenderMyViewData(Customer model, bool isBasic = false)
        {
            ViewData.Add("Gender", new SelectList(EnumHelper.GetItemValueList<GMS.Crm.Contract.EnumGender>(), "Key", "Value", model.Gender));
            ViewData.Add("Category", new SelectList(EnumHelper.GetItemValueList<EnumCategory>(), "Key", "Value", model.Category));

            if (isBasic)
                return;

            int province = -1;
            if (model.CityId.HasValue == true)
            {
                var currencity = CityDic.FirstOrDefault(p => p.Value.ID == model.CityId.Value);
                if (currencity.Value != null)
                {
                    province = currencity.Value.ProvinceID;
                }
            }

            ViewData.Add("CityIds", new SelectList(CityDic.Values.Select(c => new { Id = c.ID, Name = c.Name }), "Id", "Name", model.CityId));
            ViewData.Add("ProvinceIds", new SelectList(ProvinceDic.Values.Select(c => new { Id = c.ID, Name = c.Name }), "Id", "Name", province));

            ViewData.Add("Channel", new SelectList(EnumHelper.GetItemValueList<EnumChannel>(), "Key", "Value", model.Channel));
            ViewData.Add("BusinessType", new SelectList(EnumHelper.GetItemValueList<EnumBusinessType>(), "Key", "Value", model.BusinessType));

            ViewData.Add("ChainTypeIds", new SelectList(EnumHelper.GetItemValueList<EnumChainType>(), "Key", "Value", model.ChainType));

            List<Staff> liststaff = GetCurrentUserStaffs();
            ViewData.Add("Staffs", new SelectList(liststaff.Select(c => new { Id = c.ID, Name = c.Name }), "Id", "Name", model.StaffID));
        }

        public ActionResult GetCity(int ProvinceId)
        {
            var areas = this.CityDic.Values.Where(a => a.ProvinceID == ProvinceId);
            ViewData.Add("CityIds", new SelectList(areas, "Id", "Name"));

            return PartialView("CitySelect");
        }

        public Dictionary<int, City> CityDic
        {
            get
            {
                return AdminCacheContext.Current.CityDic;
            }
        }
        public Dictionary<int, Province> ProvinceDic
        {
            get
            {
                return AccountService.GetProvinceList().ToDictionary(a => a.ID);
            }
        }

        public IEnumerable<Cooperations> Cooperations
        {
            get
            {
                return AdminCacheContext.Current.Cooperations;
            }
        }

    }
}
