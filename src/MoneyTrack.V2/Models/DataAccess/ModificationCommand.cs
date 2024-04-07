using System;

namespace CloudyWing.MoneyTrack.Models.DataAccess {
    public class ModificationCommand {
        public ModificationCommand(ModificationType modificationType, Func<int> commandAction) {
            ModificationType = modificationType;
            CommandAction = commandAction ?? throw new ArgumentNullException(nameof(commandAction));
        }

        public ModificationType ModificationType { get; }

        public Func<int> CommandAction { get; }
    }
}
