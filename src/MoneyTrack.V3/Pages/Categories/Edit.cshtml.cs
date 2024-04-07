using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Pages.Categories.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudyWing.MoneyTrack.Pages.Categories {
    public class EditModel : PageModelBase {
        private readonly CategoryEditAppService appService;

        public EditModel(CategoryEditAppService appService) {
            ExceptionUtils.ThrowIfNull(() => appService);

            this.appService = appService;
        }

        public override string? MenuKey => "Categories";

        [BindProperty]
        public EditInputModel Input { get; set; }

        public async Task OnGetAsync(long? id) {
            Input = id.HasValue
                ? await appService.GetSingleAsync(id.Value)
                : new EditInputModel();
        }

        public async Task<ActionResult> OnPostAsync() {
            ApplicationResult result = await appService.SaveAsync(Input);
            if (result.IsError) {
                StatusNotification = new NotificationViewModel(result);
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
