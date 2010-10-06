using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public class RevWalkCullTest : RevWalkTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestProperlyCullAllAncestors1()
		{
			// Credit goes to Junio C Hamano <gitster@pobox.com> for this
			// test case in git-core (t/t6009-rev-list-parent.sh)
			//
			// We induce a clock skew so two is dated before one.
			//
			RevCommit a = Commit();
			RevCommit b = Commit(-2400, a);
			RevCommit c = Commit(b);
			RevCommit d = Commit(c);
			MarkStart(a);
			MarkUninteresting(d);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestProperlyCullAllAncestors2()
		{
			// Despite clock skew on c1 being very old it should not
			// produce, neither should a or b, or any part of that chain.
			//
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c1 = Commit(-5, b);
			RevCommit c2 = Commit(10, b);
			RevCommit d = Commit(c1, c2);
			MarkStart(d);
			MarkUninteresting(c1);
			AssertCommit(d, rw.Next());
			AssertCommit(c2, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestProperlyCullAllAncestors_LongHistory()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			for (int i = 0; i < 24; i++)
			{
				b = Commit(b);
				if ((i & 2) == 0)
				{
					MarkUninteresting(b);
				}
			}
			RevCommit c = Commit(b);
			MarkStart(c);
			MarkUninteresting(b);
			AssertCommit(c, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
			// We should have aborted before we got back so far that "a"
			// would be parsed. Thus, its parents shouldn't be allocated.
			//
			NUnit.Framework.Assert.IsNull(a.parents);
		}
	}
}
