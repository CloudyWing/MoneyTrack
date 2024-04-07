using CloudyWing.MoneyTrack.Models.DataAccess.Entities;

namespace CloudyWing.MoneyTrack.Models.Domain.CategoryModels {
    public class CategoryQueryEntity : Category {
        public long RecordCount { get; set; }
    }
}
