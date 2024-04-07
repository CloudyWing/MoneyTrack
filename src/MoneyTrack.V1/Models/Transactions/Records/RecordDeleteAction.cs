namespace CloudyWing.MoneyTrack.Models.Transactions.Records {
    public class RecordDeleteAction : ActionBase {
        public RecordDeleteAction(int id) {
            Id = id;
        }

        public int Id { get; }

        public override bool Execute() {
            DataUpdater updater = new DataUpdater(RecordModel);
            updater.SetValue(RecordModel.Id, Id);
            return updater.Delete() > 0;
        }
    }
}
