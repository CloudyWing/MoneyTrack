using System;
using System.Collections.Generic;
using System.Linq;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models {
    public sealed class WhereInfo {
        public WhereInfo() : this("", Enumerable.Empty<ParameterInfo>()) { }

        public WhereInfo(string whereClause, params ParameterInfo[] parameters)
            : this(whereClause, (IEnumerable<ParameterInfo>)parameters) {
        }

        public WhereInfo(string whereClause, IEnumerable<ParameterInfo> parameters) {
            ExceptionUtils.ThrowIfNull(() => whereClause);
            ExceptionUtils.ThrowIfNull(() => parameters);

            WhereClause = whereClause ?? throw new ArgumentNullException(nameof(whereClause));
            Parameters = (parameters as List<ParameterInfo> ?? parameters.ToList()).AsReadOnly();
        }

        public string WhereClause { get; }

        public IReadOnlyCollection<ParameterInfo> Parameters { get; }

        public class ParameterInfo {
            public ParameterInfo(string parameterName, object value) {
                ExceptionUtils.ThrowIfNullOrWhiteSpace(() => parameterName);
                ExceptionUtils.ThrowIfNull(() => value);

                ParameterName = parameterName;
                Value = value;
            }

            public string ParameterName { get; set; }

            public object Value { get; set; }
        }
    }
}
