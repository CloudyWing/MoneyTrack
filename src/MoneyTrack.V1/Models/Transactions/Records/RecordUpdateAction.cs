using System;

namespace CloudyWing.MoneyTrack.Models.Transactions.Records {
    public class RecordUpdateAction : ActionBase {
        public RecordUpdateAction(long id) {
            Id = id;
        }

        public long Id { get; set; }

        public long? CategoryId { get; set; }

        public DateTime? RecordDate { get; set; }

        public bool? IsIncome { get; set; }

        public long? Amount { get; set; }

        public string Description { get; set; }

        public override bool Verify() {
            if (Amount.HasValue && Amount.Value < 0) {
                ErrorMessage = "金額不能小於 0";

                return false;
            }

            return true;
        }

        public override bool Execute() {
            DataUpdater updater = new DataUpdater(RecordModel);
            updater.SetValue(RecordModel.Id, Id);

            if (CategoryId.HasValue) {
                updater.SetNewValue(RecordModel.CategoryId, CategoryId);
            }

            if (RecordDate.HasValue) {
                updater.SetNewValue(RecordModel.RecordDate, RecordDate);
            }

            if (IsIncome.HasValue) {
                updater.SetNewValue(RecordModel.IsIncome, IsIncome);
            }

            if (Amount.HasValue) {
                updater.SetNewValue(RecordModel.Amount, Amount);
            }

            if (Description != null) {
                updater.SetNewValue(RecordModel.Description, Description);
            }

            return updater.Update() > 0;
        }
    }
}
