using System;
using System.Web.Mvc;

namespace CloudyWing.MoneyTrack.Infrastructure.ModelBinding {
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromServicesAttribute : CustomModelBinderAttribute {
        public override IModelBinder GetBinder() {
            return new ServicesModelBinder();
        }
    }
}
