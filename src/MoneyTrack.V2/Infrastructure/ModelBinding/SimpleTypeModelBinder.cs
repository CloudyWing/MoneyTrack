using System;
using System.Web.Mvc;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Infrastructure.ModelBinding {
    public class SimpleTypeModelBinder : IModelBinder {
        private readonly string prefix;

        public SimpleTypeModelBinder(string prefix) {
            this.prefix = prefix;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            ExceptionUtils.ThrowIfNull(() => bindingContext);

            object model = bindingContext.Model;
            Type modelType = bindingContext.ModelType;

            ModelBindingContext dictionaryBindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, modelType),
                ModelName = string.IsNullOrEmpty(prefix)
                    ? bindingContext.ModelName
                    : $"{prefix}.{bindingContext.ModelName}",
                ModelState = bindingContext.ModelState,
                PropertyFilter = bindingContext.PropertyFilter,
                ValueProvider = bindingContext.ValueProvider
            };

            DefaultModelBinder defaultModelBinder = new DefaultModelBinder();

            return defaultModelBinder.BindModel(controllerContext, dictionaryBindingContext);
        }
    }
}
