namespace CloudyWing.MoneyTrack.Models.Queriers {
    public interface ISqlColumnFilter {
        /// <summary>
        /// Interface for SQL column filter.
        /// </summary>
        DataFieldCollection Filter(DataFieldCollection fields);
    }
}
