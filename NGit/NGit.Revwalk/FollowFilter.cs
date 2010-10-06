using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>Updates the internal path filter to follow copy/renames.</summary>
	/// <remarks>
	/// Updates the internal path filter to follow copy/renames.
	/// <p>
	/// This is a special filter that performs
	/// <code>AND(path, ANY_DIFF)</code>
	/// , but also
	/// triggers rename detection so that the path node is updated to include a prior
	/// file name as the RevWalk traverses history.
	/// <p>
	/// Results with this filter are unpredictable if the path being followed is a
	/// subdirectory.
	/// </remarks>
	public class FollowFilter : TreeFilter
	{
		/// <summary>Create a new tree filter for a user supplied path.</summary>
		/// <remarks>
		/// Create a new tree filter for a user supplied path.
		/// <p>
		/// Path strings are relative to the root of the repository. If the user's
		/// input should be assumed relative to a subdirectory of the repository the
		/// caller must prepend the subdirectory's path prior to creating the filter.
		/// <p>
		/// Path strings use '/' to delimit directories on all platforms.
		/// </remarks>
		/// <param name="path">
		/// the path to filter on. Must not be the empty string. All
		/// trailing '/' characters will be trimmed before string's length
		/// is checked or is used as part of the constructed filter.
		/// </param>
		/// <returns>a new filter for the requested path.</returns>
		/// <exception cref="System.ArgumentException">the path supplied was the empty string.
		/// 	</exception>
		public static NGit.Revwalk.FollowFilter Create(string path)
		{
			return new NGit.Revwalk.FollowFilter(PathFilter.Create(path));
		}

		private readonly PathFilter path;

		internal FollowFilter(PathFilter path)
		{
			this.path = path;
		}

		/// <returns>the path this filter matches.</returns>
		public virtual string GetPath()
		{
			return path.GetPath();
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override bool Include(TreeWalk walker)
		{
			return path.Include(walker) && ANY_DIFF.Include(walker);
		}

		public override bool ShouldBeRecursive()
		{
			return path.ShouldBeRecursive() || ANY_DIFF.ShouldBeRecursive();
		}

		public override TreeFilter Clone()
		{
			return new NGit.Revwalk.FollowFilter(((PathFilter)path.Clone()));
		}

		public override string ToString()
		{
			return "(FOLLOW(" + path.ToString() + ")" + " AND " + ANY_DIFF.ToString() + ")";
		}
		//
		//
	}
}
