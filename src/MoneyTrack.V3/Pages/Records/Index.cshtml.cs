using CloudyWing.MoneyTrack.Areas.Administrators.Models.RecordModel;
using CloudyWing.MoneyTrack.Infrastructure.Filters;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Models.DataAccess;
using CloudyWing.MoneyTrack.Pages.Records.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudyWing.MoneyTrack.Pages.Records {
    public class IndexModel : PageModelBase
    {
        private readonly RecordIndexAppService appService;

        public IndexModel(RecordIndexAppService appService) {
            ExceptionUtils.ThrowIfNull(() => appService);

            this.appService = appService;
        }

        public override string? MenuKey => "Records";

        [BindProperty]
        public IndexInputModel? Input { get; set; }

        public IEnumerable<SelectListItem> Categories {get; set; }

        public PagedList<IndexRecordModel> PagedList {get; set; }

        public async Task OnGetAsync() {
            DateTime today = DateTime.Today;
            DateTime startDate = today.AddDays(0 - today.Day + 1);

            Input = GetInternalTempData(
                nameof(IndexInputModel),
                    () => new IndexInputModel {
                    StartDate = startDate,
                    EndDate = startDate.AddMonths(1).AddDays(-1),
                    Income = 0
                },
                true
            );
            PagedList = await appService.GetListAsync(Input);
            Categories = await appService.GetCategoriesAsync();
        }

        [ValidationExecution(OnExecutingAction = nameof(LoadCategoriesAsync), OnFailResultAction = nameof(LoadPrevRecordsAsync))]
        public async Task OnPostAsync(int pageNumber = 1) {
            Input!.PageNumber = pageNumber;
            PagedList = await appService.GetListAsync(Input);

            SetInternalTempData(nameof(IndexInputModel), Input);
        }

        [ValidationExecution(OnExecutingAction = nameof(LoadCategoriesAsync), OnFailResultAction = nameof(LoadPrevRecordsAsync))]
        public async Task<IActionResult> OnPostDeleteAsync([BindRequired] long id) {
            return await OnPostInternalAsync(() => appService.DeleteAsync(id));
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

        private async Task LoadPrevRecordsAsync() {
            IndexInputModel? prevInput = GetInternalTempData(nameof(IndexInputModel), () => new IndexInputModel(), true);
            PagedList = await appService.GetListAsync(Input);
        }

        private async Task LoadCategoriesAsync() {
            Categories = await appService.GetCategoriesAsync();
        }
    }
}
