using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Models.Domain.CategoryModels;
using CloudyWing.MoneyTrack.Models.Domain.Statements;

namespace CloudyWing.MoneyTrack.Pages.Categories.Models {
    public class CategoryIndexAppService : ApplicationService {
        private readonly CategoryService categoryService;

        public CategoryIndexAppService(IServiceProvider serviceProvider, CategoryService? categoryService) : base(serviceProvider) {
            ExceptionUtils.ThrowIfNull(() => categoryService);

            this.categoryService = categoryService;
        }

        public async Task<IEnumerable<IndexRecordModel>> GetListAsync(IndexInputModel? input) {
            ExceptionUtils.ThrowIfNull(() => input);

            return await categoryService.GetListAsync<IndexRecordModel>(
                x => x.Column(y => y.Id, y => y.Id).Column(y => y.Name, y => y.Name),
                x => x.Column(y => y.Name).Contain(input.Name),
                x => x.Asc(y => y.DisplayOrder)
            );
        }

        public async Task<ApplicationResult> MoveUpAsync(long id) {
            return await categoryService.MoveUpAsync(id)
                ? ApplicationResult.Succeed()
                : ApplicationResult.FailExecution();
        }

        public async Task<ApplicationResult> MoveDownAsync(long id) {
            return await categoryService.MoveDownAsync(id)
                ? ApplicationResult.Succeed()
                : ApplicationResult.FailExecution();
        }

        public async Task<ApplicationResult> DeleteAsync(long id) {
            return await categoryService.DeleteAsync(id)
                ? ApplicationResult.Succeed()
                : ApplicationResult.FailExecution();
        }
    }
}
