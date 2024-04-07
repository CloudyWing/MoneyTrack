using System;

namespace CloudyWing.MoneyTrack.Models.DataAccess {
    [Serializable]
    public class PagingMetadata {
        public PagingMetadata(int pageNumber, int pageSize, int totalCount) {
            PageNumber = pageNumber;
            TotalItemCount = totalCount;
            PageSize = pageSize;
        }

        public int TotalItemCount { get; protected set; }

        public int PageNumber { get; protected set; }

        public int PageSize { get; protected set; }

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < PageCount;

        public int PageCount => PageSize == 0
                    ? 0
                    : (int)Math.Ceiling(TotalItemCount / (decimal)PageSize);

        public int FirstItemOnPage => ((PageNumber - 1) * PageSize) + 1;

        public int LastItemOnPage {
            get {
                int lastItemOnPage = FirstItemOnPage + PageSize - 1;
                return lastItemOnPage > TotalItemCount ?
                    TotalItemCount : lastItemOnPage;
            }
        }

        public bool IsFirstPage => PageNumber == 1;

        public bool IsLastPage => PageNumber >= PageCount;
    }
}
