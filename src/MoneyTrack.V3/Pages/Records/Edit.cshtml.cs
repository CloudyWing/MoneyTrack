using CloudyWing.MoneyTrack.Infrastructure.Filters;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Pages.Records.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudyWing.MoneyTrack.Pages.Records {
    public class EditModel : PageModelBase {
        private readonly RecordEditAppService appService;

        public EditModel(RecordEditAppService appService) {
            ExceptionUtils.ThrowIfNull(() => appService);

            this.appService = appService;
        }

        public override string? MenuKey => "Records";

        [BindProperty]
        public EditInputModel? Input { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public async Task OnGetAsync(long? id) {
            Input = id.HasValue
                ? await appService.GetSingleAsync(id.Value)
                : new EditInputModel {
                    RecordDate = DateTime.Today,
                };
            await LoadCategoriesAsync();
        }

        [ValidationExecution(OnFailResultAction = nameof(LoadCategoriesAsync))]
        public async Task<ActionResult> OnPostAsync() {
            ApplicationResult result = await appService.SaveAsync(Input);
            if (result.IsError) {
                StatusNotification = new NotificationViewModel(result);
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task LoadCategoriesAsync() {
            Categories = await appService.GetCategoriesAsync();
        }
    }
}
