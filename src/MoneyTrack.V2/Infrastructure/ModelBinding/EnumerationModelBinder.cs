using System;
using System.Reflection;
using System.Web.Mvc;
using CloudyWing.Enumeration.Abstractions;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Infrastructure.ModelBinding {
    public class EnumerationModelBinder : IModelBinder {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            ExceptionUtils.ThrowIfNull(() => bindingContext);

            ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            Type enumerationBaseType = GetEnumerationBaseType(bindingContext.ModelType.BaseType);

            if (enumerationBaseType is null) {
                return null;
            }

            Type typeOfGenericTypeArgs = enumerationBaseType.GenericTypeArguments[1];
            object valueOfGenericTypeArgs = result.ConvertTo(enumerationBaseType.GenericTypeArguments[1]);

            Type[] argTypes = { typeOfGenericTypeArgs, bindingContext.ModelType.MakeByRefType() };
            var tryParseMethodInfo = bindingContext.ModelType.GetMethod("TryParse", argTypes);
            if (tryParseMethodInfo is null) {
                try {
                    return bindingContext.ModelType
                        .GetMethod("op_Explicit", BindingFlags.Public | BindingFlags.Static)
                        .Invoke(null, new object[] { valueOfGenericTypeArgs });
                } catch {
                    return null;
                }
            }

            object[] args = { valueOfGenericTypeArgs, null };

            return (bool)tryParseMethodInfo.Invoke(null, args)
                ? args[1] : null;
        }

        private Type GetEnumerationBaseType(Type type) {
            if (type is null) {
                return null;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EnumerationBase<,>)) {
                return type;
            }

            return GetEnumerationBaseType(type.BaseType);
        }
    }
}
