namespace System {
    public static class DateTimeExtensions {
        public static string ToDateString(this DateTime dateTime) {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string ToDateString(this DateTime? dateTime) {
            return dateTime.HasValue ? "" : ToDateString(dateTime.Value);
        }

        public static string ToDateTimeString(this DateTime dateTime) {
            return dateTime.ToString("yyyy-MM-ddTHH:mm");
        }

        public static string ToDateTimeString(this DateTime? dateTime) {
            return dateTime.HasValue ? "" : ToDateTimeString(dateTime.Value);
        }
    }
}
