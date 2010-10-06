using NGit.Diff;
using NUnit.Framework;
using Sharpen;

namespace NGit.Diff
{
	public class EditListTest : TestCase
	{
		public virtual void TestEmpty()
		{
			EditList l = new EditList();
			NUnit.Framework.Assert.AreEqual(0, l.Count);
			NUnit.Framework.Assert.IsTrue(l.IsEmpty());
			NUnit.Framework.Assert.AreEqual("EditList[]", l.ToString());
			NUnit.Framework.Assert.IsTrue(l.Equals(l));
			NUnit.Framework.Assert.IsTrue(l.Equals(new EditList()));
			NUnit.Framework.Assert.IsFalse(l.Equals(string.Empty));
			NUnit.Framework.Assert.AreEqual(l.GetHashCode(), new EditList().GetHashCode());
		}

		public virtual void TestAddOne()
		{
			Edit e = new Edit(1, 2, 1, 1);
			EditList l = new EditList();
			l.AddItem(e);
			NUnit.Framework.Assert.AreEqual(1, l.Count);
			NUnit.Framework.Assert.IsFalse(l.IsEmpty());
			NUnit.Framework.Assert.AreSame(e, l[0]);
			NUnit.Framework.Assert.AreSame(e, l.Iterator().Next());
			NUnit.Framework.Assert.IsTrue(l.Equals(l));
			NUnit.Framework.Assert.IsFalse(l.Equals(new EditList()));
			EditList l2 = new EditList();
			l2.AddItem(e);
			NUnit.Framework.Assert.IsTrue(l.Equals(l2));
			NUnit.Framework.Assert.IsTrue(l2.Equals(l));
			NUnit.Framework.Assert.AreEqual(l.GetHashCode(), l2.GetHashCode());
		}

		public virtual void TestAddTwo()
		{
			Edit e1 = new Edit(1, 2, 1, 1);
			Edit e2 = new Edit(8, 8, 8, 12);
			EditList l = new EditList();
			l.AddItem(e1);
			l.AddItem(e2);
			NUnit.Framework.Assert.AreEqual(2, l.Count);
			NUnit.Framework.Assert.AreSame(e1, l[0]);
			NUnit.Framework.Assert.AreSame(e2, l[1]);
			Iterator<Edit> i = l.Iterator();
			NUnit.Framework.Assert.AreSame(e1, i.Next());
			NUnit.Framework.Assert.AreSame(e2, i.Next());
			NUnit.Framework.Assert.IsTrue(l.Equals(l));
			NUnit.Framework.Assert.IsFalse(l.Equals(new EditList()));
			EditList l2 = new EditList();
			l2.AddItem(e1);
			l2.AddItem(e2);
			NUnit.Framework.Assert.IsTrue(l.Equals(l2));
			NUnit.Framework.Assert.IsTrue(l2.Equals(l));
			NUnit.Framework.Assert.AreEqual(l.GetHashCode(), l2.GetHashCode());
		}

		public virtual void TestSet()
		{
			Edit e1 = new Edit(1, 2, 1, 1);
			Edit e2 = new Edit(3, 4, 3, 3);
			EditList l = new EditList();
			l.AddItem(e1);
			NUnit.Framework.Assert.AreSame(e1, l[0]);
			NUnit.Framework.Assert.AreSame(e1, l.Set(0, e2));
			NUnit.Framework.Assert.AreSame(e2, l[0]);
		}

		public virtual void TestRemove()
		{
			Edit e1 = new Edit(1, 2, 1, 1);
			Edit e2 = new Edit(8, 8, 8, 12);
			EditList l = new EditList();
			l.AddItem(e1);
			l.AddItem(e2);
			l.Remove(e1);
			NUnit.Framework.Assert.AreEqual(1, l.Count);
			NUnit.Framework.Assert.AreSame(e2, l[0]);
		}
	}
}
