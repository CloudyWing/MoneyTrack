using System.ComponentModel.DataAnnotations;

namespace CloudyWing.MoneyTrack.Pages.Categories.Models {
    public class EditInputModel {
        public long? Id { get; set; }

        [Display(Name = "分類名稱")]
        [Required]
        public string? Name { get; set; }
    }
}
