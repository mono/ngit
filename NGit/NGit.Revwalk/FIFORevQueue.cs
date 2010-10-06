using System.Text;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>A queue of commits in FIFO order.</summary>
	/// <remarks>A queue of commits in FIFO order.</remarks>
	public class FIFORevQueue : BlockRevQueue
	{
		private BlockRevQueue.Block head;

		private BlockRevQueue.Block tail;

		/// <summary>Create an empty FIFO queue.</summary>
		/// <remarks>Create an empty FIFO queue.</remarks>
		public FIFORevQueue() : base()
		{
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		internal FIFORevQueue(Generator s) : base(s)
		{
		}

		public override void Add(RevCommit c)
		{
			BlockRevQueue.Block b = tail;
			if (b == null)
			{
				b = free.NewBlock();
				b.Add(c);
				head = b;
				tail = b;
				return;
			}
			else
			{
				if (b.IsFull())
				{
					b = free.NewBlock();
					tail.next = b;
					tail = b;
				}
			}
			b.Add(c);
		}

		/// <summary>Insert the commit pointer at the front of the queue.</summary>
		/// <remarks>Insert the commit pointer at the front of the queue.</remarks>
		/// <param name="c">the commit to insert into the queue.</param>
		public virtual void Unpop(RevCommit c)
		{
			BlockRevQueue.Block b = head;
			if (b == null)
			{
				b = free.NewBlock();
				b.ResetToMiddle();
				b.Add(c);
				head = b;
				tail = b;
				return;
			}
			else
			{
				if (b.CanUnpop())
				{
					b.Unpop(c);
					return;
				}
			}
			b = free.NewBlock();
			b.ResetToEnd();
			b.Unpop(c);
			b.next = head;
			head = b;
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
				if (head == null)
				{
					tail = null;
				}
				free.FreeBlock(b);
			}
			return c;
		}

		public override void Clear()
		{
			head = null;
			tail = null;
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

		internal virtual void RemoveFlag(int f)
		{
			int not_f = ~f;
			for (BlockRevQueue.Block b = head; b != null; b = b.next)
			{
				for (int i = b.headIndex; i < b.tailIndex; i++)
				{
					b.commits[i].flags &= not_f;
				}
			}
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
