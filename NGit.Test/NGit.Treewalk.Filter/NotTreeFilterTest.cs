using NGit;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Treewalk.Filter
{
	public class NotTreeFilterTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestWrap()
		{
			TreeWalk tw = new TreeWalk(db);
			TreeFilter a = TreeFilter.ALL;
			TreeFilter n = NotTreeFilter.Create(a);
			NUnit.Framework.Assert.IsNotNull(n);
			NUnit.Framework.Assert.IsTrue(a.Include(tw));
			NUnit.Framework.Assert.IsFalse(n.Include(tw));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNegateIsUnwrap()
		{
			TreeFilter a = PathFilter.Create("a/b");
			TreeFilter n = NotTreeFilter.Create(a);
			NUnit.Framework.Assert.AreSame(a, n.Negate());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestShouldBeRecursive_ALL()
		{
			TreeFilter a = TreeFilter.ALL;
			TreeFilter n = NotTreeFilter.Create(a);
			NUnit.Framework.Assert.AreEqual(a.ShouldBeRecursive(), n.ShouldBeRecursive());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestShouldBeRecursive_PathFilter()
		{
			TreeFilter a = PathFilter.Create("a/b");
			NUnit.Framework.Assert.IsTrue(a.ShouldBeRecursive());
			TreeFilter n = NotTreeFilter.Create(a);
			NUnit.Framework.Assert.IsTrue(n.ShouldBeRecursive());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCloneIsDeepClone()
		{
			TreeFilter a = new AlwaysCloneTreeFilter();
			NUnit.Framework.Assert.AreNotSame(a, a.Clone());
			TreeFilter n = NotTreeFilter.Create(a);
			NUnit.Framework.Assert.AreNotSame(n, n.Clone());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCloneIsSparseWhenPossible()
		{
			TreeFilter a = TreeFilter.ALL;
			NUnit.Framework.Assert.AreSame(a, a.Clone());
			TreeFilter n = NotTreeFilter.Create(a);
			NUnit.Framework.Assert.AreSame(n, n.Clone());
		}
	}
}
