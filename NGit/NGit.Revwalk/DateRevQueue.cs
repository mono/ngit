using System.Text;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>A queue of commits sorted by commit time order.</summary>
	/// <remarks>A queue of commits sorted by commit time order.</remarks>
	public class DateRevQueue : AbstractRevQueue
	{
		private DateRevQueue.Entry head;

		private DateRevQueue.Entry free;

		/// <summary>Create an empty date queue.</summary>
		/// <remarks>Create an empty date queue.</remarks>
		public DateRevQueue() : base()
		{
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		internal DateRevQueue(Generator s)
		{
			for (; ; )
			{
				RevCommit c = s.Next();
				if (c == null)
				{
					break;
				}
				Add(c);
			}
		}

		public override void Add(RevCommit c)
		{
			DateRevQueue.Entry q = head;
			long when = c.commitTime;
			DateRevQueue.Entry n = NewEntry(c);
			if (q == null || when > q.commit.commitTime)
			{
				n.next = q;
				head = n;
			}
			else
			{
				DateRevQueue.Entry p = q.next;
				while (p != null && p.commit.commitTime > when)
				{
					q = p;
					p = q.next;
				}
				n.next = q.next;
				q.next = n;
			}
		}

		internal override RevCommit Next()
		{
			DateRevQueue.Entry q = head;
			if (q == null)
			{
				return null;
			}
			head = q.next;
			FreeEntry(q);
			return q.commit;
		}

		/// <summary>Peek at the next commit, without removing it.</summary>
		/// <remarks>Peek at the next commit, without removing it.</remarks>
		/// <returns>the next available commit; null if there are no commits left.</returns>
		public virtual RevCommit Peek()
		{
			return head != null ? head.commit : null;
		}

		public override void Clear()
		{
			head = null;
			free = null;
		}

		internal override bool EverbodyHasFlag(int f)
		{
			for (DateRevQueue.Entry q = head; q != null; q = q.next)
			{
				if ((q.commit.flags & f) == 0)
				{
					return false;
				}
			}
			return true;
		}

		internal override bool AnybodyHasFlag(int f)
		{
			for (DateRevQueue.Entry q = head; q != null; q = q.next)
			{
				if ((q.commit.flags & f) != 0)
				{
					return true;
				}
			}
			return false;
		}

		internal override int OutputType()
		{
			return outputType | SORT_COMMIT_TIME_DESC;
		}

		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			for (DateRevQueue.Entry q = head; q != null; q = q.next)
			{
				Describe(s, q.commit);
			}
			return s.ToString();
		}

		private DateRevQueue.Entry NewEntry(RevCommit c)
		{
			DateRevQueue.Entry r = free;
			if (r == null)
			{
				r = new DateRevQueue.Entry();
			}
			else
			{
				free = r.next;
			}
			r.commit = c;
			return r;
		}

		private void FreeEntry(DateRevQueue.Entry e)
		{
			e.next = free;
			free = e;
		}

		internal class Entry
		{
			internal DateRevQueue.Entry next;

			internal RevCommit commit;
		}
	}
}
