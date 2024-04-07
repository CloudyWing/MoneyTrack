using System.ComponentModel.DataAnnotations;
using CloudyWing.MoneyTrack.Infrastructure.Filters;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Pages.Categories.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CloudyWing.MoneyTrack.Pages.Categories {
    public class IndexModel : PageModelBase {
        private readonly CategoryIndexAppService appService;

        public IndexModel(CategoryIndexAppService appService) {
            ExceptionUtils.ThrowIfNull(() => appService);

            this.appService = appService;
        }

        public override string? MenuKey => "Categories";

        [BindProperty]
        public IndexInputModel? Input { get; set; }

        [BindProperty]
        public IEnumerable<IndexRecordModel>? Records { get; set; }

        public async Task OnGetAsync() {
            Input = GetInternalTempData(nameof(IndexInputModel), () => new IndexInputModel(), true);
            Records = await appService.GetListAsync(Input);
        }

        [ValidationExecution(OnFailResultAction = nameof(LoadPrevRecordsAsync))]
        public async Task OnPostAsync(int pageNumber = 1) {
            Input!.PageNumber = pageNumber;
            Records = await appService.GetListAsync(Input);

            SetInternalTempData(nameof(IndexInputModel), Input);
        }

        [ValidationExecution(OnFailResultAction = nameof(LoadPrevRecordsAsync))]
        public async Task<IActionResult> OnPostMoveUpAsync([BindRequired] long id) {
            return await OnPostInternalAsync(() => appService.MoveUpAsync(id));
        }

        private async Task<IActionResult> OnPostInternalAsync(Func<Task<ApplicationResult>> handlerAsync) {
            ApplicationResult result = await handlerAsync();
            StatusNotification = new NotificationViewModel(result);

            if (result.IsError) {
                await LoadPrevRecordsAsync();
                return Page();
            }

            return RedirectToPage("Index");
        }

        [ValidationExecution(OnFailResultAction = nameof(LoadPrevRecordsAsync))]
        public async Task<IActionResult> OnPostMoveDownAsync([BindRequired] long id) {
            return await OnPostInternalAsync(() => appService.MoveDownAsync(id));
        }

        [ValidationExecution(OnFailResultAction = nameof(LoadPrevRecordsAsync))]
        public async Task<IActionResult> OnPostDeleteAsync([BindRequired] long id) {
            return await OnPostInternalAsync(() => appService.DeleteAsync(id));
        }

        private async Task LoadPrevRecordsAsync() {
            IndexInputModel? prevInput = GetInternalTempData(nameof(IndexInputModel), () => new IndexInputModel(), true);
            Records = await appService.GetListAsync(Input);
        }
    }
}
