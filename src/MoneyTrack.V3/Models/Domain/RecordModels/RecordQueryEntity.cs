using CloudyWing.MoneyTrack.Models.DataAccess.Entities;

namespace CloudyWing.MoneyTrack.Models.Domain.RecordModels {
    public class RecordQueryEntity {
        public Record? Record { get; set; }

        public Category? Category { get; set; }
    }
}
