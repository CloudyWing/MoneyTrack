using CloudyWing.MoneyTrack.Models.Queriers;

namespace CloudyWing.MoneyTrack.Models.Transactions.Categories {
    public class CategoryInsertAction : ActionBase {
        public CategoryInsertAction(string name) {
            Name = name;
        }

        public string Name { get; set; }

        public override bool Verify() {
            if (string.IsNullOrWhiteSpace(Name)) {
                ErrorMessage = "分類名稱不得為 Null 或空白字元。";
                return false;
            }

            return true;
        }

        public override bool Execute() {
            SingleTableQuerier querier = new SingleTableQuerier(CategoryModel);
            int displayOrder = querier.GetCount() + 1;

            DataUpdater updater = new DataUpdater(CategoryModel);
            updater.SetValue(CategoryModel.Name, Name);
            updater.SetValue(CategoryModel.DisplayOrder, displayOrder);

            return updater.Insert() > 0;
        }
    }
}
