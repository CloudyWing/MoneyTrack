using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models {
    public static class OperatorUtils {
        public static ColumnOperator<T> Null<T>() {
            return new NullOperator<T>();
        }
        public static ColumnOperator<T> NotNull<T>() {
            return new NotNullOperator<T>();
        }

        public static ColumnOperator<T>? Equals<T>(params T?[]? values) where T : struct {
            return Equals(values as IEnumerable<T?>);
        }

        public static ColumnOperator<T>? Equals<T>(IEnumerable<T?>? values) where T : struct {
            return values is null ? null : Equals(values.Where(x => x.HasValue).Select(x => x!.Value));
        }

        public static ColumnOperator<string?>? Equals(params string?[]? values) {
            return Equals<string?>(values as IEnumerable<string>);
        }

        public static ColumnOperator<string?>? Equals(IEnumerable<string?>? values) {
            return values is null ? null : Equals<string?>(values.Select(x => x?.Trim()));
        }

        public static ColumnOperator<T>? Equals<T>(params T[]? values) {
            return Equals(values as IEnumerable<T>);
        }

        public static ColumnOperator<T>? Equals<T>(IEnumerable<T>? values) {
            if (values is null) {
                return null;
            }

            IEnumerable<T> list = values.Where(x => x != null)!;

            if (list.Any()) {
                return new EqualsOperator<T>(list);
            }

            return null;
        }

        public static ColumnOperator<T>? NotEquals<T>(params T?[]? values) where T : struct {
            return NotEquals(values as IEnumerable<T?>);
        }

        public static ColumnOperator<T>? NotEquals<T>(IEnumerable<T?>? values) where T : struct {
            return values is null ? null : NotEquals(values.Where(x => x.HasValue).Select(x => x!.Value));
        }

        public static ColumnOperator<string?>? NotEquals(params string?[]? values) {

            return NotEquals<string?>(values as IEnumerable<string?>);
        }

        public static ColumnOperator<string?>? NotEquals(IEnumerable<string?>? values) {
            return values is null ? null : NotEquals<string?>(values.Select(x => x?.Trim()));
        }

        public static ColumnOperator<T>? NotEquals<T>(params T?[]? values) {
            return NotEquals(values as IEnumerable<T>);
        }

        public static ColumnOperator<T>? NotEquals<T>(IEnumerable<T>? values) {
            if (values is null) {
                return null;
            }

            IEnumerable<T> list = values.Where(x => x != null);
            if (list.Any()) {
                return new NotEqualsOperator<T>(list);
            }

            return null;
        }

        public static ColumnOperator<T>? GreaterThan<T>(T? value) where T : struct, IConvertible {
            return value is null ? null : GreaterThan(value.Value);
        }

        public static ColumnOperator<T>? GreaterThan<T>(T value) where T : IConvertible {
            return value == null ? null : new GenericreOperator<T>(value, "GreaterThan", "> @{0}");
        }

        public static ColumnOperator<T>? GreaterThanOrEquals<T>(T? value) where T : struct, IConvertible {
            return value is null ? null : GreaterThanOrEquals(value.Value);
        }

        public static ColumnOperator<T>? GreaterThanOrEquals<T>(T value) where T : IConvertible {
            return value == null ? null : new GenericreOperator<T>(value, "GreaterThanOrEquals", ">= @{0}");
        }

        public static ColumnOperator<T>? LessThan<T>(T? value) where T : struct, IConvertible {
            return value is null ? null : LessThan(value.Value);
        }

        public static ColumnOperator<T>? LessThan<T>(T value) where T : IConvertible {
            return value == null ? null : new GenericreOperator<T>(value, "LessThan", "< @{0}");
        }

        public static ColumnOperator<T>? LessThanOrEquals<T>(T? value) where T : struct, IConvertible {
            return value is null ? null : LessThanOrEquals(value.Value);
        }

        public static ColumnOperator<T>? LessThanOrEquals<T>(T value) where T : IConvertible {
            return value == null ? null : new GenericreOperator<T>(value, "LessThanOrEquals", "<= @{0}");
        }

        public static ColumnOperator<string>? Contains(string? value) {
            return value is null ? null : new GenericreOperator<string>(value, "Contains", "LIKE '%' + @{0} + '%'");
        }

        public static ColumnOperator<string>? NotContains(string? value) {
            return value is null ? null : new GenericreOperator<string>(value, "NotContains", "NOT LIKE '%' + @{0} + '%'");
        }

        public static ColumnOperator<string>? StartsWith(string? value) {
            return value is null ? null : new GenericreOperator<string>(value, "StartsWith", "LIKE @{0} + '%'");
        }

        public static ColumnOperator<string>? NotStartsWith(string? value) {
            return value is null ? null : new GenericreOperator<string>(value, "NotStartsWith", "NOT LIKE @{0} + '%'");
        }

        public static ColumnOperator<string>? EndsWith(string? value) {
            return value is null ? null : new GenericreOperator<string>(value, "EndsWith", "LIKE '%' + @{0}");
        }

        public static ColumnOperator<string>? NotEndsWith(string value) {
            return value is null ? null : new GenericreOperator<string>(value, "NotEndsWith", "NOT LIKE '%' + @{0}");
        }

        public static ColumnOperator<T>? And<T>(
            ColumnOperator<T>? currentOperator, IEnumerable<ColumnOperator<T>?> combinedOperators
        ) {
            if (combinedOperators is null) {
                return currentOperator;
            }
            return And(CombineOperators(currentOperator, combinedOperators));
        }

        public static ColumnOperator<T>? And<T>(params ColumnOperator<T>?[] values) {
            return And(values as IEnumerable<ColumnOperator<T>>);
        }

        public static ColumnOperator<T>? And<T>(IEnumerable<ColumnOperator<T>?> values) {
            IEnumerable<ColumnOperator<T>?> operators = values.Where(x => x != null);
            return operators.Count() switch {
                0 => null,
                1 => operators.Single(),
                _ => new AndOperator<T>(values),
            };
        }

        public static ColumnOperator<T>? Or<T>(
            ColumnOperator<T>? currentOperator, IEnumerable<ColumnOperator<T>?> combinedOperators
        ) {
            if (combinedOperators.Any(x => x != null)) {
                return Or(CombineOperators(currentOperator, combinedOperators));
            }

            return currentOperator;
        }

        public static ColumnOperator<T>? Or<T>(params ColumnOperator<T>?[] values) {
            return Or(values as IEnumerable<ColumnOperator<T>>);
        }

        public static ColumnOperator<T>? Or<T>(IEnumerable<ColumnOperator<T>?> values) {
            IEnumerable<ColumnOperator<T>?> operators = values.Where(x => x != null);
            return operators.Count() switch {
                0 => null,
                1 => operators.Single(),
                _ => new OrOperator<T>(values)
            };
        }

        private static IEnumerable<ColumnOperator<T>?> CombineOperators<T>(
            ColumnOperator<T>? currentOperator, IEnumerable<ColumnOperator<T>?> operators
        ) {
            if (currentOperator != null) {
                yield return currentOperator;
            }

            foreach (ColumnOperator<T>? _operator in operators) {
                if (_operator != null) {
                    yield return _operator;
                }
            }
        }

        private static string CreatePostfix() {
            return Guid.NewGuid().ToString().Replace("-", "")[..5];
        }

        private sealed class NullOperator<T> : ColumnOperator<T> {
            public override WhereInfo CreateWhereInfoInternal(string columnName, string parameterName) {
                return new WhereInfo($"{columnName} IS NULL");
            }
        }

        private sealed class NotNullOperator<T> : ColumnOperator<T> {
            public override WhereInfo CreateWhereInfoInternal(string columnName, string parameterName) {
                return new WhereInfo($"{columnName} IS NOT NULL");
            }
        }

        private sealed class EqualsOperator<T> : ColumnOperator<T> {
            public EqualsOperator(params T[] values) : this(values as IEnumerable<T>) { }

            public EqualsOperator(IEnumerable<T> values) {
                ExceptionUtils.ThrowIfNull(() => values);

                Values = values.Where(x => x != null);
            }

            public IEnumerable<T> Values { get; }

            public override WhereInfo CreateWhereInfoInternal(string columnName, string parameterName) {
                string clause = "";
                List<WhereInfo.ParameterInfo> parameters = [];
                if (Values?.Any() == true) {
                    string prefixParameterName = $"Equal_{parameterName}_{CreatePostfix()}";
                    if (Values.Count() == 1) {
                        clause = $"{columnName} = @{prefixParameterName}";
                        parameters.Add(new WhereInfo.ParameterInfo(prefixParameterName, Values.Single()!));
                    } else {
                        clause = $"{columnName} IN @{prefixParameterName}";
                        parameters.Add(new WhereInfo.ParameterInfo(prefixParameterName, Values));

                    }
                }
                return new WhereInfo(clause, parameters);
            }
        }

        private sealed class NotEqualsOperator<T> : ColumnOperator<T> {
            public NotEqualsOperator(params T[] values) : this(values as IEnumerable<T>) { }

            public NotEqualsOperator(IEnumerable<T> values) {
                ExceptionUtils.ThrowIfNull(() => values);

                Values = values.Where(x => x != null);
            }

            public IEnumerable<T> Values { get; }

            public override WhereInfo CreateWhereInfoInternal(string columnName, string parameterName) {
                string clause = "";
                List<WhereInfo.ParameterInfo> parameters = [];
                if (Values?.Any() == true) {
                    string prefixParameterName = $"NotEqual_{parameterName}_{CreatePostfix()}";
                    if (Values.Count() == 1) {
                        clause = $"{columnName} <> @{prefixParameterName}";
                        parameters.Add(new WhereInfo.ParameterInfo(prefixParameterName, Values.Single()!));
                    } else {
                        clause = $"{columnName} NOT IN @{prefixParameterName}";
                        parameters.Add(new WhereInfo.ParameterInfo(prefixParameterName, Values));

                    }
                }
                return new WhereInfo(clause, parameters);
            }
        }

        private class GenericreOperator<T> : ColumnOperator<T> where T : IConvertible {
            private readonly T value;
            private readonly string prefix;
            public readonly string pattern;

            public GenericreOperator(T value, string prefix, string pattern) {
                ExceptionUtils.ThrowIfNull(() => prefix);
                ExceptionUtils.ThrowIfNull(() => pattern);

                this.value = value;
                this.prefix = prefix;
                this.pattern = pattern;
            }

            public override WhereInfo CreateWhereInfoInternal(string columnName, string parameterName) {
                if (value == null) {
                    return new WhereInfo();
                }

                string prefixParameterName = $"{prefix}_{parameterName}_{CreatePostfix()}";
                string format = $"{columnName} {pattern}";

                string whereClause = string.Format(format, prefixParameterName);
                return new WhereInfo(whereClause, new WhereInfo.ParameterInfo(prefixParameterName, value));
            }
        }

        private sealed class AndOperator<T>(IEnumerable<ColumnOperator<T>?> columnOperators) : ColumnOperator<T> {
            private readonly IEnumerable<ColumnOperator<T>> columnOperators = columnOperators.Where(x => x != null)!;

            public override WhereInfo CreateWhereInfoInternal(string columnName, string parameterName) {
                WhereClauseBuilder clauseBuilder = new();
                IEnumerable<WhereInfo.ParameterInfo> parameters = Enumerable.Empty<WhereInfo.ParameterInfo>();
                foreach (ColumnOperator<T> _operator in columnOperators) {
                    WhereInfo info = _operator.CreateWhereInfo(columnName, parameterName);
                    clauseBuilder.AppendIfNotEmpty(info.WhereClause);
                    parameters = parameters.Union(info.Parameters);
                }

                return new WhereInfo(clauseBuilder.And, parameters);
            }
        }

        private sealed class OrOperator<T>(IEnumerable<ColumnOperator<T>?> columnOperators) : ColumnOperator<T> {
            private readonly IEnumerable<ColumnOperator<T>> columnOperators = columnOperators.Where(x => x != null)!;

            public override WhereInfo CreateWhereInfoInternal(string columnName, string parameterName) {
                WhereClauseBuilder clauseBuilder = new();
                IEnumerable<WhereInfo.ParameterInfo> parameters = Enumerable.Empty<WhereInfo.ParameterInfo>();
                foreach (ColumnOperator<T> _operator in columnOperators) {
                    WhereInfo info = _operator.CreateWhereInfo(columnName, parameterName);
                    clauseBuilder.AppendIfNotEmpty(info.WhereClause);
                    parameters = parameters.Union(info.Parameters);
                }

                return new WhereInfo(clauseBuilder.Or, parameters);
            }
        }
    }
}
