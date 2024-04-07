using System.Web.Mvc;
using CloudyWing.MoneyTrack.Infrastructure.Hosting;

namespace CloudyWing.MoneyTrack {
    public abstract class WebViewPageBase : WebViewPage, IExtraWebViewPage {
        private readonly WebViewPageHelper helper;

        protected WebViewPageBase() {
            helper = new WebViewPageHelper(this);
        }

        public HostEnvironment HostEnvironment => helper.HostEnvironment;

        public string PageTitle => helper.PageTitle;

        public string TransferMessage => helper.TransferMessage;

        public TService GetDependencyService<TService>() {
            return helper.GetDependencyService<TService>();
        }
    }

    public abstract class WebViewPageBase<T> : WebViewPage<T>, IExtraWebViewPage {
        private readonly WebViewPageHelper helper;

        protected WebViewPageBase() {
            helper = new WebViewPageHelper(this);
        }

        public HostEnvironment HostEnvironment => helper.HostEnvironment;

        public string PageTitle => helper.PageTitle;

        public string TransferMessage => helper.TransferMessage;

        public TService GetDependencyService<TService>() {
            return helper.GetDependencyService<TService>();
        }
    }
}
