namespace CloudyWing.MoneyTrack.Infrastructure.Pages {
    public abstract class AdministratorPageBase : WebPageBase {
        protected override bool IsLoginRequired => true;
    }
}
