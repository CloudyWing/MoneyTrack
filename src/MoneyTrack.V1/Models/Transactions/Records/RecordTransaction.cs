using System;
using CloudyWing.MoneyTrack.Models.Transactions.Records;

namespace CloudyWing.MoneyTrack.Models.Transactions {
    public class RecordTransaction : DataTransaction {
        public RecordInsertAction CreateInsertAction(long categoryId, DateTime recordDate, bool isIncome, long amount, string description) {
            return SetAction(new RecordInsertAction(categoryId, recordDate, isIncome, amount, description));
        }

        public RecordUpdateAction CreateUpdateAction(int id) {
            return SetAction(new RecordUpdateAction(id));
        }

        public RecordDeleteAction CreateDeleteAction(int id) {
            return SetAction(new RecordDeleteAction(id));
        }
    }
}
