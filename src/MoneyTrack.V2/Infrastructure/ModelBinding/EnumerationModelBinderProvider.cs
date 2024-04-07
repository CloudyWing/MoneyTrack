using System;
using System.Web.Mvc;
using CloudyWing.Enumeration.Abstractions;

namespace CloudyWing.MoneyTrack.Infrastructure.ModelBinding {
    public class EnumerationModelBinderProvider : IModelBinderProvider {
        public IModelBinder GetBinder(Type modelType) {
            Type type = modelType;
            while (type.BaseType != null) {

                if (modelType.BaseType.IsGenericType && modelType.BaseType.GetGenericTypeDefinition() == typeof(EnumerationBase<,>)) {
                    return new EnumerationModelBinder();
                }

                type = type.BaseType;
            }

            return null;
        }
    }
}
