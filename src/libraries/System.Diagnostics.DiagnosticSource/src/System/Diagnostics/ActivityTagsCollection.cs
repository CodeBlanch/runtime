// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Collections;

namespace System.Diagnostics
{
    /// <summary>
    /// ActivityTagsCollection is a collection class used to store tracing tags.
    /// This collection will be used with classes like <see cref="ActivityEvent"/> and <see cref="ActivityLink"/>.
    /// This collection behaves as follows:
    ///     - The collection items will be ordered according to how they are added.
    ///     - Don't allow duplication of items with the same key.
    ///     - When using the indexer to store an item in the collection:
    ///         - If the item has a key that previously existed in the collection and the value is null, the collection item matching the key will be removed from the collection.
    ///         - If the item has a key that previously existed in the collection and the value is not null, the new item value will replace the old value stored in the collection.
    ///         - Otherwise, the item will be added to the collection.
    ///     - Add method will add a new item to the collection if an item doesn't already exist with the same key. Otherwise, it will throw an exception.
    /// </summary>
    public class ActivityTagsCollection : IDictionary<string, object?>
    {
        private DiagNode<KeyValuePair<string, object?>>? _first;
        private DiagNode<KeyValuePair<string, object?>>? _last;

        internal DiagNode<KeyValuePair<string, object?>>? First => _first;

        /// <summary>
        /// Create a new instance of the collection.
        /// </summary>
        public ActivityTagsCollection()
        {
        }

        /// <summary>
        /// Create a new instance of the collection and store the input list items in the collection.
        /// </summary>
        /// <param name="list">Initial list to store in the collection.</param>
        public ActivityTagsCollection(IEnumerable<KeyValuePair<string, object?>> list)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            foreach (KeyValuePair<string, object?> kvp in list)
            {
                if (kvp.Key != null)
                {
                    this[kvp.Key] = kvp.Value;
                }
            }
        }

        /// <summary>
        /// Get or set collection item
        /// When setting a value to this indexer property, the following behavior will be observed:
        ///     - If the key previously existed in the collection and the value is null, the collection item matching the key will get removed from the collection.
        ///     - If the key previously existed in the collection and the value is not null, the value will replace the old value stored in the collection.
        ///     - Otherwise, a new item will get added to the collection.
        /// </summary>
        /// <value>Object mapped to the key</value>
        public object? this[string key]
        {
            get => FindNode(key)?.Value.Value;

            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (value == null)
                {
                    Remove(key);
                    return;
                }

                DiagNode<KeyValuePair<string, object?>>? node = FindNode(key);
                if (node != null)
                {
                    node.Value = new KeyValuePair<string, object?>(key, value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        /// <summary>
        /// Get the list of the keys of all stored tags.
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                List<string> list = new List<string>(Count);
                foreach (KeyValuePair<string, object?> kvp in this)
                {
                    list.Add(kvp.Key);
                }
                return list;
            }
        }

        /// <summary>
        /// Get the list of the values of all stored tags.
        /// </summary>
        public ICollection<object?> Values
        {
            get
            {
                List<object?> list = new List<object?>(Count);
                foreach (KeyValuePair<string, object?> kvp in this)
                {
                    list.Add(kvp.Value);
                }
                return list;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Adds a tag with the provided key and value to the collection.
        /// This collection doesn't allow adding two tags with the same key.
        /// </summary>
        /// <param name="key">The tag key.</param>
        /// <param name="value">The tag value.</param>
        public void Add(string key, object? value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            DiagNode<KeyValuePair<string, object?>>? node = FindNode(key);
            if (node != null)
            {
                throw new InvalidOperationException(SR.Format(SR.KeyAlreadyExist, key));
            }

            node = new DiagNode<KeyValuePair<string, object?>>(new KeyValuePair<string, object?>(key, value));

            if (_first == null)
            {
                _first = _last = node;
            }
            else
            {
                Debug.Assert(_last != null);

                _last!.Next = node;
                _last = node;
            }

            Count++;
        }

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="item">Key and value pair of the tag to add to the collection.</param>
        public void Add(KeyValuePair<string, object?> item)
            => Add(item.Key, item.Value);

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            _first = _last = null;
            Count = 0;
        }

        public bool Contains(KeyValuePair<string, object?> item)
        {
            DiagNode<KeyValuePair<string, object?>>? node = FindNode(item.Key);
            return node != null && Equals(node.Value.Value, item.Value);
        }

        /// <summary>
        /// Determines whether the collection contains an element with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if the collection contains tag with that key. False otherwise.</returns>
        public bool ContainsKey(string key) => FindNode(key) != null;

        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The array that is the destination of the elements copied from collection.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            if (Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("TODO");
            }

            foreach (KeyValuePair<string, object?> item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => new Enumerator(_first);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public Enumerator GetEnumerator() => new Enumerator(_first);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(_first);

        /// <summary>
        /// Removes the tag with the specified key from the collection.
        /// </summary>
        /// <param name="key">The tag key</param>
        /// <returns>True if the item existed and removed. False otherwise.</returns>
        public bool Remove(string key)
            => Remove(key, null, matchValue: false);

        /// <summary>
        /// Removes the first occurrence of a specific item from the collection.
        /// </summary>
        /// <param name="item">The tag key value pair to remove.</param>
        /// <returns>True if item was successfully removed from the collection; otherwise, false. This method also returns false if item is not found in the original collection.</returns>
        public bool Remove(KeyValuePair<string, object?> item)
            => Remove(item.Key, item.Value, matchValue: true);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The tag key.</param>
        /// <param name="value">The tag value.</param>
        /// <returns>When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</returns>
        public bool TryGetValue(string key, out object? value)
        {
            DiagNode<KeyValuePair<string, object?>>? node = FindNode(key);
            if (node != null)
            {
                value = node.Value.Value;
                return true;
            }

            value = null;
            return false;
        }

        private DiagNode<KeyValuePair<string, object?>>? FindNode(string key)
        {
            DiagNode<KeyValuePair<string, object?>>? node = _first;
            while (node != null)
            {
                if (node.Value.Key == key)
                {
                    return node;
                }

                node = node.Next;
            }
            return null;
        }

        private bool Remove(string key, object? value, bool matchValue)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            DiagNode<KeyValuePair<string, object?>>? previousNode = null;
            DiagNode<KeyValuePair<string, object?>>? currentNode = _first;

            while (currentNode != null)
            {
                if (currentNode.Value.Key == key
                    && (!matchValue || Equals(currentNode.Value.Value, value)))
                {
                    if (previousNode == null)
                    {
                        _first = currentNode.Next;
                    }
                    else
                    {
                        previousNode.Next = currentNode.Next;
                    }

                    if (currentNode == _last)
                    {
                        _last = previousNode;
                    }

                    Count--;

                    return true;
                }

                previousNode = currentNode;
                currentNode = currentNode.Next;
            }

            return false;
        }

        public struct Enumerator : IEnumerator<KeyValuePair<string, object?>>, IEnumerator
        {
            private DiagEnumerator<KeyValuePair<string, object?>> _enumerator;
            internal Enumerator(DiagNode<KeyValuePair<string, object?>>? first) => _enumerator = new(first);

            public KeyValuePair<string, object?> Current => _enumerator.Current;
            object IEnumerator.Current => ((IEnumerator)_enumerator).Current;
            public void Dispose() => _enumerator.Dispose();
            public bool MoveNext() => _enumerator.MoveNext();
            void IEnumerator.Reset() => ((IEnumerator)_enumerator).Reset();
        }
    }
}
