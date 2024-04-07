using System.Collections;

namespace CloudyWing.MoneyTrack.Models.DataAccess {
    public sealed class PagedList<T>(IEnumerable<T> records, int pageNumber, int pageSize, int totalItemCount)
        : PagingMetadata(pageNumber, pageSize, totalItemCount), IEnumerable<T>, IEnumerable {
        private readonly IList<T> records = records is IList<T>
            ? (records as IList<T> ?? new List<T>())
            : records.ToList();

        public PagedList(IEnumerable<T> records, PagingMetadata metadata)
            : this(records, metadata.PageNumber, metadata.PageSize, metadata.TotalItemCount) { }


        public T this[int index] => records[index];

        /// <summary>
        /// Gets the page count.
        /// </summary>
        public int Count => records.Count;

        public IEnumerator<T> GetEnumerator() {
            return records.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return records.GetEnumerator();
        }
    }
}
