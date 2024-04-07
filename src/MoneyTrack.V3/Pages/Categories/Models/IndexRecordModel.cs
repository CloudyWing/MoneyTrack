using System.ComponentModel.DataAnnotations;

namespace CloudyWing.MoneyTrack.Pages.Categories.Models {
    public class IndexRecordModel {
        public long Id { get; set; }

        [Display(Name = "分類名稱")]
        public string? Name { get; set; }

        public long RecordCount { get; set; }
    }
}
