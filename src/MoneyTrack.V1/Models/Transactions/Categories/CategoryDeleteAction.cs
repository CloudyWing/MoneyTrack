using System.Data;
using CloudyWing.MoneyTrack.Models.Queriers;

namespace CloudyWing.MoneyTrack.Models.Transactions.Categories {
    public class CategoryDeleteAction : ActionBase {
        public CategoryDeleteAction(long id) {
            Id = id;
        }

        public long Id { get; }

        public override bool Verify() {
            SingleTableQuerier querier = new SingleTableQuerier(RecordModel);
            querier.AddCondition(RecordModel.CategoryId, Id);

            if (querier.HasData()) {
                ErrorMessage = $"{RecordModel.TableName} 已存在相關資料";
                return false;
            }

            return true;
        }

        public override bool Execute() {
            DataUpdater updater = new DataUpdater(CategoryModel);
            updater.SetValue(CategoryModel.Id, Id);
            if (updater.Delete() == 0) {
                return false;
            }

            SingleTableQuerier querier = new SingleTableQuerier(CategoryModel) {
                OrderBy = CategoryModel.DisplayOrder.NameWithBrackets
            };

            DataTable dataTable = querier.GetDataTable();
            int i = 1;
            foreach (DataRow row in dataTable.Rows) {
                updater.ReSet();
                updater.SetValue(CategoryModel.Id, row[CategoryModel.Id.Name]);
                updater.SetNewValue(CategoryModel.DisplayOrder, i++);
                if (updater.Update() == 0) {
                    return false;
                }
            }

            return true;
        }
    }
}
