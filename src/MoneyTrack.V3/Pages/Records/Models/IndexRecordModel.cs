using System.ComponentModel.DataAnnotations;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Pages.Records.Models {
    public class IndexRecordModel {
        public long Id { get; set; }

        [Display(Name = "分類")]
        public string? CategoryName { get; set; }

        [Display(Name = "交易日期")]
        [DisplayFormat(DataFormatString = Constants.HtmlDisplayDateFormat)]
        [DataType(DataType.Date)]
        public DateTime RecordDate { get; set; }

        [Display(Name = "收入/支出")]
        public bool IsIncome { get; set; }

        [Display(Name = "金額")]
        public long Amount { get; set; }

        [Display(Name = "備註")]
        public string? Description { get; set; }
    }
}
