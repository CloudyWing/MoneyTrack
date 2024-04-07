using CloudyWing.MoneyTrack.Areas.Administrators.Models.RecordModel;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Models.DataAccess;
using CloudyWing.MoneyTrack.Models.Domain;
using CloudyWing.MoneyTrack.Models.Domain.CategoryModels;
using CloudyWing.MoneyTrack.Models.Domain.RecordModels;
using CloudyWing.MoneyTrack.Models.Domain.Statements;

namespace CloudyWing.MoneyTrack.Pages.Records.Models {
    public class RecordIndexAppService : RecordAppService {
        private readonly RecordService recordService;

        public RecordIndexAppService(IServiceProvider serviceProvider, RecordService? recordService, CategoryService categoryService) : base(serviceProvider, categoryService) {
            ExceptionUtils.ThrowIfNull(() => recordService);

            this.recordService = recordService;
        }

        public async Task<PagedList<IndexRecordModel>> GetListAsync(IndexInputModel? input) {
            ExceptionUtils.ThrowIfNull(() => input);
            IOperatorStatement? filter(FilterProvider<RecordQueryEntity> x) {
                List<IOperatorStatement> statements = [];
                if (input.CategoryId.HasValue) {
                    statements.Add(x.Column(y => y.Record!.CategoryId).Equal(input.CategoryId.Value));
                }

                if (input.Income > 0) {
                    statements.Add(x.Column(y => y.Record!.IsIncome).Equal(input.Income == 1));
                }

                if (input.StartDate.HasValue) {
                    statements.Add(x.Column(y => y.Record!.RecordDate) >= input.StartDate.Value);
                }

                if (input.EndDate.HasValue) {
                    statements.Add(x.Column(y => y.Record!.RecordDate) <= input.EndDate.Value);
                }

                if (input.MinAmount.HasValue) {
                    statements.Add(x.Column(y => y.Record!.Amount) >= input.MinAmount.Value);
                }

                if (input.MaxAmount.HasValue) {
                    statements.Add(x.Column(y => y.Record!.Amount) <= input.MaxAmount.Value);
                }

                if (!string.IsNullOrEmpty(input.Description)) {
                    statements.Add(x.Column(y => y.Record!.Description).Contain(input.Description));
                }

                if (statements.Count == 0) {
                    return null;
                }

                return statements.Aggregate((x, y) => x.And(y));
            }

            return await recordService.GetPagedList<IndexRecordModel>(
                x => x.Column(y => y.Record!.Id, y => y.Id)
                    .Column(y => y.Category!.Name, y => y.CategoryName)
                    .Column(y => y.Record!.RecordDate, y => y.RecordDate)
                    .Column(y => y.Record!.IsIncome, y => y.IsIncome)
                    .Column(y => y.Record!.Amount, y => y.Amount)
                    .Column(y => y.Record!.Description, y => y.Description),
                x => x.Desc(y => y.Record!.RecordDate),
                input.PageNumber, 20,
                filter
            );
        }

        public async Task<ApplicationResult> DeleteAsync(long id) {
            return await recordService.DeleteAsync(id)
                ? ApplicationResult.Succeed()
                : ApplicationResult.FailExecution();
        }
    }
}
