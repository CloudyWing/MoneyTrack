using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CloudyWing.MoneyTrack.Models.DataAccess {
    public sealed class PagedList<T> : PagingMetadata, IEnumerable<T>, IEnumerable {
        public PagedList(IEnumerable<T> records, PagingMetadata metadata)
            : this(records, metadata.PageNumber, metadata.PageSize, metadata.TotalItemCount) { }

        private readonly IList<T> records;

        public PagedList(IEnumerable<T> records, int pageNumber, int pageSize, int totalItemCount)
            : base(pageNumber, pageSize, totalItemCount
        ) {
            this.records = records is IList<T> ? (records as IList<T> ?? new List<T>()) : records.ToList();
        }

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
