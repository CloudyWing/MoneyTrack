using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CloudyWing.MoneyTrack.Areas.Administrators.Models.RecordModel;
using CloudyWing.MoneyTrack.Infrastructure.Controllers;
using CloudyWing.MoneyTrack.Infrastructure.Filters;
using CloudyWing.MoneyTrack.Infrastructure.ModelBinding;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Models.DataAccess;

namespace CloudyWing.MoneyTrack.Areas.Administrators.Controllers {
    public class RecordController : BasicController {
        private readonly RecordAppService appService;

        public RecordController(RecordAppService appService) {
            ExceptionUtils.ThrowIfNull(() => appService);

            this.appService = appService;
        }

        // GET: Administrators/Record
        public ActionResult Index() {
            IndexViewModel viewModel = GetInternalTempData(nameof(IndexViewModel), () => CreateViewModel(), true);

            return View(viewModel);
        }

        private IndexViewModel CreateViewModel() {
            DateTime today = DateTime.Today;
            DateTime startDate = today.AddDays(0 - today.Day + 1);

            return new IndexViewModel {
                Income = 0,
                StartDate = startDate,
                EndDate = startDate.AddMonths(1).AddDays(-1)
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IndexViewModel viewModel) {
            if (!ModelState.IsValid) {
                IndexViewModel prevViewModel = GetInternalTempData(nameof(IndexViewModel), () => CreateViewModel(), true);
                viewModel.PrevViewModel = prevViewModel;
            } else {
                SetInternalTempData(nameof(IndexViewModel), viewModel);
            }

            return View(viewModel);
        }

        [ChildActionOnly]
        public ActionResult GetCategories() {
            return SelectList(appService.GetCategories());
        }

        [ChildActionOnly]
        public ActionResult GetList(PagingQueryViewModel<IndexViewModel> pagingQuery) {
            PagedList<IndexListItemViewModel> pagedList = appService.GetList(pagingQuery.Fields);

            PagedListPagerViewModel<IndexListItemViewModel> viewModel =
                new PagedListPagerViewModel<IndexListItemViewModel>(pagedList, pagingQuery);

            return View("_IndexList", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([SimpleTypeBind(Prefix = "item")] long? id) {
            if (!id.HasValue) {
                return HttpNotFound();
            }

            ResponseResult result = appService.Delete(id.Value);

            if (result.IsError) {
                IndexViewModel viewModel = GetInternalTempData(nameof(IndexViewModel), () => new IndexViewModel(), true);

                ModelState.AddModelError("", result.Message);

                return View("Index", viewModel);
            }

            TransferMessage = "刪除成功。";

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Create() {
            return View("Edit", new EditViewModel {
                RecordDate = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidationModelState(ViewName = "Edit")]
        public ActionResult Create(EditViewModel viewModel) {
            if (viewModel.Id.HasValue) {
                return HttpNotFound();
            }

            ResponseResult result = appService.Create(viewModel);

            TransferMessage = result.IsOk ? "新增成功。" : result.Message;

            return RedirectToAction(nameof(Index));
        }

        [ValidationModelState]
        public ActionResult Update([Required] long? id) {
            return View("Edit", appService.GetSingle(id.Value));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidationModelState(ViewName = "Edit")]
        public ActionResult Update(EditViewModel viewModel) {
            if (!viewModel.Id.HasValue) {
                return HttpNotFound();
            }

            ResponseResult result = appService.Update(viewModel);

            TransferMessage = result.IsOk ? "修改成功。" : result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
