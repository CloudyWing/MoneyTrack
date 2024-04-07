using CloudyWing.MoneyTrack.Models.Domain.Statements;

namespace CloudyWing.MoneyTrack.Models.Domain {
    public class Specification<TEntity, TRecord>
        where TEntity : class where TRecord : class {
        public Action<SelectProvider<TEntity, TRecord>>? Selector { get; set; }

        public Func<FilterProvider<TEntity>, IOperatorStatement>? Filter { get; set; }

        public Action<OrderProvider<TEntity>>? Sorter { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
