using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CloudyWing.FormModule.Infrastructure;
using CloudyWing.MoneyTrack.Models.Application;

namespace CloudyWing.MoneyTrack {
    public class WebViewPageHelper : InfrastructureBase, IExtraWebViewPage {
        private readonly WebViewPage webViewPage;
        private string transferMessage;

        public WebViewPageHelper(WebViewPage webViewPage) {
            this.webViewPage = webViewPage ?? throw new ArgumentNullException(nameof(webViewPage));
        }

        public string PageTitle {
            get {
                string areaName = webViewPage.ViewContext.RouteData.DataTokens["area"]?.ToString();
                string controllerName = webViewPage.ViewContext.RouteData.Values["controller"]?.ToString();
                string actionName = webViewPage.ViewContext.RouteData.Values["action"]?.ToString() ?? "Index";

                IEnumerable<MenuViewModel> menus = GetAllMenus(MenuViewModel.Instance, areaName, controllerName);

                MenuViewModel menu = menus.SingleOrDefault(x => x.Action?.ToLower() == actionName.ToLower())
                    ?? menus.SingleOrDefault(x => x.Action is null);

                return menu?.Title ?? webViewPage.ViewBag.Title;
            }
        }

        public string TransferMessage {
            get {
                if (transferMessage is null) {
                    transferMessage = webViewPage.TempData["TransferMessage"]?.ToString();
                }

                return transferMessage;
            }
        }

        private IEnumerable<MenuViewModel> GetAllMenus(MenuViewModel menu, string areaName, string controllerName) {
            foreach (MenuViewModel childMenu in menu.ChildMenus) {
                if (childMenu.ChildMenus?.Any() == true) {
                    foreach (MenuViewModel descendantMenu in GetAllMenus(childMenu, areaName, controllerName)) {
                        yield return descendantMenu;
                    }
                } else {
                    if (childMenu.IsMatch(areaName, controllerName)) {
                        yield return childMenu;
                    }
                }
            }
        }
    }
}
