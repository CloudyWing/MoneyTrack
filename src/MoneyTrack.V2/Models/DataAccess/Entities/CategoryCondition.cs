namespace CloudyWing.MoneyTrack.Models.DataAccess.Entities {
    public class CategoryCondition {
        public ConditionColumn<long> Id { get; set; }

        public ConditionColumn<string> Name { get; set; }

        public ConditionColumn<long> DisplayOrder { get; set; }
    }
}
