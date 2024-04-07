using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace CloudyWing.MoneyTrack.Infrastructure.Application {
    /// <summary>
    /// A class for managing transfer messages and variables between pages for a specific user session.
    /// </summary>
    [Serializable]
    public class PageTransferContext {
        public const string AnyPage = "*";
        private readonly UserContext userContext;
        private readonly Dictionary<Type, VariableDictionary> variablesMaps = new Dictionary<Type, VariableDictionary>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PageTransferContext"/> class.
        /// </summary>
        /// <param name="userContext">The user context.</param>
        /// <exception cref="ArgumentNullException">userContext</exception>
        public PageTransferContext(UserContext userContext) {
            this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        /// <summary>
        /// Gets the variables for a current page.
        /// </summary>
        public VariableDictionary Variables {
            get {
                Type type = userContext.HttpContext.Handler.GetType().BaseType;
                variablesMaps.TryAdd(type, new VariableDictionary());

                return variablesMaps[type];
            }
        }

        /// <summary>
        /// Sets a transfer message for a specific page type.
        /// </summary>
        /// <param name="transferPage">The type of the page that the message is intended for.</param>
        /// <param name="message">The message to be transferred.</param>
        public void SetMessage(Type transferPage, string message) {
            MemoryCache.Default.Set($"{transferPage.Name}_{userContext.HttpContext.Session.SessionID}", message, DateTimeOffset.Now.AddMinutes(1));
        }

        /// <summary>
        /// Gets all transfer messages for the specified page types and removes them from cache.
        /// </summary>
        /// <param name="pageTypeNames">The names of the page types to get messages for.</param>
        /// <returns>An enumerable collection of transfer messages.</returns>
        public IEnumerable<string> GetMessages(params string[] pageTypeNames) {
            foreach (string typeName in pageTypeNames) {
                string cachedKey = $"{typeName}_{userContext.HttpContext.Session.SessionID}";

                if (MemoryCache.Default.Contains(cachedKey)) {
                    yield return MemoryCache.Default.Get(cachedKey).ToString();
                }
            }
        }

        /// <summary>
        /// Remove transfer messages for the specified page types and removes them from cache.
        /// </summary>
        /// <param name="pageTypeNames">The names of the page types to get messages for.</param>
        public void RemoveMessages(params string[] pageTypeNames) {
            foreach (string typeName in pageTypeNames) {
                string cachedKey = $"{typeName}_{userContext.HttpContext.Session.SessionID}";

                if (MemoryCache.Default.Contains(cachedKey)) {
                    MemoryCache.Default.Remove(cachedKey);
                }
            }
        }

        /// <summary>
        /// Sets a transfer message that is intended for any page.
        /// </summary>
        /// <param name="message">The message to be transferred.</param>
        public void SetMessageOfAnyPage(string message) {
            MemoryCache.Default.Set(GetBasicCachedKeyOfAnyPage(), message, DateTimeOffset.Now.AddMinutes(1));
        }

        private string GetBasicCachedKeyOfAnyPage() {
            return $"{AnyPage}_{userContext.HttpContext.Session.SessionID}";
        }

        /// <summary>
        /// Gets the transfer message that is intended for any page.
        /// </summary>
        /// <returns>The transfer message that is intended for any page.</returns>
        public string GetMessageOfAnyPage() {
            return HasMessageOfAnyPage()
                    ? MemoryCache.Default.Get(GetBasicCachedKeyOfAnyPage())?.ToString()
                    : "";
        }

        /// <summary>
        /// Determines whether there is a transfer message that is intended for any page.
        /// </summary>
        /// <returns>True if there is a transfer message that is intended for any page; otherwise, false.</returns>
        public bool HasMessageOfAnyPage() {
            return MemoryCache.Default.Contains(GetBasicCachedKeyOfAnyPage())
                    && !string.IsNullOrWhiteSpace(MemoryCache.Default.Get(GetBasicCachedKeyOfAnyPage())?.ToString());
        }

        /// <summary>
        /// Gets the variables.
        /// </summary>
        /// <param name="transferPage">The transfer page.</param>
        /// <returns>The transfer variables for the specified page types.</returns>
        public VariableDictionary GetVariables(Type transferPage) {
            if (!variablesMaps.TryGetValue(transferPage, out VariableDictionary variables)) {
                variables = new VariableDictionary();
                variablesMaps[transferPage] = variables;
            }

            return variables;
        }

        /// <summary>
        /// Sets a transfer variable for a specific page type.
        /// </summary>
        /// <param name="transferPage">The type of the page that the variable is intended for.</param>
        /// <param name="key">The key of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public void SetVariable(Type transferPage, string key, string value) {
            if (!variablesMaps.TryGetValue(transferPage, out VariableDictionary variables)) {
                variables = new VariableDictionary();
                variablesMaps[transferPage] = variables;
            }

            variables[key] = value;
        }
    }
}
