using System;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models {
    /// <remarks>
    /// Because of the implicit conversions of ConditionColumn and ParameterInfo, only the abstract class is defined, not the interface.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public abstract class ColumnOperator<T> {
        public static implicit operator ColumnOperator<T>(DBNull value) {
            return OperatorUtils.Null<T>();
        }

        public static implicit operator ColumnOperator<T>(T value) {
            return OperatorUtils.Equals(value);
        }

        public static implicit operator ColumnOperator<T>(T[] values) {
            return OperatorUtils.Equals(values);
        }

        public static ColumnOperator<T> operator &(ColumnOperator<T> left, ColumnOperator<T> right) {
            return OperatorUtils.And(left, right);
        }

        public static ColumnOperator<T> operator &(ColumnOperator<T> left, T right) {
            return left & OperatorUtils.Equals(right);
        }

        public static ColumnOperator<T> operator &(T left, ColumnOperator<T> right) {
            return OperatorUtils.Equals(left) & right;
        }

        public static ColumnOperator<T> operator &(ColumnOperator<T> left, DBNull right) {
            return left & OperatorUtils.Null<T>();
        }

        public static ColumnOperator<T> operator &(DBNull left, ColumnOperator<T> right) {
            return OperatorUtils.Null<T>() & right;
        }

        public static ColumnOperator<T> operator |(ColumnOperator<T> left, ColumnOperator<T> right) {
            return OperatorUtils.Or(left, right);
        }

        public static ColumnOperator<T> operator |(ColumnOperator<T> left, T right) {
            return left | OperatorUtils.Equals(right);
        }

        public static ColumnOperator<T> operator |(T left, ColumnOperator<T> right) {
            return OperatorUtils.Equals(left) | right;
        }

        public static ColumnOperator<T> operator |(ColumnOperator<T> left, DBNull right) {
            return left | OperatorUtils.Null<T>();
        }

        public static ColumnOperator<T> operator |(DBNull left, ColumnOperator<T> right) {
            return OperatorUtils.Null<T>() | right;
        }

        public WhereInfo CreateWhereInfo(string columnName, string parameterName) {
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => columnName);
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => parameterName);

            return CreateWhereInfoInternal(columnName, parameterName);
        }

        public abstract WhereInfo CreateWhereInfoInternal(string columnName, string parameterName);
    }
}
