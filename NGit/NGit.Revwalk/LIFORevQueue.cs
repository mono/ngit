using System.Text;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>A queue of commits in LIFO order.</summary>
	/// <remarks>A queue of commits in LIFO order.</remarks>
	public class LIFORevQueue : BlockRevQueue
	{
		private BlockRevQueue.Block head;

		/// <summary>Create an empty LIFO queue.</summary>
		/// <remarks>Create an empty LIFO queue.</remarks>
		public LIFORevQueue() : base()
		{
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		internal LIFORevQueue(Generator s) : base(s)
		{
		}

		public override void Add(RevCommit c)
		{
			BlockRevQueue.Block b = head;
			if (b == null || !b.CanUnpop())
			{
				b = free.NewBlock();
				b.ResetToEnd();
				b.next = head;
				head = b;
			}
			b.Unpop(c);
		}

		internal override RevCommit Next()
		{
			BlockRevQueue.Block b = head;
			if (b == null)
			{
				return null;
			}
			RevCommit c = b.Pop();
			if (b.IsEmpty())
			{
				head = b.next;
				free.FreeBlock(b);
			}
			return c;
		}

		public override void Clear()
		{
			head = null;
			free.Clear();
		}

		internal override bool EverbodyHasFlag(int f)
		{
			for (BlockRevQueue.Block b = head; b != null; b = b.next)
			{
				for (int i = b.headIndex; i < b.tailIndex; i++)
				{
					if ((b.commits[i].flags & f) == 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		internal override bool AnybodyHasFlag(int f)
		{
			for (BlockRevQueue.Block b = head; b != null; b = b.next)
			{
				for (int i = b.headIndex; i < b.tailIndex; i++)
				{
					if ((b.commits[i].flags & f) != 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			for (BlockRevQueue.Block q = head; q != null; q = q.next)
			{
				for (int i = q.headIndex; i < q.tailIndex; i++)
				{
					Describe(s, q.commits[i]);
				}
			}
			return s.ToString();
		}
	}
}
