using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Models.Domain.CategoryModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudyWing.MoneyTrack.Pages.Records.Models {
    public abstract class RecordAppService : ApplicationService {
        private readonly CategoryService categoryService;

        public RecordAppService(IServiceProvider serviceProvider, CategoryService categoryService) : base(serviceProvider) {
            ExceptionUtils.ThrowIfNull(() => categoryService);

            this.categoryService = categoryService;
        }

        public async Task<IEnumerable<SelectListItem>> GetCategoriesAsync() {
            return await categoryService.GetListAsync<SelectListItem>(
                x => x.Column(y => y.Id, y => y.Value)
                    .Column(y => y.Name, y => y.Text),
                null,
                x => x.Asc(y => y.DisplayOrder)
            );
        }
    }
}
