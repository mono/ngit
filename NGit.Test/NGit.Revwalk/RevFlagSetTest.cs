using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public class RevFlagSetTest : RevWalkTestCase
	{
		public virtual void TestEmpty()
		{
			RevFlagSet set = new RevFlagSet();
			NUnit.Framework.Assert.AreEqual(0, set.mask);
			NUnit.Framework.Assert.AreEqual(0, set.Count);
			NUnit.Framework.Assert.IsNotNull(set.Iterator());
			NUnit.Framework.Assert.IsFalse(set.Iterator().HasNext());
		}

		public virtual void TestAddOne()
		{
			string flagName = "flag";
			RevFlag flag = rw.NewFlag(flagName);
			NUnit.Framework.Assert.IsTrue(0 != flag.mask);
			NUnit.Framework.Assert.AreSame(flagName, flag.name);
			RevFlagSet set = new RevFlagSet();
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag));
			NUnit.Framework.Assert.IsFalse(set.AddItem(flag));
			NUnit.Framework.Assert.AreEqual(flag.mask, set.mask);
			NUnit.Framework.Assert.AreEqual(1, set.Count);
			Iterator<RevFlag> i = set.Iterator();
			NUnit.Framework.Assert.IsTrue(i.HasNext());
			NUnit.Framework.Assert.AreSame(flag, i.Next());
			NUnit.Framework.Assert.IsFalse(i.HasNext());
		}

		public virtual void TestAddTwo()
		{
			RevFlag flag1 = rw.NewFlag("flag_1");
			RevFlag flag2 = rw.NewFlag("flag_2");
			NUnit.Framework.Assert.IsTrue((flag1.mask & flag2.mask) == 0);
			RevFlagSet set = new RevFlagSet();
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag1));
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag2));
			NUnit.Framework.Assert.AreEqual(flag1.mask | flag2.mask, set.mask);
			NUnit.Framework.Assert.AreEqual(2, set.Count);
		}

		public virtual void TestContainsAll()
		{
			RevFlag flag1 = rw.NewFlag("flag_1");
			RevFlag flag2 = rw.NewFlag("flag_2");
			RevFlagSet set1 = new RevFlagSet();
			NUnit.Framework.Assert.IsTrue(set1.AddItem(flag1));
			NUnit.Framework.Assert.IsTrue(set1.AddItem(flag2));
			NUnit.Framework.Assert.IsTrue(set1.ContainsAll(set1));
			NUnit.Framework.Assert.IsTrue(set1.ContainsAll(Arrays.AsList(new RevFlag[] { flag1
				, flag2 })));
			RevFlagSet set2 = new RevFlagSet();
			set2.AddItem(rw.NewFlag("flag_3"));
			NUnit.Framework.Assert.IsFalse(set1.ContainsAll(set2));
		}

		public virtual void TestEquals()
		{
			RevFlag flag1 = rw.NewFlag("flag_1");
			RevFlag flag2 = rw.NewFlag("flag_2");
			RevFlagSet set = new RevFlagSet();
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag1));
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag2));
			NUnit.Framework.Assert.IsTrue(new RevFlagSet(set).Equals(set));
			NUnit.Framework.Assert.IsTrue(new RevFlagSet(Arrays.AsList(new RevFlag[] { flag1, 
				flag2 })).Equals(set));
		}

		public virtual void TestRemove()
		{
			RevFlag flag1 = rw.NewFlag("flag_1");
			RevFlag flag2 = rw.NewFlag("flag_2");
			RevFlagSet set = new RevFlagSet();
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag1));
			NUnit.Framework.Assert.IsTrue(set.AddItem(flag2));
			NUnit.Framework.Assert.IsTrue(set.Remove(flag1));
			NUnit.Framework.Assert.IsFalse(set.Remove(flag1));
			NUnit.Framework.Assert.AreEqual(flag2.mask, set.mask);
			NUnit.Framework.Assert.IsFalse(set.Contains(flag1));
		}

		public virtual void TestContains()
		{
			RevFlag flag1 = rw.NewFlag("flag_1");
			RevFlag flag2 = rw.NewFlag("flag_2");
			RevFlagSet set = new RevFlagSet();
			set.AddItem(flag1);
			NUnit.Framework.Assert.IsTrue(set.Contains(flag1));
			NUnit.Framework.Assert.IsFalse(set.Contains(flag2));
			NUnit.Framework.Assert.IsFalse(set.Contains("bob"));
		}
	}
}
