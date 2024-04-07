using CloudyWing.MoneyTrack.Models.Application;

namespace System.Web.Mvc.Html {
    public static class PartialViewExtensions {
        public static IHtmlString PagedListPager(this HtmlHelper htmlHelper, PagedListPagerViewModel viewModel) {

            return htmlHelper.Partial("_PagedListPager", viewModel);
        }

        public static IHtmlString EmptyDataMessage(this HtmlHelper htmlHelper) {
            return htmlHelper.Partial("_EmptyDataMessage");
        }

        public static IHtmlString Breadcrumb(this HtmlHelper htmlHelper) {
            return htmlHelper.Partial("_Breadcrumb");
        }
    }
}
