using System;

namespace CloudyWing.MoneyTrack.Models.Transactions.Records {
    public class RecordInsertAction : ActionBase {
        public RecordInsertAction(long categoryId, DateTime recordDate, bool isIncome, long amount, string description) {
            CategoryId = categoryId;
            RecordDate = recordDate;
            IsIncome = isIncome;
            Amount = amount;
            Description = description;
        }

        public long CategoryId { get; set; }

        public DateTime RecordDate { get; private set; }

        public bool IsIncome { get; set; }

        public long Amount { get; set; }

        public string Description { get; set; }

        public override bool Verify() {
            if (Amount < 0) {
                ErrorMessage = "金額不能小於 0";

                return false;
            }

            return true;
        }

        public override bool Execute() {
            DataUpdater updater = new DataUpdater(RecordModel);
            updater.SetValue(RecordModel.CategoryId, CategoryId);
            updater.SetValue(RecordModel.RecordDate, RecordDate);
            updater.SetValue(RecordModel.IsIncome, IsIncome);
            updater.SetValue(RecordModel.Amount, Amount);
            updater.SetValue(RecordModel.Description, Description);

            return updater.Insert() > 0;
        }
    }
}
