using CloudyWing.MoneyTrack.Models.DataAccess;

namespace CloudyWing.MoneyTrack.Models.Application {
    public class PagedListPagerViewModel {
        public PagedListPagerViewModel(PagingMetadata pagingMetadata, PagingQueryViewModel pagingQuery) {
            PagingMetadata = pagingMetadata;
            PagingQuery = pagingQuery;
        }

        public virtual PagingMetadata PagingMetadata { get; }

        public PagingQueryViewModel PagingQuery { get; }
    }

    public class PagedListPagerViewModel<TRecord> : PagedListPagerViewModel {
        public PagedListPagerViewModel(PagedList<TRecord> pagedList, PagingQueryViewModel pagingQuery) : base(pagedList, pagingQuery) {
            PagedList = pagedList;
        }

        public PagedList<TRecord> PagedList { get; }
    }
}
