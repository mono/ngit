using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public class RevWalkSortTest : RevWalkTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_Default()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(1, a);
			RevCommit c = Commit(1, b);
			RevCommit d = Commit(1, c);
			MarkStart(d);
			AssertCommit(d, rw.Next());
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_COMMIT_TIME_DESC()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			RevCommit d = Commit(c);
			rw.Sort(RevSort.COMMIT_TIME_DESC);
			MarkStart(d);
			AssertCommit(d, rw.Next());
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_REVERSE()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			RevCommit d = Commit(c);
			rw.Sort(RevSort.REVERSE);
			MarkStart(d);
			AssertCommit(a, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(c, rw.Next());
			AssertCommit(d, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_COMMIT_TIME_DESC_OutOfOrder1()
		{
			// Despite being out of order time-wise, a strand-of-pearls must
			// still maintain topological order.
			//
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(-5, b);
			RevCommit d = Commit(10, c);
			NUnit.Framework.Assert.IsTrue(ParseBody(a).CommitTime < ParseBody(d).CommitTime);
			NUnit.Framework.Assert.IsTrue(ParseBody(c).CommitTime < ParseBody(b).CommitTime);
			rw.Sort(RevSort.COMMIT_TIME_DESC);
			MarkStart(d);
			AssertCommit(d, rw.Next());
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_COMMIT_TIME_DESC_OutOfOrder2()
		{
			// c1 is back dated before its parent.
			//
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c1 = Commit(-5, b);
			RevCommit c2 = Commit(10, b);
			RevCommit d = Commit(c1, c2);
			rw.Sort(RevSort.COMMIT_TIME_DESC);
			MarkStart(d);
			AssertCommit(d, rw.Next());
			AssertCommit(c2, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			AssertCommit(c1, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_TOPO()
		{
			// c1 is back dated before its parent.
			//
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c1 = Commit(-5, b);
			RevCommit c2 = Commit(10, b);
			RevCommit d = Commit(c1, c2);
			rw.Sort(RevSort.TOPO);
			MarkStart(d);
			AssertCommit(d, rw.Next());
			AssertCommit(c2, rw.Next());
			AssertCommit(c1, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_TOPO_REVERSE()
		{
			// c1 is back dated before its parent.
			//
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c1 = Commit(-5, b);
			RevCommit c2 = Commit(10, b);
			RevCommit d = Commit(c1, c2);
			rw.Sort(RevSort.TOPO);
			rw.Sort(RevSort.REVERSE, true);
			MarkStart(d);
			AssertCommit(a, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(c1, rw.Next());
			AssertCommit(c2, rw.Next());
			AssertCommit(d, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}
	}
}
