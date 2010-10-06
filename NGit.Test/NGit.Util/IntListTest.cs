using System;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class IntListTest : TestCase
	{
		public virtual void TestEmpty_DefaultCapacity()
		{
			IntList i = new IntList();
			NUnit.Framework.Assert.AreEqual(0, i.Size());
			try
			{
				i.Get(0);
				NUnit.Framework.Assert.Fail("Accepted 0 index on empty list");
			}
			catch (IndexOutOfRangeException)
			{
				NUnit.Framework.Assert.IsTrue(true);
			}
		}

		public virtual void TestEmpty_SpecificCapacity()
		{
			IntList i = new IntList(5);
			NUnit.Framework.Assert.AreEqual(0, i.Size());
			try
			{
				i.Get(0);
				NUnit.Framework.Assert.Fail("Accepted 0 index on empty list");
			}
			catch (IndexOutOfRangeException)
			{
				NUnit.Framework.Assert.IsTrue(true);
			}
		}

		public virtual void TestAdd_SmallGroup()
		{
			IntList i = new IntList();
			int n = 5;
			for (int v = 0; v < n; v++)
			{
				i.Add(10 + v);
			}
			NUnit.Framework.Assert.AreEqual(n, i.Size());
			for (int v_1 = 0; v_1 < n; v_1++)
			{
				NUnit.Framework.Assert.AreEqual(10 + v_1, i.Get(v_1));
			}
			try
			{
				i.Get(n);
				NUnit.Framework.Assert.Fail("Accepted out of bound index on list");
			}
			catch (IndexOutOfRangeException)
			{
				NUnit.Framework.Assert.IsTrue(true);
			}
		}

		public virtual void TestAdd_ZeroCapacity()
		{
			IntList i = new IntList(0);
			NUnit.Framework.Assert.AreEqual(0, i.Size());
			i.Add(1);
			NUnit.Framework.Assert.AreEqual(1, i.Get(0));
		}

		public virtual void TestAdd_LargeGroup()
		{
			IntList i = new IntList();
			int n = 500;
			for (int v = 0; v < n; v++)
			{
				i.Add(10 + v);
			}
			NUnit.Framework.Assert.AreEqual(n, i.Size());
			for (int v_1 = 0; v_1 < n; v_1++)
			{
				NUnit.Framework.Assert.AreEqual(10 + v_1, i.Get(v_1));
			}
			try
			{
				i.Get(n);
				NUnit.Framework.Assert.Fail("Accepted out of bound index on list");
			}
			catch (IndexOutOfRangeException)
			{
				NUnit.Framework.Assert.IsTrue(true);
			}
		}

		public virtual void TestFillTo0()
		{
			IntList i = new IntList();
			i.FillTo(0, int.MinValue);
			NUnit.Framework.Assert.AreEqual(0, i.Size());
		}

		public virtual void TestFillTo1()
		{
			IntList i = new IntList();
			i.FillTo(1, int.MinValue);
			NUnit.Framework.Assert.AreEqual(1, i.Size());
			i.Add(0);
			NUnit.Framework.Assert.AreEqual(int.MinValue, i.Get(0));
			NUnit.Framework.Assert.AreEqual(0, i.Get(1));
		}

		public virtual void TestFillTo100()
		{
			IntList i = new IntList();
			i.FillTo(100, int.MinValue);
			NUnit.Framework.Assert.AreEqual(100, i.Size());
			i.Add(3);
			NUnit.Framework.Assert.AreEqual(int.MinValue, i.Get(99));
			NUnit.Framework.Assert.AreEqual(3, i.Get(100));
		}

		public virtual void TestClear()
		{
			IntList i = new IntList();
			int n = 5;
			for (int v = 0; v < n; v++)
			{
				i.Add(10 + v);
			}
			NUnit.Framework.Assert.AreEqual(n, i.Size());
			i.Clear();
			NUnit.Framework.Assert.AreEqual(0, i.Size());
			try
			{
				i.Get(0);
				NUnit.Framework.Assert.Fail("Accepted 0 index on empty list");
			}
			catch (IndexOutOfRangeException)
			{
				NUnit.Framework.Assert.IsTrue(true);
			}
		}

		public virtual void TestSet()
		{
			IntList i = new IntList();
			i.Add(1);
			NUnit.Framework.Assert.AreEqual(1, i.Size());
			NUnit.Framework.Assert.AreEqual(1, i.Get(0));
			i.Set(0, 5);
			NUnit.Framework.Assert.AreEqual(5, i.Get(0));
			try
			{
				i.Set(5, 5);
				NUnit.Framework.Assert.Fail("accepted set of 5 beyond end of list");
			}
			catch (IndexOutOfRangeException)
			{
				NUnit.Framework.Assert.IsTrue(true);
			}
			i.Set(1, 2);
			NUnit.Framework.Assert.AreEqual(2, i.Size());
			NUnit.Framework.Assert.AreEqual(2, i.Get(1));
		}

		public virtual void TestToString()
		{
			IntList i = new IntList();
			i.Add(1);
			NUnit.Framework.Assert.AreEqual("[1]", i.ToString());
			i.Add(13);
			i.Add(5);
			NUnit.Framework.Assert.AreEqual("[1, 13, 5]", i.ToString());
		}
	}
}
