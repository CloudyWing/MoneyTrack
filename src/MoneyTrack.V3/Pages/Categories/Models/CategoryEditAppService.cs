using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Models.Domain.CategoryModels;
using CloudyWing.MoneyTrack.Models.Domain.Statements;

namespace CloudyWing.MoneyTrack.Pages.Categories.Models {
    public class CategoryEditAppService : ApplicationService {
        private readonly CategoryService categoryService;

        public CategoryEditAppService(IServiceProvider? serviceProvider, CategoryService? categoryService) : base(serviceProvider) {
            ExceptionUtils.ThrowIfNull(() => categoryService);

            this.categoryService = categoryService;
        }

        public async Task<EditInputModel> GetSingleAsync(long id) {
            return await categoryService.GetSingleAsync<EditInputModel>(
                x => x.Column(y => y.Id, y => y.Id).Column(y => y.Name, y => y.Name),
                x => x.Column(y => y.Id).Equal(id)
            );
        }

        public async Task<ApplicationResult> SaveAsync(EditInputModel? input) {
            ExceptionUtils.ThrowIfNull(() => input);

            bool isOk;
            if (input.Id.HasValue) {
                isOk = await categoryService.UpdateAsync(new CategoryEditor(input.Id.Value) {
                    Name = input.Name
                });
            } else {
                isOk = await categoryService.CreateAsync(new CategoryEditor {
                    Name = input.Name
                });
            }

            return isOk
                ? ApplicationResult.Succeed()
                : ApplicationResult.FailExecution();
        }
    }
}
