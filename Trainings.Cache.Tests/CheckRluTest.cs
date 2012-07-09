using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;



namespace Trainings.Cache.Tests
{
	[TestFixture(typeof(String), typeof(String),
		  new String[] {
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.REMOVE_KEY_COMMAND,
			},
			new String[] { "a", "b", "b" },
			new String[] { "a", "b", null },
			1,
			0,
			new String[0] )]
	[TestFixture(typeof(String), typeof(Double),
			new String[] {
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.REMOVE_KEY_COMMAND,
				CheckRluTest<TKey, TValue>.REMOVE_KEY_COMMAND,
				CheckRluTest<TKey, TValue>.REMOVE_KEY_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
			},
			new String[] { "pi",		"e",		"",		" ",	"l",	"x",	"x", "l", "", "fft",	"   "	},
			new Double[] { 3.14d,		2.71d,	0d,		-10d,	-4d,	3d,		0d,		0d,	0d,	-10d,		20d	},
			4,
			3,
			new String[] { " ", "fft", "   " } )]
	[TestFixture(typeof(String), typeof(String),
			new String[] {
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.MODIFY_COMMAND,
				CheckRluTest<TKey, TValue>.MODIFY_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
			},
			new String[] { "ak", "bk", "ck", "ak",		"bk",			"dk" },
			new String[] { "av", "bv", "cv", "newak",	"newbk",	"dv" },
			3,
			3,
			new String[] { "ak", "bk", "dk" })]
	[TestFixture(typeof(String), typeof(String),
			new String[] {
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.MODIFY_COMMAND,
				CheckRluTest<TKey, TValue>.MODIFY_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.MODIFY_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.ADD_COMMAND,
				CheckRluTest<TKey, TValue>.REMOVE_KEY_COMMAND,
				CheckRluTest<TKey, TValue>.REMOVE_KEY_COMMAND,
			},
			new String[] { "ak", "bk", "ck", "bk",		"ak",			"dk", "ek", "ak",			"fk", "gk", "fk", "gk" },
			new String[] { "av", "bv", "cv", "newbk", "newak",	"dv", "ev", "newak2",	"fv", "fv", null, null },
			3,
			1,
			new String[] { "ak" })]
	class CheckRluTest<TKey, TValue>
	{
		public const String ADD_COMMAND = "add";
		public const String REMOVE_KEY_COMMAND = "remove key";
		public const String MODIFY_COMMAND = "modify";

		private String[] commands;
		private TKey[] keys;
		private TValue[] values;
		private Int32 capacity;
		private Int32 expectedSize;
		private TKey[] keysAfter;

		public CheckRluTest(String[] commands, TKey[] keys, TValue[] values, Int32 capacity, Int32 expectedSize, TKey[] keysAfter)
		{
			this.commands = commands;
			this.keys = keys;
			this.values = values;
			this.capacity = capacity;
			this.expectedSize = expectedSize;
			this.keysAfter = keysAfter;
		}

		[Test]
		public void PerformCommandsAndTest()
		{
			var cache = new MyOwnCacheWithBlackjackAndHookers<TKey, TValue>(capacity);
			for (int i = 0; i < commands.Length; i++)
			{
				String command = commands[i];
				TKey key = keys[i];
				TValue value = values[i];
				switch (command)
				{
					case ADD_COMMAND:
						cache.Add(key, value);
						break;
					case REMOVE_KEY_COMMAND:
						cache.Remove(key);
						break;
					case MODIFY_COMMAND:
						cache[key] = value;
						break;
				}
			}
			Assert.That(cache.Capacity, Is.EqualTo(capacity));
			Assert.That(cache.Count, Is.EqualTo(expectedSize));
			foreach (var key in keysAfter)
			{
				Assert.That(cache.ContainsKey(key), Is.True);
			}
		}
	}
}
