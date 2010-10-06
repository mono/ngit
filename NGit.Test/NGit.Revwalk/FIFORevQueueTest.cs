using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public class FIFORevQueueTest : RevQueueTestCase<FIFORevQueue>
	{
		protected internal override FIFORevQueue Create()
		{
			return new FIFORevQueue();
		}

		/// <exception cref="System.Exception"></exception>
		public override void TestEmpty()
		{
			base.TestEmpty();
			NUnit.Framework.Assert.AreEqual(0, q.OutputType());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCloneEmpty()
		{
			q = new FIFORevQueue(AbstractRevQueue.EMPTY_QUEUE);
			NUnit.Framework.Assert.IsNull(q.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestAddLargeBlocks()
		{
			AList<RevCommit> lst = new AList<RevCommit>();
			for (int i = 0; i < 3 * BlockRevQueue.Block.BLOCK_SIZE; i++)
			{
				RevCommit c = Commit();
				lst.AddItem(c);
				q.Add(c);
			}
			for (int i_1 = 0; i_1 < lst.Count; i_1++)
			{
				NUnit.Framework.Assert.AreSame(lst[i_1], q.Next());
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestUnpopAtFront()
		{
			RevCommit a = Commit();
			RevCommit b = Commit();
			RevCommit c = Commit();
			q.Add(a);
			q.Unpop(b);
			q.Unpop(c);
			NUnit.Framework.Assert.AreSame(c, q.Next());
			NUnit.Framework.Assert.AreSame(b, q.Next());
			NUnit.Framework.Assert.AreSame(a, q.Next());
		}
	}
}
