using System;
using NGit.Revwalk;
using NGit.Revwalk.Filter;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Revwalk
{
	public class RevWalkMergeBaseTest : RevWalkTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestNone()
		{
			RevCommit c1 = Commit(Commit(Commit()));
			RevCommit c2 = Commit(Commit(Commit()));
			rw.SetRevFilter(RevFilter.MERGE_BASE);
			MarkStart(c1);
			MarkStart(c2);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDisallowTreeFilter()
		{
			RevCommit c1 = Commit();
			RevCommit c2 = Commit();
			rw.SetRevFilter(RevFilter.MERGE_BASE);
			rw.SetTreeFilter(TreeFilter.ANY_DIFF);
			MarkStart(c1);
			MarkStart(c2);
			try
			{
				NUnit.Framework.Assert.IsNull(rw.Next());
				NUnit.Framework.Assert.Fail("did not throw IllegalStateException");
			}
			catch (InvalidOperationException)
			{
			}
		}

		// expected result
		/// <exception cref="System.Exception"></exception>
		public virtual void TestSimple()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c1 = Commit(Commit(Commit(Commit(Commit(b)))));
			RevCommit c2 = Commit(Commit(Commit(Commit(Commit(b)))));
			rw.SetRevFilter(RevFilter.MERGE_BASE);
			MarkStart(c1);
			MarkStart(c2);
			AssertCommit(b, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMultipleHeads_SameBase1()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c1 = Commit(Commit(Commit(Commit(Commit(b)))));
			RevCommit c2 = Commit(Commit(Commit(Commit(Commit(b)))));
			RevCommit c3 = Commit(Commit(Commit(b)));
			rw.SetRevFilter(RevFilter.MERGE_BASE);
			MarkStart(c1);
			MarkStart(c2);
			MarkStart(c3);
			AssertCommit(b, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMultipleHeads_SameBase2()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			RevCommit d1 = Commit(Commit(Commit(Commit(Commit(b)))));
			RevCommit d2 = Commit(Commit(Commit(Commit(Commit(c)))));
			RevCommit d3 = Commit(Commit(Commit(c)));
			rw.SetRevFilter(RevFilter.MERGE_BASE);
			MarkStart(d1);
			MarkStart(d2);
			MarkStart(d3);
			AssertCommit(b, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCrissCross()
		{
			// See http://marc.info/?l=git&m=111463358500362&w=2 for a nice
			// description of what this test is creating. We don't have a
			// clean merge base for d,e as they each merged the parents b,c
			// in different orders.
			//
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(a);
			RevCommit d = Commit(b, c);
			RevCommit e = Commit(c, b);
			rw.SetRevFilter(RevFilter.MERGE_BASE);
			MarkStart(d);
			MarkStart(e);
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}
	}
}
