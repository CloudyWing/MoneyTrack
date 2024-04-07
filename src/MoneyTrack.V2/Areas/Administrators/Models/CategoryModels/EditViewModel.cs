using System.ComponentModel.DataAnnotations;

namespace CloudyWing.MoneyTrack.Areas.Administrators.Models.CategoryModel {
    public class EditViewModel {
        public long? Id { get; set; }

        [Display(Name = "分類名稱")]
        [Required]
        public string Name { get; set; }
    }
}
