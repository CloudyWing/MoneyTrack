using System.Collections.Generic;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Models.Domain.CategoryModels;

namespace CloudyWing.MoneyTrack.Areas.Administrators.Models.CategoryModel {
    public class CategoryAppService : ApplicationService {
        private readonly CategoryService categoryService;

        public CategoryAppService(CategoryService categoryService) {
            ExceptionUtils.ThrowIfNull(() => categoryService);

            this.categoryService = categoryService;
        }

        public IEnumerable<IndexListItemViewModel> GetList(IndexViewModel input) {
            CategoryQueryCondition condition = new CategoryQueryCondition {
                Name = OperatorUtils.Contains(input.Name)
            };

            return categoryService.GetList<IndexListItemViewModel>(condition: condition);
        }

        public ResponseResult MoveUp(long id) {
            bool isOk = categoryService.MoveUp(id);

            return isOk
                ? ResponseResult.Succeed()
                : ResponseResult.FailExecution();
        }

        public ResponseResult MoveDown(long id) {
            bool isOk = categoryService.MoveDown(id);

            return isOk
                ? ResponseResult.Succeed()
                : ResponseResult.FailExecution();
        }

        public ResponseResult Delete(long id) {
            bool isOk = categoryService.Delete(id);

            return isOk
                ? ResponseResult.Succeed()
                : ResponseResult.FailExecution();
        }

        public ResponseResult Create(EditViewModel viewModel) {
            bool isOk = categoryService.Create(new CategoryEditor {
                Name = viewModel.Name
            });

            return isOk
                ? ResponseResult.Succeed()
                : ResponseResult.FailExecution();
        }

        public EditViewModel GetSingle(long id) {
            CategoryQueryCondition condition = new CategoryQueryCondition {
                Id = id
            };

            return categoryService.GetSingle<EditViewModel>(condition);
        }

        public ResponseResult Update(EditViewModel viewModel) {
            bool isOk = categoryService.Update(new CategoryEditor(viewModel.Id.Value) {
                Name = viewModel.Name
            });

            return isOk
                ? ResponseResult.Succeed()
                : ResponseResult.FailExecution();
        }
    }
}
