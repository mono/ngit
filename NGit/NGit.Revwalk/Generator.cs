using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>Produces commits for RevWalk to return to applications.</summary>
	/// <remarks>
	/// Produces commits for RevWalk to return to applications.
	/// <p>
	/// Implementations of this basic class provide the real work behind RevWalk.
	/// Conceptually a Generator is an iterator or a queue, it returns commits until
	/// there are no more relevant. Generators may be piped/stacked together to
	/// create a more complex set of operations.
	/// </remarks>
	/// <seealso cref="PendingGenerator">PendingGenerator</seealso>
	/// <seealso cref="StartGenerator">StartGenerator</seealso>
	public abstract class Generator
	{
		/// <summary>Commits are sorted by commit date and time, descending.</summary>
		/// <remarks>Commits are sorted by commit date and time, descending.</remarks>
		internal const int SORT_COMMIT_TIME_DESC = 1 << 0;

		/// <summary>
		/// Output may have
		/// <see cref="RevWalk.REWRITE">RevWalk.REWRITE</see>
		/// marked on it.
		/// </summary>
		internal const int HAS_REWRITE = 1 << 1;

		/// <summary>
		/// Output needs
		/// <see cref="RewriteGenerator">RewriteGenerator</see>
		/// .
		/// </summary>
		internal const int NEEDS_REWRITE = 1 << 2;

		/// <summary>Topological ordering is enforced (all children before parents).</summary>
		/// <remarks>Topological ordering is enforced (all children before parents).</remarks>
		internal const int SORT_TOPO = 1 << 3;

		/// <summary>
		/// Output may have
		/// <see cref="RevWalk.UNINTERESTING">RevWalk.UNINTERESTING</see>
		/// marked on it.
		/// </summary>
		internal const int HAS_UNINTERESTING = 1 << 4;

		/// <summary>Connect the supplied queue to this generator's own free list (if any).</summary>
		/// <remarks>Connect the supplied queue to this generator's own free list (if any).</remarks>
		/// <param name="q">another FIFO queue that wants to share our queue's free list.</param>
		internal virtual void ShareFreeList(BlockRevQueue q)
		{
		}

		// Do nothing by default.
		/// <summary>Obtain flags describing the output behavior of this generator.</summary>
		/// <remarks>Obtain flags describing the output behavior of this generator.</remarks>
		/// <returns>
		/// one or more of the constants declared in this class, describing
		/// how this generator produces its results.
		/// </returns>
		internal abstract int OutputType();

		/// <summary>Return the next commit to the application, or the next generator.</summary>
		/// <remarks>Return the next commit to the application, or the next generator.</remarks>
		/// <returns>next available commit; null if no more are to be returned.</returns>
		/// <exception cref="NGit.Errors.MissingObjectException">NGit.Errors.MissingObjectException
		/// 	</exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException">NGit.Errors.IncorrectObjectTypeException
		/// 	</exception>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		internal abstract RevCommit Next();
	}
}
