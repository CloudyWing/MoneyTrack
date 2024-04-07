using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using CloudyWing.MoneyTrack.Infrastructure.Application;

namespace CloudyWing.MoneyTrack.MasterPages {
    public partial class SiteMobileMaster : SiteMaster {
        private UserContext userContext;

        protected UserContext UserContext {
            get {
                if (userContext is null) {
                    userContext = UserContext.GetInstance(Context);
                }
                return userContext;
            }
        }

        protected HostEnvironment HostEnvironment { get; } = new HostEnvironment();

        protected void Page_Init(object sender, EventArgs e) {
            if (UserContext.IsLogined) {
                CreateMenu();
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            if (!IsPostBack) {
                hlkHome.NavigateUrl = UserContext.IsLogined ? "~/Administrators/RecordIndex.aspx" : "~/Default.aspx";
            }
        }

        private void CreateMenu() {
            phNavItems.Controls.Clear();

            if (UserContext.IsLogined) {
                foreach (SiteMapNode childNode in SiteMap.RootNode.ChildNodes) {
                    if (childNode.HasChildNodes) {
                        Menu menu = new Menu(childNode.Title);
                        CreateMenuItem(menu, childNode);
                        phNavItems.Controls.Add(menu);
                    } else {
                        Menu menu = new Menu(childNode.Title, childNode.Url);
                        phNavItems.Controls.Add(menu);
                    }
                }
            }
        }

        private void CreateMenuItem(Menu control, SiteMapNode node) {
            foreach (SiteMapNode childNode in node.ChildNodes) {
                if (childNode.HasChildNodes) {
                    Menu menu = new Menu(childNode.Title);
                    CreateMenuItem(menu, childNode);
                    control.AddNewItem(menu);
                } else {
                    MenuItem menuItem = new MenuItem(childNode.Title, childNode.Url);
                    control.AddNewItem(menuItem);
                }
            }
        }

        private class Menu : HtmlGenericControl {
            private readonly HtmlGenericControl subMenus = null;

            public Menu(string title) : base("li") {
                subMenus = new HtmlGenericControl("div");
                string ddlId = "navbarDropdown" + Guid.NewGuid().ToString().Split('-')[0];

                Attributes.Add("class", "nav-item dropdown");

                HtmlGenericControl a = new HtmlGenericControl("a");
                a.Attributes.Add("id", ddlId);
                a.Attributes.Add("class", "nav-link dropdown-toggle");
                a.Attributes.Add("href", "#");
                a.Attributes.Add("role", "button");
                a.Attributes.Add("data-bs-toggle", "dropdown");
                a.Attributes.Add("aria-expanded", "false");
                a.InnerText = title;

                Controls.Add(a);

                subMenus.Attributes.Add("class", "dropdown-menu");
                subMenus.Attributes.Add("aria-labelledby", ddlId);

                Controls.Add(subMenus);
            }

            public void AddNewItem(Control control) {
                if (subMenus is null) {
                    Controls.Add(control);
                } else {
                    subMenus.Controls.Add(control);
                }
            }

            public Menu(string title, string url) : base("li") {
                Attributes.Add("class", "nav-item");
                HtmlGenericControl a = new HtmlGenericControl("a");
                a.Attributes.Add("class", "nav-link");
                a.Attributes.Add("href", url);
                a.InnerText = title;

                Controls.Add(a);
            }
        }

        private class MenuItem : HtmlGenericControl {
            public MenuItem(string title, string url) : base("a") {
                Attributes.Add("class", "dropdown-item");
                Attributes.Add("href", url);

                InnerText = title;
            }
        }
    }
}
