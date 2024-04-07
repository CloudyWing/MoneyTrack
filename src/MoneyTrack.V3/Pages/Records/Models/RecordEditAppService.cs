using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Models.Domain.CategoryModels;
using CloudyWing.MoneyTrack.Models.Domain.RecordModels;
using CloudyWing.MoneyTrack.Models.Domain.Statements;

namespace CloudyWing.MoneyTrack.Pages.Records.Models {
    public class RecordEditAppService : RecordAppService {
        private readonly RecordService recordService;

        public RecordEditAppService(IServiceProvider serviceProvider, RecordService? recordService, CategoryService categoryService) : base(serviceProvider, categoryService) {
            ExceptionUtils.ThrowIfNull(() => recordService);

            this.recordService = recordService;
        }

        public async Task<EditInputModel> GetSingleAsync(long id) {
            return await recordService.GetSingleAsync<EditInputModel>(
                x => x.Column(y => y.Record!.Id, y => y.Id)
                    .Column(y => y.Record!.CategoryId, y => y.CategoryId)
                    .Column(y => y.Record!.IsIncome, y => y.IsIncome)
                    .Column(y => y.Record!.RecordDate, y => y.RecordDate)
                    .Column(y => y.Record!.Amount, y => y.Amount)
                    .Column(y => y.Record!.Description, y => y.Description),
                x => x.Column(y => y.Record!.Id).Equal(id)
            );
        }

        public async Task<ApplicationResult> SaveAsync(EditInputModel? input) {
            ExceptionUtils.ThrowIfNull(() => input);

            bool isOk;
            if (input.Id.HasValue) {
                isOk = await recordService.UpdateAsync(new RecordEditor(input.Id.Value) {
                    CategoryId = input.CategoryId.Value,
                    IsIncome = input.IsIncome.Value,
                    RecordDate = input.RecordDate.Value,
                    Amount = input.Amount.Value,
                    Description = input.Description
                });
            } else {
                isOk = await recordService.CreateAsync(new RecordEditor {
                    CategoryId = input.CategoryId.Value,
                    IsIncome = input.IsIncome.Value,
                    RecordDate = input.RecordDate.Value,
                    Amount = input.Amount.Value,
                    Description = input.Description
                });
            }

            return isOk
                ? ApplicationResult.Succeed()
                : ApplicationResult.FailExecution();
        }
    }
}
