using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public abstract class RevQueueTestCase<T> : RevWalkTestCase where T:AbstractRevQueue
	{
		protected internal T q;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			q = Create();
		}

		protected internal abstract T Create();

		/// <exception cref="System.Exception"></exception>
		public virtual void TestEmpty()
		{
			NUnit.Framework.Assert.IsNull(q.Next());
			NUnit.Framework.Assert.IsTrue(q.EverbodyHasFlag(RevWalk.UNINTERESTING));
			NUnit.Framework.Assert.IsFalse(q.AnybodyHasFlag(RevWalk.UNINTERESTING));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestClear()
		{
			RevCommit a = ParseBody(Commit());
			RevCommit b = ParseBody(Commit(a));
			q.Add(a);
			q.Add(b);
			q.Clear();
			NUnit.Framework.Assert.IsNull(q.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestHasFlags()
		{
			RevCommit a = ParseBody(Commit());
			RevCommit b = ParseBody(Commit(a));
			q.Add(a);
			q.Add(b);
			NUnit.Framework.Assert.IsFalse(q.EverbodyHasFlag(RevWalk.UNINTERESTING));
			NUnit.Framework.Assert.IsFalse(q.AnybodyHasFlag(RevWalk.UNINTERESTING));
			a.flags |= RevWalk.UNINTERESTING;
			NUnit.Framework.Assert.IsFalse(q.EverbodyHasFlag(RevWalk.UNINTERESTING));
			NUnit.Framework.Assert.IsTrue(q.AnybodyHasFlag(RevWalk.UNINTERESTING));
			b.flags |= RevWalk.UNINTERESTING;
			NUnit.Framework.Assert.IsTrue(q.EverbodyHasFlag(RevWalk.UNINTERESTING));
			NUnit.Framework.Assert.IsTrue(q.AnybodyHasFlag(RevWalk.UNINTERESTING));
		}
	}
}
