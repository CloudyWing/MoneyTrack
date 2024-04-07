using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CloudyWing.Enumeration.Abstractions;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using Dapper;

namespace CloudyWing.MoneyTrack.Models {
    public sealed class ConditionParser<TCondition> {
        public ConditionParser(TCondition condition, WhereClauseBuilder whereClauseBuilder, DynamicParameters parameters) {
            ExceptionUtils.ThrowIfNull(() => whereClauseBuilder);
            ExceptionUtils.ThrowIfNull(() => parameters);

            Condition = condition;
            WhereClauseBuilder = whereClauseBuilder;
            Parameters = parameters;
        }

        public TCondition Condition { get; }

        public WhereClauseBuilder WhereClauseBuilder { get; }

        public DynamicParameters Parameters { get; }

        public void ParseColumnToWhereInfo<T>(Expression<Func<TCondition, ConditionColumn<T>>> source, string columnName) {
            // 因為 GUID 沒有 implement IConvertible，所以在這邊做判斷
            ValidateType<T>();

            Func<TCondition, ConditionColumn<T>> func = source.Compile();
            ConditionColumn<T> property = func(Condition);

            if (property is null) {
                return;
            }

            string parameterName = GetParameterNameByExpression(source);
            WhereInfo info = property.Operator.CreateWhereInfo(columnName, parameterName);

            BuildParameters<T>(info);

            WhereClauseBuilder.AppendIfNotEmpty(info.WhereClause);
        }

        private static void ValidateType<T>() {
            Type baseType = typeof(T);
            Type underlyingType = Nullable.GetUnderlyingType(baseType);
            Type keyType = underlyingType ?? baseType;

            if (!typeof(IConvertible).IsAssignableFrom(keyType) && keyType != typeof(Guid)) {
                throw new ArgumentException("Invalid type. The type must be IConvertible or Guid.", nameof(T));
            }
        }

        private void BuildParameters<T>(WhereInfo info) {
            Type superBaseType = typeof(T).BaseType;
            bool isEnumeration = superBaseType.IsGenericType
                && superBaseType.GetGenericTypeDefinition() == typeof(EnumerationBase<,>);

            foreach (var param in info.Parameters) {
                bool isMultiValue = param.Value is IEnumerable<T>;

                object value = typeof(T).IsEnum
                    ? FixValueFromEnum<T>(param.Value, isMultiValue)
                    : isEnumeration
                        ? FixValueFromEnumeration<T>(param.Value, superBaseType.GetGenericArguments()[1], isMultiValue)
                        : param.Value;
                Parameters.Add(param.ParameterName, value);
            }
        }

        private static object FixValueFromEnum<T>(object value, bool isMultiValue) {
            return isMultiValue
                ? (object)(value as IEnumerable<T>).Select(x => Convert.ToInt32(x))
                : (int)value;
        }

        private static object FixValueFromEnumeration<T>(object value, Type genericType, bool isMultiValue) {
            return isMultiValue
                ? (value as IEnumerable<T>)
                    .Select(x => Convert.ChangeType(value, genericType))
                : Convert.ChangeType(value, genericType);
        }

        public static string GetParameterNameByExpression<TOut>(Expression<Func<TCondition, TOut>> expression) {
            ExceptionUtils.ThrowIfNull(() => expression);

            if (expression.Body is ConstantExpression constant && constant.Value is string) {
                return constant.Value as string;
            }

            MemberExpression member = expression.Body as MemberExpression ??
                (expression.Body is UnaryExpression unary ? unary.Operand as MemberExpression : null);

            return member is null
                ? throw new ArgumentException("Invalid expression format.", nameof(expression))
                : member.Member.Name;
        }

        /// <summary>
        /// The custom clause format: {0:column where clause}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="customClause">The custom clause.</param>
        /// <param name="columnName">Name of the column.</param>
        public void ParseCustomClauseToWhereInfo<T>(
            Expression<Func<TCondition, ConditionColumn<T>>> source,
            string customClause, string columnName
        ) {
            // 因為 GUID 沒有 implement IConvertible，所以在這邊做判斷
            ValidateType<T>();

            Func<TCondition, ConditionColumn<T>> func = source.Compile();
            ConditionColumn<T> property = func(Condition);

            if (property is null) {
                return;
            }

            string parameterName = GetParameterNameByExpression(source);
            WhereInfo info = property.Operator.CreateWhereInfo(columnName, parameterName);

            if (string.IsNullOrWhiteSpace(info.WhereClause)) {
                return;
            }

            BuildParameters<T>(info);

            WhereClauseBuilder.Append(string.Format(customClause, info.WhereClause));
        }
    }
}
