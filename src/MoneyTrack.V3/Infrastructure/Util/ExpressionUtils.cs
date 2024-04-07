using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    public static class ExpressionUtils {
        public static IEnumerable<string> GetMember<TObject, TProperty>(Expression<Func<TObject, TProperty>> expression) {
            List<string> keys = [];
            if (expression is LambdaExpression lambda && lambda.Body is ConstantExpression constant) {
                string? key = constant.Value as string;
                if (key is not null) {
                    keys.Add(key);
                }
            } else {
                MemberExpression? memberExpression = GetMemberInternal(expression)
                    ?? throw new ArgumentException("Wrong expression.", nameof(expression));
                do {
                    keys.Add(memberExpression.Member.Name);
                    memberExpression = GetMemberInternal(memberExpression.Expression);
                } while (memberExpression != null);
            }

            keys.Reverse();
            return keys;
        }

        private static MemberExpression? GetMemberInternal([NotNull] Expression? expression) {
            ExceptionUtils.ThrowIfNull(() => expression);

            if (expression is MemberExpression member) {
                return member;
            } else if (expression is LambdaExpression lambda) {
                // 如果是 Value Type 的話 Body 會是 UnaryExpression
                // Reference Type 才會是直接取得到 MemberExpression
                return lambda.Body as MemberExpression
                    ?? (lambda.Body is UnaryExpression unary ? unary.Operand as MemberExpression : null);
            }

            return null;
        }
    }
}
