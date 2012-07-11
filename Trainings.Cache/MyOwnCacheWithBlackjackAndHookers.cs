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
		#region Fields

		private readonly Int32 _capacity;
		private readonly IDictionary<TKey, TValue> _data;
		private readonly LinkedList<TKey> _candidatesForDeletion;
		private readonly IDictionary<TKey, LinkedListNode<TKey>> _keyNodeMapping;

		#endregion

		#region Constructors

		public MyOwnCacheWithBlackjackAndHookers(Int32 fixedCacheCapacity)
		{
			if (fixedCacheCapacity <= 0)
			{
				throw new ArgumentOutOfRangeException("capacity", fixedCacheCapacity, "capacity can't be less than or equal to zero!");
			}
			_capacity = fixedCacheCapacity;
			_data = new Dictionary<TKey, TValue>(fixedCacheCapacity);
			_candidatesForDeletion = new LinkedList<TKey>();
			_keyNodeMapping = new Dictionary<TKey, LinkedListNode<TKey>>(fixedCacheCapacity);
		}

		#endregion

		#region Properties

		public bool IsReadOnly
		{
			get { return _data.IsReadOnly; }
		}

		/// <summary>
		/// Maximal and fixed size of the cache.
		/// Do not confuse with <see cref="Count"/> which is current size.
		/// </summary>
		public Int32 Capacity
		{
			get { return _capacity; }
		}

		/// <summary>
		/// Current number of cached key-value pairs in the cache.
		/// Can't be greater than <see cref="Capacity"/> of the cache.
		/// </summary>
		/// <seealso cref="Dictionary{TKey,TValue}.Count"/>
		public int Count
		{
			get { return _data.Count; }
		}

		/// <summary>
		/// A copy of keys collection.
		/// Modification of the copy has no effect on the underlying cache data.
		/// </summary>
		public ICollection<TKey> Keys
		{
			get { return new List<TKey>(_candidatesForDeletion); }
		}

		/// <summary>
		/// A copy of values collection.
		/// Modification of the copy has no effect on the underlying cache data.
		/// </summary>
		public ICollection<TValue> Values
		{
			get { return new List<TValue>(_data.Values); }
		}

		public TValue this[TKey key]
		{
			get
			{
				var value = _data[key];
				UpdateKey(key);
				return value;
			}
			set
			{
				_data[key] = value;
				UpdateKey(key);
			}
		}

		#endregion

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			_data.Clear();
			_keyNodeMapping.Clear();
			_candidatesForDeletion.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return _data.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			_data.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			bool contains = Contains(item);
			if (contains)
			{
				Remove(item.Key);
			}
			return contains;
		}

		public bool ContainsKey(TKey key)
		{
			return _data.ContainsKey(key);
		}

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
			var node = _candidatesForDeletion.AddLast(key);
			_keyNodeMapping.Add(key, node);
			_data.Add(key, value);
		}

		public bool Remove(TKey key)
		{
			bool contains = ContainsKey(key);
			if (contains)
			{
				RemoveKey(key);
			}
			return contains;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			bool success = _data.TryGetValue(key, out value);
			if (success)
			{
				UpdateKey(key);
			}
			return success;
		}

		#region Override

		public override bool Equals(object obj)
		{
			var cache = obj as MyOwnCacheWithBlackjackAndHookers<TKey, TValue>;
			return Equals(cache);
		}

		public bool Equals(MyOwnCacheWithBlackjackAndHookers<TKey, TValue> cache)
		{
			if (ReferenceEquals(cache, null))
			{
				return false;
			}
			if (ReferenceEquals(cache, this))
			{
				return true;
			}
			return _capacity.Equals(cache._capacity)
				&& _data.Equals(cache._data)
				&& _candidatesForDeletion.Equals(cache._candidatesForDeletion)
				&& _keyNodeMapping.Equals(cache._keyNodeMapping);
		}

		public override int GetHashCode()
		{
			Int32 hashCode = _capacity.GetHashCode();
			unchecked
			{
				hashCode = 37 * hashCode + _data.GetHashCode();
				hashCode = 37 * hashCode + _candidatesForDeletion.GetHashCode();
				hashCode = 37 * hashCode + _keyNodeMapping.GetHashCode();
			}
			return hashCode;
		}

		#endregion

		private void UpdateKey(TKey key)
		{
			var node = _keyNodeMapping[key];
			_candidatesForDeletion.Remove(node);
			_candidatesForDeletion.AddLast(node);
		}

		private void RemoveLastUsed()
		{
			RemoveKey(_candidatesForDeletion.First.Value);
		}

		private void RemoveKey(TKey key)
		{
			var node = _keyNodeMapping[key];
			_candidatesForDeletion.Remove(node);
			_keyNodeMapping.Remove(key);
			_data.Remove(key);
		}
	}
}
