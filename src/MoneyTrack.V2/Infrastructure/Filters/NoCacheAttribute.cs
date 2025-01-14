﻿using System.Web;
using System.Web.Mvc;

namespace CloudyWing.MoneyTrack.Infrastructure.Filters {
    public class NoCacheAttribute : ActionFilterAttribute {
        public override void OnResultExecuting(ResultExecutingContext filterContext) {
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();
            base.OnResultExecuting(filterContext);
        }
    }
}
