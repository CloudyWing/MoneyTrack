using System;
using System.ComponentModel.DataAnnotations;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application;

namespace CloudyWing.MoneyTrack.Areas.Administrators.Models.RecordModel {
    public class IndexViewModel : PagingQueryFieldsViewModel<IndexViewModel> {
        [Display(Name = "分類")]
        public long? CategoryId { get; set; }

        [Display(Name = "收入/支出")]
        public int Income { get; set; }

        [Display(Name = "開始日期")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = Constants.HtmlEditorDateFormat)]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "結束日期")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = Constants.HtmlEditorDateFormat)]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "最小金額")]
        public long? MinAmount { get; set; }

        [Display(Name = "最大金額")]
        public long? MaxAmount { get; set; }

        [Display(Name = "備註")]
        public string Description { get; set; }
    }
}
