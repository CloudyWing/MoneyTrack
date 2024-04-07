using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models.Domain {
    public class EditorBase<TKey> where TKey : struct {
        protected EditorBase() { }

        protected EditorBase(TKey id) {
            Id = id;
        }

        public TKey? Id { get; private set; }

        public void SetId(TKey? id) {
            ExceptionUtils.ThrowIfNull(() => id);

            Id = id;
        }
    }
}
