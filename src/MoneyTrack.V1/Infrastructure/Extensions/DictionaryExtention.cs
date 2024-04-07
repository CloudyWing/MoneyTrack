namespace System.Collections.Generic {
    /// <summary>
    /// Provides extension methods for the <see cref="Dictionary{TKey, TValue}"/> class.
    /// </summary>
    public static class DictionaryExtention {
        /// <summary>
        /// Adds a new key-value pair to the dictionary if the key does not exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the dictionary keys.</typeparam>
        /// <typeparam name="TValue">The type of the dictionary values.</typeparam>
        /// <param name="dictionary">The dictionary object to add the key-value pair to.</param>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>true if the key-value pair is added successfully; otherwise, false.</returns>
        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) {
            if (!dictionary.ContainsKey(key)) {
                dictionary.Add(key, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a new key-value pair to the dictionary, or replaces the value of an existing key.
        /// </summary>
        /// <typeparam name="TKey">The type of the dictionary keys.</typeparam>
        /// <typeparam name="TValue">The type of the dictionary values.</typeparam>
        /// <param name="dictionary">The dictionary object to add the key-value pair to.</param>
        /// <param name="key">The key to add or replace.</param>
        /// <param name="value">The value to add or replace.</param>
        public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key] = value;
            } else {
                dictionary.Add(key, value);
            }
        }
    }
}
