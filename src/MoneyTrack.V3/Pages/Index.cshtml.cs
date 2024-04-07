using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Models.Application.IndexModel;
using Microsoft.AspNetCore.Mvc;

namespace CloudyWing.MoneyTrack.Pages {
    public class IndexModel : PageModelBase {
        private readonly LoginAppService appService;
        public IndexModel(LoginAppService appService) {
            ExceptionUtils.ThrowIfNull(() => appService);

            this.appService = appService;
        }

        public override string? Title => "µn¤J";

        [BindProperty]
        public LoginInputModel? Input { get; set; }

        public void OnGet() {
            Input = new LoginInputModel();
        }

        public async Task<ActionResult> OnPostAsync() {
            if (!await appService.LoginAsync(Input)) {
                ModelState.AddModelError("", "±b¸¹©Î±K½X¿ù»~¡C");
                return Page();
            }

            return RedirectToPage("./Records/Index");
        }
    }
}
