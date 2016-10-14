using GMS.Account.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GMS.Web.Admin.Areas.Account.Controllers
{
    [Permission(EnumBusinessPermission.AccountManage_Role)]
    public class SettingsController : Controller
    {
        //
        // GET: /Account/Settings/

        public ActionResult Index()
        {
            return View();
        }

    }
}
