namespace CloudyWing.MoneyTrack.Models.Application {
    public class PagingQueryFieldsViewModel<TViewModel> : QueryFieldsViewModel<TViewModel> {
        public int PageNumber { get; set; } = 1;
    }
}
