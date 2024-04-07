namespace CloudyWing.MoneyTrack.Models.DataAccess {
    public class ModificationCommand(ModificationType modificationType, Func<Task<int>> commandAction) {
        public ModificationType ModificationType { get; } = modificationType;

        public Func<Task<int>> CommandActionAsync { get; } = commandAction ?? throw new ArgumentNullException(nameof(commandAction));
    }
}
