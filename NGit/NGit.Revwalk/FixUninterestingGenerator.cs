using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>
	/// Filters out commits marked
	/// <see cref="RevWalk.UNINTERESTING">RevWalk.UNINTERESTING</see>
	/// .
	/// <p>
	/// This generator is only in front of another generator that has fully buffered
	/// commits, such that we are called only after the
	/// <see cref="PendingGenerator">PendingGenerator</see>
	/// has
	/// exhausted its input queue and given up. It skips over any uninteresting
	/// commits that may have leaked out of the PendingGenerator due to clock skew
	/// being detected in the commit objects.
	/// </summary>
	internal sealed class FixUninterestingGenerator : Generator
	{
		private readonly Generator pending;

		internal FixUninterestingGenerator(Generator g)
		{
			pending = g;
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
			for (; ; )
			{
				RevCommit c = pending.Next();
				if (c == null)
				{
					return null;
				}
				if ((c.flags & RevWalk.UNINTERESTING) == 0)
				{
					return c;
				}
			}
		}
	}
}
