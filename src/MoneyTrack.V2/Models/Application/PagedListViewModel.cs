namespace CloudyWing.MoneyTrack.Models.Application {
    public class PagingQueryViewModel {
        public string FormId { get; set; } = "queryForm";

        public string PageNumberSelector { get; set; }
    }

    public class PagingQueryViewModel<TFields> : PagingQueryViewModel {
        public TFields Fields { get; set; }
    }
}
