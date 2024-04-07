using System.Web.Mvc;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Infrastructure.ModelBinding {
    public class EncryptedRouteDataValueProviderFactory : ValueProviderFactory {
        /// <summary>
        /// Gets an instance of the <see cref="EncryptedRouteDataValueProvider"/> for the specified controller context.
        /// </summary>
        /// <param name="controllerContext">The controller context for which to create the value provider.</param>
        /// <returns>An instance of the <see cref="EncryptedRouteDataValueProvider"/>.</returns>
        public override IValueProvider GetValueProvider(ControllerContext controllerContext) {
            ExceptionUtils.ThrowIfNull(() => controllerContext);

            return new EncryptedRouteDataValueProvider(controllerContext);
        }
    }
}
