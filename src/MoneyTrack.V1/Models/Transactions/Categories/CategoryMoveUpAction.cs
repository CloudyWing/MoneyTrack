using System;
using System.Data;
using System.Data.SqlClient;
using CloudyWing.MoneyTrack.Models.Queriers;

namespace CloudyWing.MoneyTrack.Models.Transactions.Categories {
    public class CategoryMoveUpAction : ActionBase {
        public CategoryMoveUpAction(long id) {
            Id = id;
        }

        public long Id { get; set; }

        public override bool Execute() {
            SingleTableQuerier querier = new SingleTableQuerier(CategoryModel);
            querier.AddCondition(CategoryModel.Id, Id);
            long displayOrder = 1;

            using (SqlDataReader dr = querier.GetDataReader(CommandBehavior.SingleRow)) {
                if (dr.Read()) {
                    displayOrder = Convert.ToInt64(dr[CategoryModel.DisplayOrder.Name]);
                }
            }

            if (displayOrder == 1) {
                ErrorMessage = "已是第一筆";
                return false;
            }

            DataUpdater updater = new DataUpdater(CategoryModel);
            updater.SetMultiValue(CategoryModel.DisplayOrder, displayOrder - 1);
            updater.SetNewValue(CategoryModel.DisplayOrder, displayOrder);
            updater.IsUnique = false;
            if (updater.Update() == 0) {
                return false;
            }

            updater.ReSet();
            updater.SetValue(CategoryModel.Id, Id);
            updater.SetNewValue(CategoryModel.DisplayOrder, displayOrder - 1);
            return updater.Update() > 0;
        }
    }
}
