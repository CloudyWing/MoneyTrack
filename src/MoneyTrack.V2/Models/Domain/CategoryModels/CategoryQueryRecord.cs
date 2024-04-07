using CloudyWing.MoneyTrack.Models.DataAccess.Entities;

namespace CloudyWing.MoneyTrack.Models.Domain.CategoryModels {
    public class CategoryQueryRecord : Category {
        public long RecordCount { get; set; }
    }
}
