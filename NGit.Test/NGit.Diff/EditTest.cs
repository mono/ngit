using NGit.Diff;
using NUnit.Framework;
using Sharpen;

namespace NGit.Diff
{
	public class EditTest : TestCase
	{
		public virtual void TestCreate()
		{
			Edit e = new Edit(1, 2, 3, 4);
			NUnit.Framework.Assert.AreEqual(1, e.GetBeginA());
			NUnit.Framework.Assert.AreEqual(2, e.GetEndA());
			NUnit.Framework.Assert.AreEqual(3, e.GetBeginB());
			NUnit.Framework.Assert.AreEqual(4, e.GetEndB());
		}

		public virtual void TestCreateEmpty()
		{
			Edit e = new Edit(1, 3);
			NUnit.Framework.Assert.AreEqual(1, e.GetBeginA());
			NUnit.Framework.Assert.AreEqual(1, e.GetEndA());
			NUnit.Framework.Assert.AreEqual(3, e.GetBeginB());
			NUnit.Framework.Assert.AreEqual(3, e.GetEndB());
			NUnit.Framework.Assert.IsTrue("is empty", e.IsEmpty());
			NUnit.Framework.Assert.AreSame(Edit.Type.EMPTY, e.GetType());
		}

		public virtual void TestSwap()
		{
			Edit e = new Edit(1, 2, 3, 4);
			e.Swap();
			NUnit.Framework.Assert.AreEqual(3, e.GetBeginA());
			NUnit.Framework.Assert.AreEqual(4, e.GetEndA());
			NUnit.Framework.Assert.AreEqual(1, e.GetBeginB());
			NUnit.Framework.Assert.AreEqual(2, e.GetEndB());
		}

		public virtual void TestType_Insert()
		{
			Edit e = new Edit(1, 1, 1, 2);
			NUnit.Framework.Assert.AreSame(Edit.Type.INSERT, e.GetType());
			NUnit.Framework.Assert.IsFalse("not empty", e.IsEmpty());
			NUnit.Framework.Assert.AreEqual(0, e.GetLengthA());
			NUnit.Framework.Assert.AreEqual(1, e.GetLengthB());
		}

		public virtual void TestType_Delete()
		{
			Edit e = new Edit(1, 2, 1, 1);
			NUnit.Framework.Assert.AreSame(Edit.Type.DELETE, e.GetType());
			NUnit.Framework.Assert.IsFalse("not empty", e.IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, e.GetLengthA());
			NUnit.Framework.Assert.AreEqual(0, e.GetLengthB());
		}

		public virtual void TestType_Replace()
		{
			Edit e = new Edit(1, 2, 1, 4);
			NUnit.Framework.Assert.AreSame(Edit.Type.REPLACE, e.GetType());
			NUnit.Framework.Assert.IsFalse("not empty", e.IsEmpty());
			NUnit.Framework.Assert.AreEqual(1, e.GetLengthA());
			NUnit.Framework.Assert.AreEqual(3, e.GetLengthB());
		}

		public virtual void TestType_Empty()
		{
			Edit e = new Edit(1, 1, 2, 2);
			NUnit.Framework.Assert.AreSame(Edit.Type.EMPTY, e.GetType());
			NUnit.Framework.Assert.AreSame(Edit.Type.EMPTY, new Edit(1, 2).GetType());
			NUnit.Framework.Assert.IsTrue("is empty", e.IsEmpty());
			NUnit.Framework.Assert.AreEqual(0, e.GetLengthA());
			NUnit.Framework.Assert.AreEqual(0, e.GetLengthB());
		}

		public virtual void TestToString()
		{
			Edit e = new Edit(1, 2, 1, 4);
			NUnit.Framework.Assert.AreEqual("REPLACE(1-2,1-4)", e.ToString());
		}

		public virtual void TestEquals1()
		{
			Edit e1 = new Edit(1, 2, 3, 4);
			Edit e2 = new Edit(1, 2, 3, 4);
			NUnit.Framework.Assert.IsTrue(e1.Equals(e1));
			NUnit.Framework.Assert.IsTrue(e1.Equals(e2));
			NUnit.Framework.Assert.IsTrue(e2.Equals(e1));
			NUnit.Framework.Assert.AreEqual(e1.GetHashCode(), e2.GetHashCode());
			NUnit.Framework.Assert.IsFalse(e1.Equals(string.Empty));
		}

		public virtual void TestNotEquals1()
		{
			NUnit.Framework.Assert.IsFalse(new Edit(1, 2, 3, 4).Equals(new Edit(0, 2, 3, 4)));
		}

		public virtual void TestNotEquals2()
		{
			NUnit.Framework.Assert.IsFalse(new Edit(1, 2, 3, 4).Equals(new Edit(1, 0, 3, 4)));
		}

		public virtual void TestNotEquals3()
		{
			NUnit.Framework.Assert.IsFalse(new Edit(1, 2, 3, 4).Equals(new Edit(1, 2, 0, 4)));
		}

		public virtual void TestNotEquals4()
		{
			NUnit.Framework.Assert.IsFalse(new Edit(1, 2, 3, 4).Equals(new Edit(1, 2, 3, 0)));
		}

		public virtual void TestExtendA()
		{
			Edit e = new Edit(1, 2, 1, 1);
			e.ExtendA();
			NUnit.Framework.Assert.AreEqual(new Edit(1, 3, 1, 1), e);
			e.ExtendA();
			NUnit.Framework.Assert.AreEqual(new Edit(1, 4, 1, 1), e);
		}

		public virtual void TestExtendB()
		{
			Edit e = new Edit(1, 2, 1, 1);
			e.ExtendB();
			NUnit.Framework.Assert.AreEqual(new Edit(1, 2, 1, 2), e);
			e.ExtendB();
			NUnit.Framework.Assert.AreEqual(new Edit(1, 2, 1, 3), e);
		}

		public virtual void TestBeforeAfterCuts()
		{
			Edit whole = new Edit(1, 8, 2, 9);
			Edit mid = new Edit(4, 5, 3, 6);
			NUnit.Framework.Assert.AreEqual(new Edit(1, 4, 2, 3), whole.Before(mid));
			NUnit.Framework.Assert.AreEqual(new Edit(5, 8, 6, 9), whole.After(mid));
		}
	}
}
