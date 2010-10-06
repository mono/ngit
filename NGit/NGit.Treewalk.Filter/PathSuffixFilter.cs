using System;
using NGit;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Treewalk.Filter
{
	/// <summary>Includes tree entries only if they match the configured path.</summary>
	/// <remarks>Includes tree entries only if they match the configured path.</remarks>
	public class PathSuffixFilter : TreeFilter
	{
		/// <summary>Create a new tree filter for a user supplied path.</summary>
		/// <remarks>
		/// Create a new tree filter for a user supplied path.
		/// <p>
		/// Path strings use '/' to delimit directories on all platforms.
		/// </remarks>
		/// <param name="path">the path (suffix) to filter on. Must not be the empty string.</param>
		/// <returns>a new filter for the requested path.</returns>
		/// <exception cref="System.ArgumentException">the path supplied was the empty string.
		/// 	</exception>
		public static NGit.Treewalk.Filter.PathSuffixFilter Create(string path)
		{
			if (path.Length == 0)
			{
				throw new ArgumentException(JGitText.Get().emptyPathNotPermitted);
			}
			return new NGit.Treewalk.Filter.PathSuffixFilter(path);
		}

		internal readonly string pathStr;

		internal readonly byte[] pathRaw;

		private PathSuffixFilter(string s)
		{
			pathStr = s;
			pathRaw = Constants.Encode(pathStr);
		}

		public override TreeFilter Clone()
		{
			return this;
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override bool Include(TreeWalk walker)
		{
			if (walker.IsSubtree)
			{
				return true;
			}
			else
			{
				return walker.IsPathSuffix(pathRaw, pathRaw.Length);
			}
		}

		public override bool ShouldBeRecursive()
		{
			return true;
		}
	}
}
