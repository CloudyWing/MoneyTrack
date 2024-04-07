using System;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    [Flags]
    public enum SerializerOptions {
        None = 0,
        Formatted = 2,
        ConvertNullToEmpty = 4,
        CamelCase = 8
    }
}
