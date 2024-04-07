using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using CloudyWing.MoneyTrack.Infrastructure.Pages;

namespace CloudyWing.MoneyTrack.Infrastructure.UserControls {
    public partial class Breadcrumb : System.Web.UI.UserControl {
        protected WebPageBase WebPage => Page as WebPageBase;

        protected void Page_Init(object sender, EventArgs e) {
            CreateBreadcrumb();
        }

        private void CreateBreadcrumb() {
            if (WebPage is null) {
                return;
            }

            IEnumerable<BreadcrumbItem> items = GetBreadcrumbItems(SiteMap.RootNode);

            phBreadcrumb.Controls.Add(new BreadcrumbItem("MoneyTrack", !items.Any()));

            foreach (BreadcrumbItem item in items) {
                phBreadcrumb.Controls.Add(item);
            }
        }

        private IEnumerable<BreadcrumbItem> GetBreadcrumbItems(SiteMapNode node) {
            foreach (SiteMapNode childNode in node.ChildNodes) {
                if (ResolveUrl(WebPage.RootNavUrl) == ResolveUrl(childNode.Url)) {
                    yield return new BreadcrumbItem(childNode.Title, true);
                    yield break;
                }

                IEnumerable<BreadcrumbItem> descendantItems = GetBreadcrumbItems(childNode);
                if (descendantItems.Any()) {
                    yield return new BreadcrumbItem(childNode.Title);

                    foreach (BreadcrumbItem descendantNote in descendantItems) {
                        yield return descendantNote;
                    }
                    yield break;
                }
            }
        }

        private class BreadcrumbItem : HtmlGenericControl {
            public BreadcrumbItem(string title, bool isActive = false) : base("li") {
                if (isActive) {
                    Attributes.Add("class", "breadcrumb-item active");
                    Attributes.Add("aria-current", "page");
                } else {
                    Attributes.Add("class", "breadcrumb-item");
                }

                InnerText = title;
            }
        }
    }
}
