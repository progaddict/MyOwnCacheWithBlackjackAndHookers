using System;
using System.Collections;
using System.Collections.Generic;



namespace Trainings.Cache
{
	/// <summary>
	/// LRU-cache with fixed capacity backed by dictionaries and linked list.
	/// Adding, modifying, removing and reading are O(1) operations
	/// assuming that these operations on <see cref="Dictionary{TKey,TValue}"/> are O(1).
	/// LRU = Last Recently Used means that if cache is full and new element needs to be added,
	/// least recently used element will be removed to free space for the new one.
	/// Element is used when it's modified, added or read.
	/// </summary>
	/// <seealso cref="Dictionary{TKey,TValue}"/>
	/// <seealso cref="LinkedList{T}"/>
	public sealed class MyOwnCacheWithBlackjackAndHookers<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly Int32 capacity;
		private readonly IDictionary<TKey, TValue> data;
		private readonly LinkedList<TKey> candidatesForDeletion;
		private readonly IDictionary<TKey, LinkedListNode<TKey>> keyNodeMapping;

		/// <summary>
		/// Create LU-cache with fixed capacity.
		/// </summary>
		/// <param name="capacity">Fixed capacity of the cache.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Capacity is less than or equals to zero.
		/// </exception>
		public MyOwnCacheWithBlackjackAndHookers(Int32 capacity)
		{
			if (capacity <= 0)
			{
				throw new ArgumentOutOfRangeException("capacity", capacity, "capacity can't be less than or equal to zero!");
			}
			this.capacity = capacity;
			data = new Dictionary<TKey, TValue>(capacity);
			candidatesForDeletion = new LinkedList<TKey>();
			keyNodeMapping = new Dictionary<TKey, LinkedListNode<TKey>>(capacity);
		}

		/// <summary>
		/// Maximal capacity of the cache.
		/// Do not confuse with <see cref="Count"/> which is current size.
		/// </summary>
		public Int32 Capacity
		{
			get { return capacity; }
		}

		/// <see cref="Dictionary{TKey,TValue}.GetEnumerator()"/>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		/// <see cref="Dictionary{TKey,TValue}.GetEnumerator()"/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <see cref="Add(TKey,TValue)"/>
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		/// <summary>
		/// Clear cache.
		/// </summary>
		/// <seealso cref="Dictionary{TKey,TValue}.Clear"/>
		/// <seealso cref="LinkedList{T}.Clear"/>
		public void Clear()
		{
			data.Clear();
			keyNodeMapping.Clear();
			candidatesForDeletion.Clear();
		}

		/// <see cref="Dictionary{TKey,TValue}.Contains(System.Collections.Generic.KeyValuePair{TKey,TValue})"/>
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return data.Contains(item);
		}

		/// <see cref="Dictionary{TKey,TValue}.CopyTo(System.Array, int)"/>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			data.CopyTo(array, arrayIndex);
		}

		/// <see cref="Remove(TKey)"/>
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			bool contains = Contains(item);
			if (contains)
			{
				Remove(item.Key);
			}
			return contains;
		}

		/// <summary>
		/// Current size of the cache.
		/// Can't be greater than <see cref="Capacity"/>.
		/// </summary>
		/// <seealso cref="Dictionary{TKey,TValue}.Count"/>
		public int Count
		{
			get { return data.Count; }
		}

		/// <see cref="Dictionary{TKey,TValue}.IsReadOnly"/>
		public bool IsReadOnly
		{
			get { return data.IsReadOnly; }
		}

		/// <see cref="Dictionary{TKey,TValue}.ContainsKey"/>
		public bool ContainsKey(TKey key)
		{
			return data.ContainsKey(key);
		}

		/// <see cref="Dictionary{TKey,TValue}.Add"/>
		/// <exception cref="ArgumentException">
		/// An element with the same key already exists in the cache.
		/// </exception>
		public void Add(TKey key, TValue value)
		{
			if (ContainsKey(key))
			{
				throw new ArgumentException("An element with the same key already exists in the cache.");
			}
			if (Count == Capacity)
			{
				RemoveLastUsed();
			}
			var node = candidatesForDeletion.AddLast(key);
			keyNodeMapping.Add(key, node);
			data.Add(key, value);
		}

		/// <see cref="Dictionary{TKey,TValue}.Remove"/>
		public bool Remove(TKey key)
		{
			if (ContainsKey(key))
			{
				RemoveKey(key);
				return true;
			}
			return false;
		}

		/// <see cref="Dictionary{TKey,TValue}.TryGetValue"/>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return data.TryGetValue(key, out value);
		}

		/// <see cref="Dictionary{TKey,TValue}.this"/>
		public TValue this[TKey key]
		{
			get
			{
				var value = data[key];
				UpdateKey(key);
				return value;
			}
			set
			{
				data[key] = value;
				UpdateKey(key);
			}
		}

		/// <summary>
		/// A copy of keys collection.
		/// Modification of the copy has no effect on the underlying cache data.
		/// </summary>
		/// <seealso cref="Dictionary{TKey,TValue}.Keys"/>
		public ICollection<TKey> Keys
		{
			get { return new List<TKey>(candidatesForDeletion); }
		}

		/// <summary>
		/// A copy of values collection.
		/// Modification of the copy has no effect on the underlying cache data.
		/// </summary>
		/// <seealso cref="Dictionary{TKey,TValue}.Values"/>
		public ICollection<TValue> Values
		{
			get { return new List<TValue>(data.Values); }
		}

		private void UpdateKey(TKey key)
		{
			var node = keyNodeMapping[key];
			candidatesForDeletion.Remove(node);
			candidatesForDeletion.AddLast(node);
		}

		private void RemoveLastUsed()
		{
			RemoveKey(candidatesForDeletion.First.Value);
		}

		private void RemoveKey(TKey key)
		{
			var node = keyNodeMapping[key];
			candidatesForDeletion.Remove(node);
			keyNodeMapping.Remove(key);
			data.Remove(key);
		}
	}
}
