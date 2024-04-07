using System;
using System.Collections.Generic;

namespace CloudyWing.MoneyTrack.Infrastructure.Application {
    /// <summary>
    /// Provides a context for storing and retrieving page-level variables during a web page request.
    /// </summary>
    [Serializable]
    public class PageCacheContext {
        private readonly UserContext userContext;
        private readonly Dictionary<Type, VariableDictionary> variablesMaps = new Dictionary<Type, VariableDictionary>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PageCacheContext"/> class.
        /// </summary>
        /// <param name="userContext">The user context.</param>
        /// <exception cref="ArgumentNullException">userContext</exception>
        public PageCacheContext(UserContext userContext) {
            this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        /// <summary>
        /// Gets the variable dictionary associated with the current web page request.
        /// </summary>
        public VariableDictionary Variables {
            get {
                Type type = userContext.HttpContext.Handler.GetType().BaseType;
                variablesMaps.TryAdd(type, new VariableDictionary());

                return variablesMaps[type];
            }
        }

        /// <summary>
        /// Removes the variable dictionary associated with the current web page request.
        /// </summary>
        public void RemovePageVariables() {
            Type type = userContext.HttpContext.Handler.GetType().BaseType;

            if (variablesMaps.ContainsKey(type)) {
                variablesMaps.Remove(type);
            }
        }
    }
}
