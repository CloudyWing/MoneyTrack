using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CloudyWing.MoneyTrack.Infrastructure.Application {
    /// <summary>
    /// A dictionary that uses case-insensitive keys and supports adding, removing, and checking items.
    /// </summary>
    public class VariableDictionary : IDictionary<string, object>, ICollection<KeyValuePair<string, object>> {
        private readonly Dictionary<string, object> variableMaps = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a collection of the keys in the dictionary.
        /// </summary>
        public ICollection<string> Keys => variableMaps.Keys;

        /// <summary>
        /// Gets a collection of the values in the dictionary.
        /// </summary>
        public ICollection<object> Values => variableMaps.Values;

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key.</returns>
        public object this[string key] {
            get => variableMaps[key];
            set => variableMaps[key] = value;
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the dictionary.
        /// </summary>
        public int Count => variableMaps.Count;

        /// <summary>
        /// Gets a value indicating whether the dictionary is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an element with the specified key and value to the VariableDictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(string key, object value) {
            variableMaps.AddOrReplace(key, value);
        }

        /// <summary>
        /// Adds an item to the VariableDictionary.
        /// </summary>
        /// <param name="keyValuePair">The KeyValuePair structure representing the item to add.</param>
        public void Add(KeyValuePair<string, object> keyValuePair) {
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        /// <summary>
        /// Determines whether the VariableDictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the VariableDictionary.</param>
        /// <returns>true if the VariableDictionary contains an element with the specified key; otherwise, false.</returns>
        public bool Contains(string key) {
            return ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the VariableDictionary contains the specified KeyValuePair structure.
        /// </summary>
        /// <param name="keyValuePair">The KeyValuePair structure to locate in the VariableDictionary.</param>
        /// <returns>true if the KeyValuePair structure is found in the VariableDictionary; otherwise, false.</returns>
        public bool Contains(KeyValuePair<string, object> keyValuePair) {
            return variableMaps.Contains(keyValuePair);
        }

        /// <summary>
        /// Determines whether the VariableDictionary contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the VariableDictionary.</param>
        /// <returns>true if the VariableDictionary contains an element with the specified key; otherwise, false.</returns>
        public bool ContainsKey(string key) {
            return variableMaps.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the VariableDictionary contains an element with the specified value.
        /// </summary>
        /// <param name="value">The value to locate in the VariableDictionary.</param>
        /// <returns>true if the VariableDictionary contains an element with the specified value; otherwise, false.</returns>
        public bool ContainsValue(object value) {
            return variableMaps.ContainsValue(value);
        }

        public T GetValue<T>(string key) {
            if (!TryGetValue(key, out var value)) {
                return default;
            }

            return (T)value;
        }

        /// <summary>
        /// Removes the element with the specified key from the VariableDictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if key was not found in the original VariableDictionary.</returns>
        public bool Remove(string key) {
            return variableMaps.Remove(key);
        }

        /// <summary>
        /// Removes the first occurrence of a specific KeyValuePair structure from the VariableDictionary.
        /// </summary>
        /// <param name="keyValuePair">The KeyValuePair structure representing the item to remove.</param>
        public bool Remove(KeyValuePair<string, object> keyValuePair) {
            return ((ICollection<KeyValuePair<string, object>>)variableMaps).Remove(keyValuePair);
        }

        /// <summary>
        /// Tries to get the value associated with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns>true if the dictionary contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(string key, out object value) {
            return variableMaps.TryGetValue(key, out value);
        }

        public bool TryGetValue<T>(string key, out T value) {
            bool result = variableMaps.TryGetValue(key, out object valueObj);

            try {
                value = (T)valueObj;
            } catch {
                value = default;
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Removes all items from the dictionary.
        /// </summary>
        public void Clear() {
            variableMaps.Clear();
        }

        /// <summary>
        /// Copies the elements of the dictionary to an array, starting at a particular index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the dictionary.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<string, object>[] array, int index) {
            ((ICollection<KeyValuePair<string, object>>)variableMaps).CopyTo(array, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the dictionary.</returns>
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() {
            return variableMaps.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the dictionary.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return variableMaps.GetEnumerator();
        }
    }
}
