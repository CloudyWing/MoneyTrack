using System.Diagnostics.CodeAnalysis;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    /// <summary>
    /// Defines the basic class for the operator statement in a SQL query;
    /// </summary>
    /// <seealso cref="IOperatorStatement" />
    public abstract class OperatorStatement : IOperatorStatement {
        private bool isInitial;
        private string? statementString;
        private IReadOnlyList<ParameterInfo>? parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorStatement"/> class.
        /// </summary>
        /// <param name="columnStatement">The column statement.</param>
        protected OperatorStatement(IColumnStatement columnStatement) {
            ColumnStatement = columnStatement;
        }

        /// <inheritdoc/>
        public IColumnStatement ColumnStatement { get; }

        /// <inheritdoc/>
        public string StatementString {
            get {
                Initialize();

                return statementString;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<ParameterInfo> Parameters {
            get {
                Initialize();

                return parameters;
            }
        }

        [MemberNotNull(nameof(statementString))]
        [MemberNotNull(nameof(parameters))]
        private void Initialize() {
            if (!isInitial) {
                InitializeInternal(out statementString, out IEnumerable<ParameterInfo> _parameters);

                ExceptionUtils.ThrowIfNull(() => statementString);
                ExceptionUtils.ThrowIfNull(() => _parameters);

                parameters = _parameters.ToList().AsReadOnly();

                isInitial = true;
            }

            ExceptionUtils.ThrowIfNull(() => statementString);
            ExceptionUtils.ThrowIfNull(() => parameters);
        }

        /// <summary>
        /// Initializes the statement and parameters.
        /// </summary>
        /// <param name="statementString">The statement string.</param>
        /// <param name="parameters">The parameters.</param>
        protected abstract void InitializeInternal(out string statementString, out IEnumerable<ParameterInfo> parameters);

        /// <summary>
        /// Implements the operator &amp;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static OperatorStatement operator &(OperatorStatement left, IOperatorStatement right) {
            return left.And(right);
        }

        /// <summary>
        /// Implements the operator |.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static OperatorStatement operator |(OperatorStatement left, IOperatorStatement right) {
            return left.Or(right);
        }
    }
}
