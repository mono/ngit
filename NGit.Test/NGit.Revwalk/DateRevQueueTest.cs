using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public class DateRevQueueTest : RevQueueTestCase<DateRevQueue>
	{
		protected internal override DateRevQueue Create()
		{
			return new DateRevQueue();
		}

		/// <exception cref="System.Exception"></exception>
		public override void TestEmpty()
		{
			base.TestEmpty();
			NUnit.Framework.Assert.IsNull(q.Peek());
			NUnit.Framework.Assert.AreEqual(Generator.SORT_COMMIT_TIME_DESC, q.OutputType());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCloneEmpty()
		{
			q = new DateRevQueue(AbstractRevQueue.EMPTY_QUEUE);
			NUnit.Framework.Assert.IsNull(q.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInsertOutOfOrder()
		{
			RevCommit a = ParseBody(Commit());
			RevCommit b = ParseBody(Commit(10, a));
			RevCommit c1 = ParseBody(Commit(5, b));
			RevCommit c2 = ParseBody(Commit(-50, b));
			q.Add(c2);
			q.Add(a);
			q.Add(b);
			q.Add(c1);
			AssertCommit(c1, q.Next());
			AssertCommit(b, q.Next());
			AssertCommit(a, q.Next());
			AssertCommit(c2, q.Next());
			NUnit.Framework.Assert.IsNull(q.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInsertTie()
		{
			RevCommit a = ParseBody(Commit());
			RevCommit b = ParseBody(Commit(0, a));
			{
				q = Create();
				q.Add(a);
				q.Add(b);
				AssertCommit(a, q.Next());
				AssertCommit(b, q.Next());
				NUnit.Framework.Assert.IsNull(q.Next());
			}
			{
				q = Create();
				q.Add(b);
				q.Add(a);
				AssertCommit(b, q.Next());
				AssertCommit(a, q.Next());
				NUnit.Framework.Assert.IsNull(q.Next());
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCloneFIFO()
		{
			RevCommit a = ParseBody(Commit());
			RevCommit b = ParseBody(Commit(200, a));
			RevCommit c = ParseBody(Commit(200, b));
			FIFORevQueue src = new FIFORevQueue();
			src.Add(a);
			src.Add(b);
			src.Add(c);
			q = new DateRevQueue(src);
			NUnit.Framework.Assert.IsFalse(q.EverbodyHasFlag(RevWalk.UNINTERESTING));
			NUnit.Framework.Assert.IsFalse(q.AnybodyHasFlag(RevWalk.UNINTERESTING));
			AssertCommit(c, q.Peek());
			AssertCommit(c, q.Peek());
			AssertCommit(c, q.Next());
			AssertCommit(b, q.Next());
			AssertCommit(a, q.Next());
			NUnit.Framework.Assert.IsNull(q.Next());
		}
	}
}
