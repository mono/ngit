using System;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public class AlwaysEmptyRevQueueTest : RevWalkTestCase
	{
		private readonly AbstractRevQueue q = AbstractRevQueue.EMPTY_QUEUE;

		/// <exception cref="System.Exception"></exception>
		public virtual void TestEmpty()
		{
			NUnit.Framework.Assert.IsNull(q.Next());
			NUnit.Framework.Assert.IsTrue(q.EverbodyHasFlag(RevWalk.UNINTERESTING));
			NUnit.Framework.Assert.IsFalse(q.AnybodyHasFlag(RevWalk.UNINTERESTING));
			NUnit.Framework.Assert.AreEqual(0, q.OutputType());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestClear()
		{
			q.Clear();
			TestEmpty();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestAddFails()
		{
			try
			{
				q.Add(Commit());
				NUnit.Framework.Assert.Fail("Did not throw UnsupportedOperationException");
			}
			catch (NotSupportedException)
			{
			}
		}
		// expected result
	}
}
