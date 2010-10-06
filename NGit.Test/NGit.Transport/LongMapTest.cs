using NGit.Transport;
using NUnit.Framework;
using Sharpen;

namespace NGit.Transport
{
	public class LongMapTest : TestCase
	{
		private LongMap<long> map;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			map = new LongMap<long>();
		}

		public virtual void TestEmptyMap()
		{
			NUnit.Framework.Assert.IsFalse(map.ContainsKey(0));
			NUnit.Framework.Assert.IsFalse(map.ContainsKey(1));
			NUnit.Framework.Assert.IsNull(map.Get(0));
			NUnit.Framework.Assert.IsNull(map.Get(1));
			NUnit.Framework.Assert.IsNull(map.Remove(0));
			NUnit.Framework.Assert.IsNull(map.Remove(1));
		}

		public virtual void TestInsertMinValue()
		{
			long min = Sharpen.Extensions.ValueOf(long.MinValue);
			NUnit.Framework.Assert.IsNull(map.Put(long.MinValue, min));
			NUnit.Framework.Assert.IsTrue(map.ContainsKey(long.MinValue));
			NUnit.Framework.Assert.AreSame(min, map.Get(long.MinValue));
			NUnit.Framework.Assert.IsFalse(map.ContainsKey(int.MinValue));
		}

		public virtual void TestReplaceMaxValue()
		{
			long min = Sharpen.Extensions.ValueOf(long.MaxValue);
			long one = Sharpen.Extensions.ValueOf(1);
			NUnit.Framework.Assert.IsNull(map.Put(long.MaxValue, min));
			NUnit.Framework.Assert.AreSame(min, map.Get(long.MaxValue));
			NUnit.Framework.Assert.AreSame(min, map.Put(long.MaxValue, one));
			NUnit.Framework.Assert.AreSame(one, map.Get(long.MaxValue));
		}

		public virtual void TestRemoveOne()
		{
			long start = 1;
			NUnit.Framework.Assert.IsNull(map.Put(start, Sharpen.Extensions.ValueOf(start)));
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(start), map.Remove(start
				));
			NUnit.Framework.Assert.IsFalse(map.ContainsKey(start));
		}

		public virtual void TestRemoveCollision1()
		{
			// This test relies upon the fact that we always >>> 1 the value
			// to derive an unsigned hash code. Thus, 0 and 1 fall into the
			// same hash bucket. Further it relies on the fact that we add
			// the 2nd put at the top of the chain, so removing the 1st will
			// cause a different code path.
			//
			NUnit.Framework.Assert.IsNull(map.Put(0, Sharpen.Extensions.ValueOf(0)));
			NUnit.Framework.Assert.IsNull(map.Put(1, Sharpen.Extensions.ValueOf(1)));
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(0), map.Remove(0));
			NUnit.Framework.Assert.IsFalse(map.ContainsKey(0));
			NUnit.Framework.Assert.IsTrue(map.ContainsKey(1));
		}

		public virtual void TestRemoveCollision2()
		{
			// This test relies upon the fact that we always >>> 1 the value
			// to derive an unsigned hash code. Thus, 0 and 1 fall into the
			// same hash bucket. Further it relies on the fact that we add
			// the 2nd put at the top of the chain, so removing the 2nd will
			// cause a different code path.
			//
			NUnit.Framework.Assert.IsNull(map.Put(0, Sharpen.Extensions.ValueOf(0)));
			NUnit.Framework.Assert.IsNull(map.Put(1, Sharpen.Extensions.ValueOf(1)));
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(1), map.Remove(1));
			NUnit.Framework.Assert.IsTrue(map.ContainsKey(0));
			NUnit.Framework.Assert.IsFalse(map.ContainsKey(1));
		}

		public virtual void TestSmallMap()
		{
			long start = 12;
			long n = 8;
			for (long i = start; i < start + n; i++)
			{
				NUnit.Framework.Assert.IsNull(map.Put(i, Sharpen.Extensions.ValueOf(i)));
			}
			for (long i_1 = start; i_1 < start + n; i_1++)
			{
				NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(i_1), map.Get(i_1));
			}
		}

		public virtual void TestLargeMap()
		{
			long start = int.MaxValue;
			long n = 100000;
			for (long i = start; i < start + n; i++)
			{
				NUnit.Framework.Assert.IsNull(map.Put(i, Sharpen.Extensions.ValueOf(i)));
			}
			for (long i_1 = start; i_1 < start + n; i_1++)
			{
				NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(i_1), map.Get(i_1));
			}
		}
	}
}
