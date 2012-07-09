using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;



namespace Trainings.Cache.Tests
{
	[TestFixture(typeof(String), typeof(String),
			new String[] { "a", "", "c", "d", "e", "f", "g" },
			new String[] { "a", "b", "c", null, "e", "f", "g" })]
	[TestFixture(typeof(Int32), typeof(Int32),
			new Int32[] { 1, 2, 3, 4, 5, 6, 7 },
			new Int32[] { 8, 9, 10, 11, 12, 13, 14 })]
	[TestFixture(typeof(String), typeof(Int32),
			new String[] { "a", "", "c", "d", "e", "f", "g" },
			new Int32[] { 8, 9, 1, 11, 12, 13, 14 })]
	[TestFixture(typeof(Double), typeof(String),
			new Double[] { -5d, 0d, -1d, 1d, 3.14d, 6d, 8.81d, -2.78d, 3.33d },
			new String[] { "a", "b", "c", "d", "e", "f", "", null, null })]
	class ExctremeCasesTest<TKey, TValue>
	{
		private TKey[] sampleKeys;
		private TValue[] sampleValues;

		public ExctremeCasesTest(TKey[] sampleKeys, TValue[] sampleValues)
		{
			this.sampleKeys = sampleKeys;
			this.sampleValues = sampleValues;
		}

		[Test]
		public void OneCapacityTest()
		{
			var cache = new MyOwnCacheWithBlackjackAndHookers<TKey, TValue>(1);
			var numberOfElements = sampleKeys.Length;
			for (int i = 0; i < numberOfElements; i++)
			{
				cache.Add(sampleKeys[i], sampleValues[i]);
			}
			for (int i = 0; i < numberOfElements - 1; i++)
			{
				Assert.That(cache.ContainsKey(sampleKeys[i]), Is.False);
			}
			Assert.That(cache.ContainsKey(sampleKeys[numberOfElements - 1]), Is.True);
			Assert.That(cache.Capacity, Is.EqualTo(1));
			Assert.That(cache.Count, Is.EqualTo(1));
			cache.Remove(sampleKeys[numberOfElements - 1]);
			Assert.That(cache.Capacity, Is.EqualTo(1));
			Assert.That(cache.Count, Is.EqualTo(0));
			for (int i = 0; i < numberOfElements; i++)
			{
				Assert.That(cache.ContainsKey(sampleKeys[i]), Is.False);
			}
		}
	}
}
