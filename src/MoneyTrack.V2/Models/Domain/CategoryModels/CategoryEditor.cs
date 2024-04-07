namespace CloudyWing.MoneyTrack.Models.Domain.CategoryModels {
    public class CategoryEditor : EditorBase<long> {
        public CategoryEditor() { }

        public CategoryEditor(long id) : base(id) { }

        public ValueWatcher<string> Name { get; set; }
    }
}
