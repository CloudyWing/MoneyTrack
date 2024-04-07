using System.Diagnostics.CodeAnalysis;

namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    public static class OperatorStatementExtensions {
        public static OperatorStatement And(this IOperatorStatement operatorStatement, params IOperatorStatement[] otherOperatorStatement) {
            return And(operatorStatement, otherOperatorStatement as IEnumerable<IOperatorStatement>);
        }

        public static OperatorStatement And(this IOperatorStatement operatorStatement, IEnumerable<IOperatorStatement> otherOperatorStatement) {
            return new AndOperator(operatorStatement, otherOperatorStatement);
        }

        public static OperatorStatement Or(this IOperatorStatement operatorStatement, params IOperatorStatement[] otherOperatorStatement) {
            return Or(operatorStatement, otherOperatorStatement as IEnumerable<IOperatorStatement>);
        }

        public static OperatorStatement Or(this IOperatorStatement operatorStatement, IEnumerable<IOperatorStatement> otherOperatorStatement) {
            return new OrOperator(operatorStatement, otherOperatorStatement);
        }

        private sealed class AndOperator(IOperatorStatement operatorStatement, IEnumerable<IOperatorStatement> otherOperatorStatements)
            : OperatorStatement(operatorStatement.ColumnStatement) {
            private readonly IEnumerable<IOperatorStatement> operatorStatements = new IOperatorStatement[] { operatorStatement }.Union(otherOperatorStatements).Where(x => x != null);

            protected override void InitializeInternal([NotNull] out string statementString, [NotNull] out IEnumerable<ParameterInfo> parameters) {
                WhereClauseBuilder clauseBuilder = new();
                parameters = [];

                foreach (IOperatorStatement _operator in operatorStatements) {
                    clauseBuilder.AppendIfNotEmpty(_operator.StatementString);
                    parameters = parameters.Union(_operator.Parameters);
                }

                statementString = clauseBuilder.And;
            }
        }

        private sealed class OrOperator(IOperatorStatement operatorStatement, IEnumerable<IOperatorStatement> otherOperatorStatements)
            : OperatorStatement(operatorStatement.ColumnStatement) {
            private readonly IEnumerable<IOperatorStatement> operatorStatements = new IOperatorStatement[] { operatorStatement }.Union(otherOperatorStatements).Where(x => x != null);

            protected override void InitializeInternal([NotNull] out string statementString, [NotNull] out IEnumerable<ParameterInfo> parameters) {
                WhereClauseBuilder clauseBuilder = new();
                parameters = [];

                foreach (IOperatorStatement _operator in operatorStatements) {
                    clauseBuilder.AppendIfNotEmpty(_operator.StatementString);
                    parameters = parameters.Union(_operator.Parameters);
                }

                statementString = clauseBuilder.Or;
            }
        }
    }
}
