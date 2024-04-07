using System.ComponentModel.DataAnnotations;
using CloudyWing.MoneyTrack.Models.Application;

namespace CloudyWing.MoneyTrack.Areas.Administrators.Models.CategoryModel {
    public class IndexViewModel : QueryFieldsViewModel<IndexViewModel> {
        [Display(Name = "分類名稱")]
        public string Name { get; set; }
    }
}
