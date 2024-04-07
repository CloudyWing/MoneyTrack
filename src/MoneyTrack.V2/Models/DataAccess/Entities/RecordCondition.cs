using System;

namespace CloudyWing.MoneyTrack.Models.DataAccess.Entities {
    public class RecordCondition {
        public ConditionColumn<long> Id { get; set; }

        public ConditionColumn<long> CategoryId { get; set; }

        public ConditionColumn<DateTime> RecordDate { get; set; }

        public ConditionColumn<bool> IsIncome { get; set; }

        public ConditionColumn<long> Amount { get; set; }

        public ConditionColumn<string> Description { get; set; }
    }
}
