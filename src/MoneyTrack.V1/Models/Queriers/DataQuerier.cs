using System.Data;

namespace CloudyWing.MoneyTrack.Models.Queriers {
    /// <summary>
    /// Provides an abstract class for retrieving data from a data source.
    /// </summary>
    public abstract class DataQuerier {
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int PageNumber { set; get; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        public int PageSize { set; get; }

        /// <summary>
        /// Gets or sets the order by clause for the query.
        /// </summary>
        public virtual string OrderBy { get; set; }

        /// <summary>
        /// Determines whether the data Provider has any data.
        /// </summary>
        /// <returns>true if the data Provider has data; otherwise, false.</returns>
        public virtual bool HasData() {
            return GetCount() > 0;
        }

        /// <summary>
        /// Gets the number of records in the data Provider.
        /// </summary>
        /// <returns>The number of records in the data Provider.</returns>
        public virtual int GetCount() {
            return GetDataTable().Rows.Count;
        }

        /// <summary>
        /// Gets the data table for the data Provider.
        /// </summary>
        /// <returns>The data table for the data Provider.</returns>
        public virtual DataTable GetDataTable() {
            return new DataTable();
        }
    }
}
