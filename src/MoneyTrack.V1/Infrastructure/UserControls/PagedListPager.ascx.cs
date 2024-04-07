using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudyWing.MoneyTrack.Models.Queriers;

namespace CloudyWing.MoneyTrack.Infrastructure.UserControls {
    public partial class PagedListPager : UserControl {
        public Action<int> DataBinder { get; set; }

        public int DisplayedMaxPageCount { get; set; } = 10;

        public int PageNumber => int.TryParse(hidPage.Value, out int pageNumber) ? pageNumber : 1;

        public void BindMetadata(PagingMetadata pagedMetadata) {
            hidPage.Value = pagedMetadata.PageNumber.ToString();

            pnlPager.Visible = pagedMetadata.TotalItemCount > 0;

            btnFirstPage.Visible = pagedMetadata.HasPreviousPage;
            btnFirstPage.CommandArgument = "1";

            btnPrevPage.Visible = pagedMetadata.HasPreviousPage;
            btnPrevPage.CommandArgument = (pagedMetadata.PageNumber - 1).ToString();

            btnNextPage.Visible = pagedMetadata.HasNextPage;
            btnNextPage.CommandArgument = (pagedMetadata.PageNumber + 1).ToString();

            btnLastPage.Visible = pagedMetadata.HasNextPage;
            btnLastPage.CommandArgument = pagedMetadata.PageCount.ToString();

            rptPage.DataSource = GetPageSource(pagedMetadata.PageCount);
            rptPage.DataBind();
        }

        private IEnumerable<int> GetPageSource(int pageCount) {
            int startPage = PageNumber - (DisplayedMaxPageCount / 2);
            if (startPage < 1) {
                startPage = 1;
            }

            int endPage = startPage + DisplayedMaxPageCount - 1;
            if (endPage > pageCount) {
                endPage = pageCount;
            }

            for (int i = startPage; i <= endPage; i++) {
                yield return i;
            }
        }

        protected void btnPage_Command(object sender, CommandEventArgs e) {
            DataBinder(Convert.ToInt32(e.CommandArgument));
        }
    }
}
