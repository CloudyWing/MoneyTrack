using System;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models {
    public sealed class ConditionColumn<T> {
        private ConditionColumn(ColumnOperator<T> @operator) {
            ExceptionUtils.ThrowIfNull(() => @operator);

            Operator = @operator;
        }
        public ColumnOperator<T> Operator { get; private set; }

        public static implicit operator ConditionColumn<T>(DBNull value) {
            return OperatorUtils.Null<T>();
        }

        public static implicit operator ConditionColumn<T>(T value) {
            return OperatorUtils.Equals(value);
        }

        public static implicit operator ConditionColumn<T>(T[] values) {
            return OperatorUtils.Equals(values);
        }

        public static implicit operator ConditionColumn<T>(ColumnOperator<T> columnOperator) {
            return columnOperator is null ? null : new ConditionColumn<T>(columnOperator);
        }

        public static ConditionColumn<T> operator &(ConditionColumn<T> column, DBNull value) {
            column &= OperatorUtils.Null<T>();
            return column;
        }

        public static ConditionColumn<T> operator &(ConditionColumn<T> column, T value) {
            column &= OperatorUtils.Equals(value);
            return column;
        }

        public static ConditionColumn<T> operator &(ConditionColumn<T> column, T[] values) {
            column &= OperatorUtils.Equals(values);
            return column;
        }

        public static ConditionColumn<T> operator &(ConditionColumn<T> column, ColumnOperator<T> value) {
            column.Operator = OperatorUtils.And(column.Operator, new ColumnOperator<T>[] { value });
            return column;
        }

        public static ConditionColumn<T> operator |(ConditionColumn<T> column, DBNull value) {
            column |= OperatorUtils.Null<T>();
            return column;
        }

        public static ConditionColumn<T> operator |(ConditionColumn<T> column, T value) {
            column |= OperatorUtils.Equals(value);
            return column;
        }

        public static ConditionColumn<T> operator |(ConditionColumn<T> column, T[] values) {
            column |= OperatorUtils.Equals(values);
            return column;
        }

        public static ConditionColumn<T> operator |(ConditionColumn<T> column, ColumnOperator<T> value) {
            column.Operator = OperatorUtils.Or(column.Operator, new ColumnOperator<T>[] { value });
            return column;
        }
    }
}
