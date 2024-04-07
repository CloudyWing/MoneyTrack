using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    /// <summary>
    /// The sql parameter info.
    /// </summary>
    public class ParameterInfo {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterInfo"/> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ExceptionUtils"></exception>
        public ParameterInfo(string? parameterName, object? value) {
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => parameterName);
            ExceptionUtils.ThrowIfNull(() => value);

            ParameterName = parameterName;
            Value = value;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        /// <value>
        /// The name of the parameter.
        /// </value>
        public string ParameterName { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get; }
    }
}
