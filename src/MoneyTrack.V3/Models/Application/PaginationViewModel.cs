using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.DataAccess;

namespace CloudyWing.MoneyTrack.Models.Application {
    public class PaginationViewModel : PagingMetadata {
        public PaginationViewModel(PagingMetadata pagingMetadata, string? page, string? pageHandler = null)
            : base(pagingMetadata.PageNumber, pagingMetadata.PageSize, pagingMetadata.TotalItemCount) {

            ExceptionUtils.ThrowIfNull(() => pagingMetadata);
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => page);

            Page = page;
            PageHandler = pageHandler;
        }

        public string Page { get; }

        public string? PageHandler { get; }
    }
}
