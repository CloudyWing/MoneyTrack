using System;
using System.Linq;
using System.Web;
using CloudyWing.MoneyTrack.Infrastructure.Options;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models.Application.HomeModels {
    public class HomeAppService : ApplicationService {
        private readonly AccountOptions accountOptions;

        private const string CacheLoginKey = "CacheLogin";
        public HomeAppService(AccountOptions accountOptions) {
            ExceptionUtils.ThrowIfNull(() => accountOptions);

            this.accountOptions = accountOptions;
        }
        public bool Login(IndexViewModel viewModel) {
            if (viewModel.UserId?.Equals(accountOptions.UserId, StringComparison.CurrentCultureIgnoreCase) == true
                && viewModel.Password == accountOptions.Password
            ) {
                UserContext userContext = new UserContext {
                    UserId = viewModel.UserId
                };
                userContext.Save(HttpContext);

                if (HostEnvironment.IsDevelopment) {
                    HttpContext.Response.Cookies.Add(new HttpCookie(CacheLoginKey, viewModel.UserId));
                }

                return true;
            }

            return false;
        }

        public bool TryLoginFromCookie() {
            if (!HostEnvironment.IsDevelopment || !HttpContext.Request.Cookies.AllKeys.Contains(CacheLoginKey)) {
                return false;
            }

            string userId = HttpContext.Request.Cookies.Get(CacheLoginKey).Value;
            if (userId?.Equals(accountOptions.UserId, StringComparison.CurrentCultureIgnoreCase) == true) {
                UserContext userContext = new UserContext {
                    UserId = userId
                };

                return userContext.Save(HttpContext);
            }

            return false;
        }

        public void Logout() {
            HttpCookie cookie = HttpContext.Response.Cookies.Get(CacheLoginKey);
            cookie.Expires = DateTime.Now.AddSeconds(-1);

            HttpContext.Session.Clear();
            HttpContext.Session.Abandon();
        }
    }
}
