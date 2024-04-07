using System;
using System.Linq.Expressions;
using FluentValidation;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    public static class ExceptionUtils {
        public static void ThrowIfNull<T>(Expression<Func<T>> expression) where T : class {
            _ = expression.Compile().Invoke()
                ?? throw new ArgumentNullException(GetMemberName(expression));
        }

        public static void ThrowIfNull<T>(Expression<Func<T?>> expression) where T : struct {
            _ = expression.Compile().Invoke()
                ?? throw new ArgumentNullException(GetMemberName(expression));
        }

        public static void ThrowIfNullOrWhiteSpace(Expression<Func<string>> expression) {
            string value = expression.Compile().Invoke();
            if (string.IsNullOrWhiteSpace(value)) {
                throw new ArgumentException("不得為 Null 或空白字元。", GetMemberName(expression));
            }
        }

        public static void ThrowItemNotFound() {
            throw new ValidationException("資料項目不存在。");
        }

        public static void ThrowIfItemNotFound<T>(T obj) where T : class {
            _ = obj ?? throw new ValidationException("資料項目不存在。");
        }

        public static void ThrowIfInvalid<TValidator, TValidatable>(TValidator validator, Expression<Func<TValidatable>> expression)
            where TValidator : IValidator<TValidatable>
            where TValidatable : class {
            TValidatable instance = expression.Compile().Invoke() ?? throw new ArgumentNullException(GetMemberName(expression));
            validator.ValidateAndThrow(instance);
        }

        private static string GetMemberName<T>(Expression<Func<T>> expression) {
            if (!(expression.Body is MemberExpression expressionBody)) {
                throw new ArgumentException(null, nameof(expression));
            }
            return expressionBody.Member.Name;
        }
    }
}
