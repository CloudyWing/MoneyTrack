using System.Web.Mvc;

namespace CloudyWing.MoneyTrack.App_Start {
    public class ViewEngineConfig {
        public static void RegisterViewEngines(ViewEngineCollection viewEngines) {
            viewEngines.Clear();
            viewEngines.Add(new RazorViewEngine());
        }
    }
}
