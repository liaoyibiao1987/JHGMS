using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GMS.OA.Contract;
using GMS.Account.Contract;
using GMS.Web.Admin.Common;
using GMS.Framework.Utility;

namespace GMS.Web.Admin.Areas.OA.Controllers
{
    [Permission(EnumBusinessPermission.OAManage_Staff)]
    public class StaffController : AdminControllerBase
    {
        //
        // GET: /OA/Staff/

        public ActionResult Index(StaffRequest request)
        {
            var result = this.OAService.GetStaffList(request);
            return View(result);
        }

        //
        // GET: /OA/Staff/Create

        public ActionResult Create()
        {
            var model = new Staff() { };
            this.RenderMyViewData(model);
            return View("Edit", model);
        }

        //
        // POST: /OA/Staff/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var model = new Staff();
            this.TryUpdateModel<Staff>(model);

            this.OAService.SaveStaff(model);

            return this.RefreshParent();
        }

        //
        // GET: /OA/Staff/Edit/5

        public ActionResult Edit(int id)
        {
            var model = this.OAService.GetStaff(id);
            this.RenderMyViewData(model);
            return View(model);
        }

        //
        // POST: /OA/Staff/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var model = this.OAService.GetStaff(id);
            int? olduserid = model.UserID;


            this.TryUpdateModel<Staff>(model);
            if (olduserid != model.UserID)
            {
                if (olduserid.HasValue)
                {
                    User olduser = this.AccountService.GetUser(olduserid.Value);
                    olduser.StaffID = null;
                    AccountService.SaveUser(olduser);
                }

                if (model.UserID != null)
                {
                    User newuser = this.AccountService.GetUser(model.UserID.Value);
                    newuser.StaffID = model.ID;
                    AccountService.SaveUser(newuser);
                }

            }

            this.OAService.SaveStaff(model);

            return this.RefreshParent();
        }

        // POST: /OA/Staff/Delete/5

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            this.OAService.DeleteStaff(ids);
            return RedirectToAction("Index");
        }

        private SelectList GetAllLoginID(int staffId, int currentUserID)
        {
            IEnumerable<User> users = AccountService.GetActivedUserList(staffId);
            return new SelectList(users, "ID", "LoginName", currentUserID);
        }

        private void RenderMyViewData(Staff model)
        {
            ViewData.Add("Position", new SelectList(EnumHelper.GetItemValueList<EnumPosition>(), "Key", "Value", model.Position));
            ViewData.Add("Gender", new SelectList(EnumHelper.GetItemValueList<EnumGender>(), "Key", "Value", model.Gender));
            //ViewData.Add("LoginIDs", new SelectList(this.AccountService.GetActivedUserList(), "ID", "LoginName", model.LoginID ?? -1));
            ViewData.Add("UserID", GetAllLoginID(model.ID, model.UserID ?? -1));
        }
    }
}
