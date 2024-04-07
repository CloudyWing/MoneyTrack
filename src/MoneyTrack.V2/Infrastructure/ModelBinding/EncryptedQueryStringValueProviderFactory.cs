using System;
using System.Web.Mvc;

namespace CloudyWing.MoneyTrack.Infrastructure.ModelBinding {
    /// <summary>
    /// Represents a factory for creating instances of the <see cref="EncryptedQueryStringValueProvider"/>.
    /// </summary>
    public class EncryptedQueryStringValueProviderFactory : ValueProviderFactory {
        /// <summary>
        /// Gets an instance of the <see cref="EncryptedQueryStringValueProvider"/> for the specified controller context.
        /// </summary>
        /// <param name="controllerContext">The controller context for which to create the value provider.</param>
        /// <returns>An instance of the <see cref="EncryptedQueryStringValueProvider"/>.</returns>
        public override IValueProvider GetValueProvider(ControllerContext controllerContext) {
            if (controllerContext is null) {
                throw new ArgumentNullException(nameof(controllerContext));
            }

            return new EncryptedQueryStringValueProvider(controllerContext);
        }
    }
}
