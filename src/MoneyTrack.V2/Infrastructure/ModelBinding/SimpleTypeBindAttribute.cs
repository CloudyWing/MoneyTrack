using System;
using System.Web.Mvc;

namespace CloudyWing.MoneyTrack.Infrastructure.ModelBinding {
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class SimpleTypeBindAttribute : CustomModelBinderAttribute {
        public string Prefix { get; set; }

        public override IModelBinder GetBinder() {
            return new SimpleTypeModelBinder(Prefix);
        }
    }
}
