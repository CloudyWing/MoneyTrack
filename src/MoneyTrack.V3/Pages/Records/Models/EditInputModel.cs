using System.ComponentModel.DataAnnotations;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Pages.Records.Models {
    public class EditInputModel {
        public long? Id { get; set; }

        [Display(Name = "分類")]
        [Required]
        public long? CategoryId { get; set; }

        [Display(Name = "交易日期")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = Constants.HtmlEditorDateFormat)]
        [DataType(DataType.Date)]
        [Required]
        public DateTime? RecordDate { get; set; }

        [Display(Name = "收入/支出")]
        [Required]
        public bool? IsIncome { get; set; }

        [Display(Name = "金額")]
        [Required]
        public long? Amount { get; set; }

        [Display(Name = "備註")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string? Description { get; set; }
    }
}
