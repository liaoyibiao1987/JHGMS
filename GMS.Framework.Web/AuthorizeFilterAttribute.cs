using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GMS.Framework.Web
{
    /// <summary>
    /// Attribute for power Authorize
    /// </summary>
    public class AuthorizeFilterAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }

        public AuthorizeFilterAttribute(string name)
        {
            this.Name = name;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!this.Authorize(filterContext, this.Name))
                filterContext.Result = new ContentResult { Content = "<script>alert('抱歉,你不具有当前操作的权限！');history.go(-1)</script>" };
        }

        protected virtual bool Authorize(ActionExecutingContext filterContext, string permissionName)
        {
            //var p = filterContext.ActionDescriptor;
            var attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true);
            if (attrs.Length > 0) return true;
            foreach (var item in attrs)
            {
                AllowAnonymousAttribute attr = item as AllowAnonymousAttribute;
            }

            if (filterContext.HttpContext == null)
                throw new ArgumentNullException("httpContext");

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                return false;

            return true;
        }
    }
}