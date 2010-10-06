using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>
	/// Delays commits to be at least
	/// <see cref="PendingGenerator.OVER_SCAN">PendingGenerator.OVER_SCAN</see>
	/// late.
	/// <p>
	/// This helps to "fix up" weird corner cases resulting from clock skew, by
	/// slowing down what we produce to the caller we get a better chance to ensure
	/// PendingGenerator reached back far enough in the graph to correctly mark
	/// commits
	/// <see cref="RevWalk.UNINTERESTING">RevWalk.UNINTERESTING</see>
	/// if necessary.
	/// <p>
	/// This generator should appear before
	/// <see cref="FixUninterestingGenerator">FixUninterestingGenerator</see>
	/// if the
	/// lower level
	/// <see cref="pending">pending</see>
	/// isn't already fully buffered.
	/// </summary>
	internal sealed class DelayRevQueue : Generator
	{
		private const int OVER_SCAN = PendingGenerator.OVER_SCAN;

		private readonly Generator pending;

		private readonly FIFORevQueue delay;

		private int size;

		internal DelayRevQueue(Generator g)
		{
			pending = g;
			delay = new FIFORevQueue();
		}

		internal override int OutputType()
		{
			return pending.OutputType();
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		internal override RevCommit Next()
		{
			while (size < OVER_SCAN)
			{
				RevCommit c = pending.Next();
				if (c == null)
				{
					break;
				}
				delay.Add(c);
				size++;
			}
			RevCommit c_1 = delay.Next();
			if (c_1 == null)
			{
				return null;
			}
			size--;
			return c_1;
		}
	}
}
