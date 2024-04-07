using System.Web.Mvc;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Infrastructure.ModelBinding {
    public class ServicesModelBinder : IModelBinder {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            ExceptionUtils.ThrowIfNull(() => bindingContext);

            return DependencyResolver.Current.GetService(bindingContext.ModelType);
        }
    }
}
