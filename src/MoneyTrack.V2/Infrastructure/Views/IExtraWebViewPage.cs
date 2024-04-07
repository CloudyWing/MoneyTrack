using CloudyWing.MoneyTrack.Infrastructure.Hosting;

namespace CloudyWing.MoneyTrack {
    public interface IExtraWebViewPage {
        HostEnvironment HostEnvironment { get; }

        string PageTitle { get; }

        string TransferMessage { get; }

        TService GetDependencyService<TService>();
    }
}
