using System;
using NGit;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Treewalk.Filter
{
	/// <summary>Includes tree entries only if they match the configured path.</summary>
	/// <remarks>
	/// Includes tree entries only if they match the configured path.
	/// <p>
	/// Applications should use
	/// <see cref="PathFilterGroup">PathFilterGroup</see>
	/// to connect these into a tree
	/// filter graph, as the group supports breaking out of traversal once it is
	/// known the path can never match.
	/// </remarks>
	public class PathFilter : TreeFilter
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
		public static NGit.Treewalk.Filter.PathFilter Create(string path)
		{
			while (path.EndsWith("/"))
			{
				path = Sharpen.Runtime.Substring(path, 0, path.Length - 1);
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(JGitText.Get().emptyPathNotPermitted);
			}
			return new NGit.Treewalk.Filter.PathFilter(path);
		}

		internal readonly string pathStr;

		internal readonly byte[] pathRaw;

		private PathFilter(string s)
		{
			pathStr = s;
			pathRaw = Constants.Encode(pathStr);
		}

		/// <returns>the path this filter matches.</returns>
		public virtual string GetPath()
		{
			return pathStr;
		}

		public override bool Include(TreeWalk walker)
		{
			return walker.IsPathPrefix(pathRaw, pathRaw.Length) == 0;
		}

		public override bool ShouldBeRecursive()
		{
			foreach (byte b in pathRaw)
			{
				if (b == '/')
				{
					return true;
				}
			}
			return false;
		}

		public override TreeFilter Clone()
		{
			return this;
		}

		public override string ToString()
		{
			return "PATH(\"" + pathStr + "\")";
		}
	}
}
