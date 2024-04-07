using System;
using CloudyWing.FormValidators.Core;

namespace CloudyWing.FormValidators {
    public static class ValidationProviderExtensions {
        public static Func<string, string, BooleanValidator> Bool(this ValidationProvider provider) {
            return (column, value) => new BooleanValidator(column, value);
        }
    }
}
