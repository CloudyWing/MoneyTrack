using System.Web.Mvc;
using CloudyWing.MoneyTrack.Infrastructure.Controllers;
using CloudyWing.MoneyTrack.Infrastructure.Filters;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application.HomeModels;

namespace CloudyWing.MoneyTrack.Controllers {
    [AllowAnonymous]
    public class HomeController : BasicController {
        private readonly HomeAppService homeAppService;

        public HomeController(HomeAppService homeAppService) {
            ExceptionUtils.ThrowIfNull(() => homeAppService);

            this.homeAppService = homeAppService;
        }

        public ActionResult Index() {
            return View(new IndexViewModel());
        }

        [HttpPost]
        [ValidationModelState]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IndexViewModel viewModel) {
            if (!homeAppService.Login(viewModel)) {
                return View(viewModel);
            }

            TransferMessage = "登入成功。";
            return RedirectToAction("Index", "Record", new { Area = "Administrators" });
        }

        public ActionResult Logout() {
            homeAppService.Logout();

            return RedirectToAction(nameof(Index));
        }
    }
}
