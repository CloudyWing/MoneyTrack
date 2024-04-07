using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CloudyWing.MoneyTrack.Areas.Administrators.Models.CategoryModel;
using CloudyWing.MoneyTrack.Infrastructure.Controllers;
using CloudyWing.MoneyTrack.Infrastructure.Filters;
using CloudyWing.MoneyTrack.Infrastructure.ModelBinding;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models;

namespace CloudyWing.MoneyTrack.Areas.Administrators.Controllers {
    public class CategoryController : BasicController {
        private readonly CategoryAppService appService;

        public CategoryController(CategoryAppService appService) {
            ExceptionUtils.ThrowIfNull(() => appService);

            this.appService = appService;
        }

        // GET: Administrators/Category
        public ActionResult Index() {
            IndexViewModel viewModel = GetInternalTempData(nameof(IndexViewModel), () => new IndexViewModel(), true);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IndexViewModel viewModel) {
            if (!ModelState.IsValid) {
                IndexViewModel prevViewModel = GetInternalTempData(nameof(IndexViewModel), () => new IndexViewModel(), true);
                viewModel.PrevViewModel = prevViewModel;
            } else {
                SetInternalTempData(nameof(IndexViewModel), viewModel);
            }

            return View(viewModel);
        }

        [ChildActionOnly]
        public ActionResult GetList(IndexViewModel viewModel) {
            IEnumerable<IndexListItemViewModel> items = appService.GetList(viewModel);

            return View("_IndexList", items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoveUp([SimpleTypeBind(Prefix = "item")] long? id) {
            if (!id.HasValue) {
                return HttpNotFound();
            }

            ResponseResult result = appService.MoveUp(id.Value);

            if (result.IsError) {
                IndexViewModel viewModel = GetInternalTempData(nameof(IndexViewModel), () => new IndexViewModel(), true);

                ModelState.AddModelError("", result.Message);

                return View("Index", viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoveDown([SimpleTypeBind(Prefix = "item")] long? id) {
            if (!id.HasValue) {
                return HttpNotFound();
            }

            ResponseResult result = appService.MoveDown(id.Value);

            if (result.IsError) {
                IndexViewModel viewModel = GetInternalTempData(nameof(IndexViewModel), () => new IndexViewModel(), true);

                ModelState.AddModelError("", result.Message);

                return View("Index", viewModel);
            }

            return RedirectToAction(nameof(Index));
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
            return View("Edit", new EditViewModel());
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
