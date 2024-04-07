using System.Diagnostics.CodeAnalysis;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    /// <summary>
    /// The <c>OperatorColumn</c> extension methods.
    /// </summary>
    internal static class OperatorColumnExtensions {
        /// <summary>
        /// Indicates that the column is null.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TColumn">The type of Indicates that the column.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement Null<TEntity, TColumn>(this OperatorColumn<TEntity, TColumn> column) {
            return new NullOperator(column.ColumnStatement);
        }

        /// <summary>
        /// Indicates that the column is not null.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TColumn">The type of Indicates that the column.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement NotNull<TEntity, TColumn>(this OperatorColumn<TEntity, TColumn> column) {
            return new NotNullOperator(column.ColumnStatement);
        }

        /// <summary>
        /// Indicates that the column equals to value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TColumn">The type of Indicates that the column.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement Equal<TEntity, TColumn>(this OperatorColumn<TEntity, TColumn> column, params TColumn[] values) {
            return Equal(column, values as IEnumerable<TColumn>);
        }

        /// <summary>
        /// Indicates that the column equals to value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TColumn">The type of Indicates that the column.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement Equal<TEntity, TColumn>(this OperatorColumn<TEntity, TColumn> column, IEnumerable<TColumn> values) {
            return new EqualOperator<TColumn>(column.ColumnStatement, values);
        }

        /// <summary>
        /// Indicates that the column equals to value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="values">The values.</param>
        /// 
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement Equal<TEntity>(this OperatorColumn<TEntity, string> column, params string[] values) {
            return Equal(column, values as IEnumerable<string>);
        }

        /// <summary>
        /// Indicates that the column equals to value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="values">The values.</param>
        /// 
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement Equal<TEntity>(this OperatorColumn<TEntity, string> column, IEnumerable<string> values) {
            return new EqualOperator(column.ColumnStatement, values);
        }

        /// <summary>
        /// Indicates that the column not equals to value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TColumn">The type of Indicates that the column.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement NotEqual<TEntity, TColumn>(this OperatorColumn<TEntity, TColumn> column, params TColumn[] values) {
            return NotEqual(column, values as IEnumerable<TColumn>);
        }

        /// <summary>
        /// Indicates that the column not equals to value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TColumn">The type of Indicates that the column.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement NotEqual<TEntity, TColumn>(this OperatorColumn<TEntity, TColumn> column, IEnumerable<TColumn> values) {
            return new NotEqualOperator<TColumn>(column.ColumnStatement, values);
        }

        /// <summary>
        /// Indicates that the column not equals to value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="values">The values.</param>
        /// 
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement NotEqual<TEntity>(this OperatorColumn<TEntity, string> column, params string[] values) {
            return NotEqual(column, values as IEnumerable<string>);
        }

        /// <summary>
        /// Indicates that the column not equals to value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="values">The values.</param>
        /// 
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement NotEqual<TEntity>(this OperatorColumn<TEntity, string> column, IEnumerable<string> values) {
            return new NotEqualOperator(column.ColumnStatement, values);
        }

        /// <summary>
        /// Indicates that the column not greater than value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TColumn">The type of Indicates that the column.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement Greater<TEntity, TColumn>(this OperatorColumn<TEntity, TColumn> column, TColumn value) {
            return new GenericreOperator<TColumn>(column.ColumnStatement, value, nameof(Greater), "> @{0}");
        }

        /// <summary>
        /// Indicates that the column not greater than or equals to value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TColumn">The type of Indicates that the column.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement GreaterOrEqual<TEntity, TColumn>(this OperatorColumn<TEntity, TColumn> column, TColumn value) {
            return new GenericreOperator<TColumn>(column.ColumnStatement, value, nameof(GreaterOrEqual), ">= @{0}");
        }

        public static OperatorStatement Less<TEntity, TColumn>(this OperatorColumn<TEntity, TColumn> column, TColumn value) {
            return new GenericreOperator<TColumn>(column.ColumnStatement, value, nameof(Less), "< @{0}");
        }

        /// <summary>
        /// Indicates that the column not less than value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TColumn">The type of Indicates that the column.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement LessOrEqual<TEntity, TColumn>(this OperatorColumn<TEntity, TColumn> column, TColumn value) {
            return new GenericreOperator<TColumn>(column.ColumnStatement, value, nameof(LessOrEqual), "<= @{0}");
        }

        /// <summary>
        /// Indicates that the column contains value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement Contain<TEntity>(this OperatorColumn<TEntity, string> column, string? value) {
            return new GenericreOperator<string>(column.ColumnStatement, value, nameof(Contain), "LIKE '%' + @{0} + '%'");
        }

        /// <summary>
        /// Indicates that the column not contains value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement NotContain<TEntity>(this OperatorColumn<TEntity, string> column, string? value) {
            return new GenericreOperator<string>(column.ColumnStatement, value, nameof(NotContain), "NOT LIKE '%' + @{0} + '%'");
        }

        /// <summary>
        /// Indicates that the column start withs value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement StartWith<TEntity>(this OperatorColumn<TEntity, string> column, string? value) {
            return new GenericreOperator<string>(column.ColumnStatement, value, nameof(StartWith), "LIKE @{0} + '%'");
        }

        /// <summary>
        /// Indicates that the column not starts with value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement NotStartWith<TEntity>(this OperatorColumn<TEntity, string> column, string? value) {
            return new GenericreOperator<string>(column.ColumnStatement, value, nameof(NotStartWith), "NOT LIKE @{0} + '%'");
        }

        /// <summary>
        /// Indicates that the column ends with value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement EndWith<TEntity>(this OperatorColumn<TEntity, string> column, string? value) {
            return new GenericreOperator<string>(column.ColumnStatement, value, nameof(EndWith), "LIKE '%' + @{0}");
        }

        /// <summary>
        /// Indicates that the column not ends with value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="column">Indicates that the column.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <c>OperatorStatement</c>.</returns>
        public static OperatorStatement NotEndWith<TEntity>(this OperatorColumn<TEntity, string> column, string? value) {
            return new GenericreOperator<string>(column.ColumnStatement, value, nameof(NotEndWith), "NOT LIKE '%' + @{0}");
        }

        private static string CreatePostfix() {
            return Guid.NewGuid().ToString().Replace("-", "")[..5];
        }

        private sealed class EmptyOperator(IColumnStatement columnStatement)
            : OperatorStatement(columnStatement) {
            protected override void InitializeInternal([NotNull] out string statementString, [NotNull] out IEnumerable<ParameterInfo> parameters) {
                statementString = "";
                parameters = [];
            }
        }

        private sealed class NullOperator(IColumnStatement columnStatement)
            : OperatorStatement(columnStatement) {
            protected override void InitializeInternal([NotNull] out string statementString, [NotNull] out IEnumerable<ParameterInfo> parameters) {
                statementString = $"{ColumnStatement} IS NULL";
                parameters = [];
            }
        }

        private sealed class NotNullOperator(IColumnStatement columnStatement)
            : OperatorStatement(columnStatement) {
            protected override void InitializeInternal([NotNull] out string statementString, [NotNull] out IEnumerable<ParameterInfo> parameters) {
                statementString = $"{ColumnStatement} IS NOT NULL";
                parameters = [];
            }
        }

        private sealed class EqualOperator<TValue> : OperatorStatement {
            public EqualOperator(IColumnStatement columnStatement, [NotNull] IEnumerable<TValue>? values) : base(columnStatement) {
                ExceptionUtils.ThrowIfNull(() => values);

                Values = values.Where(x => x != null).ToList().AsReadOnly();
            }

            public IReadOnlyList<TValue> Values { get; }

            protected override void InitializeInternal([NotNull] out string statementString, [NotNull] out IEnumerable<ParameterInfo>? parameters) {
                List<ParameterInfo> _parameters = [];
                statementString = "";

                if (Values.Any() == true) {
                    string parameterName = $"Equal_{ColumnStatement.ColumnStatementString.Replace(".", "_")}_{CreatePostfix()}";
                    if (Values.Count == 1) {
                        statementString = $"{ColumnStatement} = @{parameterName}";
                        _parameters.Add(new ParameterInfo(parameterName, Values.Single()!));
                    } else {
                        statementString = $"{ColumnStatement} IN @{parameterName}";
                        _parameters.Add(new ParameterInfo(parameterName, Values));

                    }
                }

                parameters = _parameters;
            }
        }

        private sealed class EqualOperator : OperatorStatement {
            public EqualOperator(IColumnStatement columnStatement, [NotNull] IEnumerable<string>? values) : base(columnStatement) {
                ExceptionUtils.ThrowIfNull(() => values);

                Values = values.Where(x => x != null).ToList().AsReadOnly();
            }

            public IReadOnlyList<string> Values { get; }

            protected override void InitializeInternal([NotNull] out string statementString, [NotNull] out IEnumerable<ParameterInfo>? parameters) {
                List<ParameterInfo> _parameters = [];
                statementString = "";

                if (Values.Any() == true) {
                    string parameterName = $"Equal_{ColumnStatement.ColumnStatementString.Replace(".", "_")}_{CreatePostfix()}";
                    if (Values.Count == 1) {
                        statementString = $"{ColumnStatement} = @{parameterName}";
                        _parameters.Add(new ParameterInfo(parameterName, Values.Single()!));
                    } else {
                        statementString = $"{ColumnStatement} IN @{parameterName}";
                        _parameters.Add(new ParameterInfo(parameterName, Values));

                    }
                }

                parameters = _parameters;
            }
        }

        private sealed class NotEqualOperator<TValue> : OperatorStatement {
            public NotEqualOperator(IColumnStatement columnStatement, params TValue[] values) : this(columnStatement, values as IEnumerable<TValue>) { }

            public NotEqualOperator(IColumnStatement columnStatement, IEnumerable<TValue> values) : base(columnStatement) {
                ExceptionUtils.ThrowIfNull(() => values);

                Values = values.Where(x => x != null).ToList().AsReadOnly();
            }

            public IReadOnlyList<TValue> Values { get; }

            protected override void InitializeInternal([NotNull] out string statementString, [NotNull] out IEnumerable<ParameterInfo> parameters) {
                List<ParameterInfo> _parameters = [];
                statementString = "";

                if (Values?.Any() == true) {
                    string parameterName = $"NotEqual_{ColumnStatement.ColumnStatementString.Replace(".", "_")}_{CreatePostfix()}";
                    if (Values.Count == 1) {
                        statementString = $"{ColumnStatement} <> @{parameterName}";
                        _parameters.Add(new ParameterInfo(parameterName, Values.Single()!));
                    } else {
                        statementString = $"{ColumnStatement} NOT IN @{parameterName}";
                        _parameters.Add(new ParameterInfo(parameterName, Values));

                    }
                }

                parameters = _parameters;
            }
        }

        private sealed class NotEqualOperator : OperatorStatement {
            public NotEqualOperator(IColumnStatement columnStatement, [NotNull] IEnumerable<string>? values) : base(columnStatement) {
                ExceptionUtils.ThrowIfNull(() => values);

                Values = values.Where(x => x != null).ToList().AsReadOnly();
            }

            public IReadOnlyList<string> Values { get; }

            public bool IgnoreCase { get; }

            protected override void InitializeInternal([NotNull] out string statementString, [NotNull] out IEnumerable<ParameterInfo>? parameters) {
                List<ParameterInfo> _parameters = [];
                statementString = "";

                if (Values.Any() == true) {
                    string parameterName = $"NotEqual_{ColumnStatement.ColumnStatementString.Replace(".", "_")}_{CreatePostfix()}";
                    if (Values.Count == 1) {
                        statementString = $"{ColumnStatement} <> @{parameterName}";
                        _parameters.Add(new ParameterInfo(parameterName, Values.Single()!));
                    } else {
                        statementString = $"{ColumnStatement} NOT IN @{parameterName}";
                        _parameters.Add(new ParameterInfo(parameterName, Values));

                    }
                }

                parameters = _parameters;
            }
        }

        private class GenericreOperator<TValue> : OperatorStatement {
            private readonly TValue value;
            private readonly string prefix;
            private readonly string pattern;

            public GenericreOperator(IColumnStatement columnStatement, TValue? value, string prefix, string pattern)
                : base(columnStatement) {
                ExceptionUtils.ThrowIfNull(() => prefix);
                ExceptionUtils.ThrowIfNull(() => pattern);

                this.value = value;
                this.prefix = prefix;
                this.pattern = pattern;
            }

            protected override void InitializeInternal([NotNull] out string statementString, [NotNull] out IEnumerable<ParameterInfo> parameters) {
                List<ParameterInfo> _parameters = [];
                parameters = _parameters;

                if (value is null) {
                    statementString = "";
                    return;
                }

                string parameterName = $"{prefix}_{ColumnStatement.ColumnStatementString.Replace(".", "_")}_{CreatePostfix()}";
                string format = $"{ColumnStatement} {pattern}";

                statementString = string.Format(format, parameterName);
                _parameters.Add(new ParameterInfo(parameterName, value));
            }
        }
    }
}
