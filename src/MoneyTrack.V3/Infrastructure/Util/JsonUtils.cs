using System.Text.Json;

namespace CloudyWing.MoneyTrack.Util {
    public static class JsonUtils {
        public static string Serialize<T>(T value) {
            return JsonSerializer.Serialize(value);
        }

        public static T? Deserialize<T>(string value) {
            return JsonSerializer.Deserialize<T>(value);
        }
    }
}
