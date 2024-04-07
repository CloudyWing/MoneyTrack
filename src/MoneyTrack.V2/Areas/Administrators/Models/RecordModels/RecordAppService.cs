using System.Collections.Generic;
using System.Web.Mvc;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Models.DataAccess;
using CloudyWing.MoneyTrack.Models.Domain.CategoryModels;

namespace CloudyWing.MoneyTrack.Areas.Administrators.Models.RecordModel {
    public class RecordAppService : ApplicationService {
        private readonly RecordService recordService;
        private readonly CategoryService categoryService;

        public RecordAppService(RecordService recordService, CategoryService categoryService) {
            ExceptionUtils.ThrowIfNull(() => recordService);
            ExceptionUtils.ThrowIfNull(() => categoryService);

            this.recordService = recordService;
            this.categoryService = categoryService;
        }

        public IEnumerable<SelectListItem> GetCategories() {
            return categoryService.GetList<SelectListItem>("Id Value, Name Text");
        }

        public PagedList<IndexListItemViewModel> GetList(IndexViewModel viewModel) {
            RecordQueryCondition condition = new RecordQueryCondition {
                CategoryId = viewModel.CategoryId,
                IsIncome = viewModel.Income == 1 ? true
                    : viewModel.Income == 2 ? (bool?)false
                    : null,
                RecordDate = OperatorUtils.GreaterThanOrEquals(viewModel.StartDate)
                    & OperatorUtils.LessThanOrEquals(viewModel.EndDate),
                Amount = OperatorUtils.GreaterThanOrEquals(viewModel.MinAmount)
                    & OperatorUtils.LessThanOrEquals(viewModel.MaxAmount),
                Description = OperatorUtils.Contains(viewModel.Description)
            };

            return recordService.GetPagedList<IndexListItemViewModel>(viewModel.PageNumber, 20, condition);
        }

        public ResponseResult Delete(long id) {
            bool isOk = recordService.Delete(id);

            return isOk
                ? ResponseResult.Succeed()
                : ResponseResult.FailExecution();
        }

        public ResponseResult Create(EditViewModel viewModel) {
            bool isOk = recordService.Create(new RecordEditor {
                CategoryId = viewModel.CategoryId.Value,
                IsIncome = viewModel.IsIncome.Value,
                RecordDate = viewModel.RecordDate.Value,
                Amount = viewModel.Amount.Value,
                Description = viewModel.Description
            });

            return isOk
                ? ResponseResult.Succeed()
                : ResponseResult.FailExecution();
        }

        public EditViewModel GetSingle(long id) {
            RecordQueryCondition condition = new RecordQueryCondition {
                Id = id
            };

            return recordService.GetSingle<EditViewModel>(condition);
        }

        public ResponseResult Update(EditViewModel viewModel) {
            bool isOk = recordService.Update(new RecordEditor(viewModel.Id.Value) {
                CategoryId = viewModel.CategoryId.Value,
                IsIncome = viewModel.IsIncome.Value,
                RecordDate = viewModel.RecordDate.Value,
                Amount = viewModel.Amount.Value,
                Description = viewModel.Description
            });

            return isOk
                ? ResponseResult.Succeed()
                : ResponseResult.FailExecution();
        }
    }
}
