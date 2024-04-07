using System;

namespace CloudyWing.MoneyTrack.Models.Domain.CategoryModels {
    public class RecordEditor : EditorBase<long> {
        public RecordEditor() { }

        public RecordEditor(long id) : base(id) { }

        public ValueWatcher<long> CategoryId { get; set; }

        public ValueWatcher<DateTime> RecordDate { get; set; }

        public ValueWatcher<bool> IsIncome { get; set; }

        public ValueWatcher<long> Amount { get; set; }

        public ValueWatcher<string> Description { get; set; }
    }
}
