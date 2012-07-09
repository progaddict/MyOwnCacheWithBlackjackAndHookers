using System;
using System.Collections.Generic;
using NUnit.Framework;



namespace Trainings.Cache.Tests
{
	[TestFixture(typeof(String), typeof(String),
			new String[] { "a", "b", "c", "d" },
			new String[] { "a", "b", "c", "d" },
			3)]
	[TestFixture(typeof(Int32), typeof(Int32),
			new Int32[] { 1, 2, 3, 4, 5, 6, 7 },
			new Int32[] { 8, 9, 10, 11, 12, 13, 14 },
			2)]
	[TestFixture(typeof(String), typeof(Int32),
			new String[] { "a", "b", "c", "d", "e", "f", "g" },
			new Int32[] { 8, 9, 10, 11, 12, 13, 14 },
			2)]
	[TestFixture(typeof(Double), typeof(String),
			new Double[] { -5d, 0d, -1d, 1d, 3.14d, 6d, 8.81d },
			new String[] { "a", "b", "c", "d", "e", "f", "" },
			3)]
	public class OverflowTest<TKey, TValue>
	{
		private TKey[] keys;
		private TValue[] values;
		private Int32 capacity;

		public OverflowTest(TKey[] keys, TValue[] values, Int32 capacity)
		{
			this.keys = keys;
			this.values = values;
			this.capacity = capacity;
		}

		[Test]
		public void AdditionTest()
		{
			var cache = new MyOwnCacheWithBlackjackAndHookers<TKey, TValue>(capacity);
			for (int i = 0; i < keys.Length; i++)
			{
				cache.Add(keys[i], values[i]);
			}
			Assert.That(cache.Capacity, Is.EqualTo(capacity));
			Assert.That(cache.Count, Is.EqualTo(capacity));
		}

		[Test]
		public void RemovalTest()
		{
			var cache = new MyOwnCacheWithBlackjackAndHookers<TKey, TValue>(capacity);
			for (int i = 0; i < keys.Length; i++)
			{
				cache.Add(keys[i], values[i]);
			}
			for (int i = 0; i < keys.Length; i++)
			{
				cache.Remove(keys[i]);
			}
			Assert.That(cache.Capacity, Is.EqualTo(capacity));
			Assert.That(cache.Count, Is.EqualTo(0));
		}

		[Test]
		public void AddRemoveTest()
		{
			var cache = new MyOwnCacheWithBlackjackAndHookers<TKey, TValue>(capacity);
			for (int i = 0; i < keys.Length; i++)
			{
				cache.Add(keys[i], values[i]);
				cache.Remove(keys[i]);
			}
			Assert.That(cache.Capacity, Is.EqualTo(capacity));
			Assert.That(cache.Count, Is.EqualTo(0));
		}
	}
}
