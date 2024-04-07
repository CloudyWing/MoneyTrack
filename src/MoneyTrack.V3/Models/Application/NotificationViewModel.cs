namespace CloudyWing.MoneyTrack.Models.Application {
    public class NotificationViewModel(string message, NotificationLevel level = NotificationLevel.None) {
        public NotificationViewModel(ApplicationResult result)
            : this(result?.Message ?? "", result?.IsOk ?? true) { }

        public NotificationViewModel(string message, bool isOk)
            : this(message, isOk ? NotificationLevel.Success : NotificationLevel.Warning) { }

        public string Message { get; set; } = message;

        public NotificationLevel Level { get; set; } = level;
    }
}
