using System;
using System.Web;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Infrastructure.Application {
    /// <summary>
    /// Represents the context of the current user.
    /// </summary>
    [Serializable]
    public class UserContext {
        private UserContext(HttpContext httpContext) {
            HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            PageCache = new PageCacheContext(this);
            PageTransfer = new PageTransferContext(this);
            PageHistory = new PageHistoryContext(this);
            IPAddress = new IPAddressContext(this);
        }

        /// <summary>
        /// Gets the HTTP context.
        /// </summary>
        public HttpContext HttpContext { get; private set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets a value indicating whether the user is logged in.
        /// </summary>
        public bool IsLogined => !string.IsNullOrWhiteSpace(Id);

        /// <summary>
        /// Gets the <see cref="PageCacheContext"/> associated with the current user.
        /// </summary>
        public PageCacheContext PageCache { get; }

        /// <summary>
        /// Gets the <see cref="TransferContext"/> associated with the current user.
        /// </summary>
        public PageTransferContext PageTransfer { get; }

        /// <summary>
        /// Gets the <see cref="PageHistoryContext"/> associated with the current user.
        /// </summary>
        public PageHistoryContext PageHistory { get; }

        /// <summary>
        /// Gets the instance of the <see cref="UserContext"/> class associated with the current user.
        /// </summary>
        /// <param name="httpContext">The HTTP context of the user.</param>
        /// <returns>The instance of the <see cref="UserContext"/> class.</returns>
        public IPAddressContext IPAddress { get; }

        private void UpdateHttpContext(HttpContext httpContext) {
            HttpContext = httpContext;
        }

        /// <summary>
        /// Gets the instance of the <see cref="UserContext"/> class.
        /// </summary>
        public static UserContext GetInstance(HttpContext httpContext) {
            UserContext userContext;
            if (httpContext.Session["UserContext"] is null || !(httpContext.Session["UserContext"] is UserContext)) {
                userContext = new UserContext(httpContext);
                httpContext.Session["UserContext"] = userContext;
            } else {
                userContext = httpContext.Session["UserContext"] as UserContext;
                userContext.UpdateHttpContext(httpContext);
            }

            return userContext;
        }
    }
}
