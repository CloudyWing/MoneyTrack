using System.ComponentModel.DataAnnotations;
using CloudyWing.MoneyTrack.Models.Application;

namespace CloudyWing.MoneyTrack.Pages.Categories.Models {
    public class IndexInputModel : PagingInputModel {
        [Display(Name = "分類名稱")]
        public string? Name { get; set; }
    }
}
