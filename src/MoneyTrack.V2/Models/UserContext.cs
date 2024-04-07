using System;
using System.Web;

namespace CloudyWing.MoneyTrack.Models {
    /// <summary>
    /// Represents the context of a user session.
    /// </summary>
    [Serializable]
    public class UserContext {
        /// <summary>
        /// Gets or sets the user ID associated with the user context.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Loads the user context from the provided HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context containing relevant data.</param>
        /// <returns>True if the user context was successfully loaded; otherwise, false.</returns>
        public bool Load(HttpContextBase httpContext) {
            if (httpContext.Session[nameof(UserContext)] is UserContext userContext
                && userContext != null
            ) {
                UserId = userContext.UserId;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves the user context into the provided HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context where data will be stored.</param>
        /// <returns>True if the user context was successfully saved; otherwise, false.</returns>
        public bool Save(HttpContextBase httpContext) {
            if (httpContext.Session != null) {
                httpContext.Session[nameof(UserContext)] = this;
                return true;
            }
            return false;
        }
    }
}
