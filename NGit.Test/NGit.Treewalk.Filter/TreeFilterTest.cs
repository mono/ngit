using NGit;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Treewalk.Filter
{
	public class TreeFilterTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestALL_IncludesAnything()
		{
			TreeWalk tw = new TreeWalk(db);
			NUnit.Framework.Assert.IsTrue(TreeFilter.ALL.Include(tw));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestALL_ShouldNotBeRecursive()
		{
			NUnit.Framework.Assert.IsFalse(TreeFilter.ALL.ShouldBeRecursive());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestALL_IdentityClone()
		{
			NUnit.Framework.Assert.AreSame(TreeFilter.ALL, TreeFilter.ALL.Clone());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNotALL_IncludesNothing()
		{
			TreeWalk tw = new TreeWalk(db);
			NUnit.Framework.Assert.IsFalse(TreeFilter.ALL.Negate().Include(tw));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestANY_DIFF_IncludesSingleTreeCase()
		{
			TreeWalk tw = new TreeWalk(db);
			NUnit.Framework.Assert.IsTrue(TreeFilter.ANY_DIFF.Include(tw));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestANY_DIFF_ShouldNotBeRecursive()
		{
			NUnit.Framework.Assert.IsFalse(TreeFilter.ANY_DIFF.ShouldBeRecursive());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestANY_DIFF_IdentityClone()
		{
			NUnit.Framework.Assert.AreSame(TreeFilter.ANY_DIFF, TreeFilter.ANY_DIFF.Clone());
		}
	}
}
