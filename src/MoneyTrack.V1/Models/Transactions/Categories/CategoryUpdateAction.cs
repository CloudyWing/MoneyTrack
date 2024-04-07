namespace CloudyWing.MoneyTrack.Models.Transactions.Categories {
    public class CategoryUpdateAction : ActionBase {
        public CategoryUpdateAction(long id) {
            Id = id;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public override bool Verify() {
            if (Name != null && Name.Trim().Length == 0) {
                ErrorMessage = "分類名稱不得為空白字元。";
                return false;
            }

            return true;
        }

        public override bool Execute() {
            DataUpdater updater = new DataUpdater(CategoryModel);
            updater.SetValue(CategoryModel.Id, Id);
            if (Name != null) {
                updater.SetNewValue(CategoryModel.Name, Name);
            }

            return updater.Update() > 0;
        }
    }
}
