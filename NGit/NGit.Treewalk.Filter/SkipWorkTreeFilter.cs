using NGit.Dircache;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Treewalk.Filter
{
	/// <summary>
	/// To be used in combination with a DirCacheIterator: includes only tree entries
	/// for which 'skipWorkTree' flag is not set.
	/// </summary>
	/// <remarks>
	/// To be used in combination with a DirCacheIterator: includes only tree entries
	/// for which 'skipWorkTree' flag is not set.
	/// </remarks>
	public class SkipWorkTreeFilter : TreeFilter
	{
		/// <summary>Index of DirCacheIterator to work on.</summary>
		/// <remarks>Index of DirCacheIterator to work on.</remarks>
		private readonly int treeIdx;

		/// <summary>Create a filter to work on the specified DirCacheIterator.</summary>
		/// <remarks>Create a filter to work on the specified DirCacheIterator.</remarks>
		/// <param name="treeIdx">
		/// index of DirCacheIterator to work on. If the index does not
		/// refer to a DirCacheIterator, the filter will include all
		/// entries.
		/// </param>
		public SkipWorkTreeFilter(int treeIdx)
		{
			this.treeIdx = treeIdx;
		}

		public override bool Include(TreeWalk walker)
		{
			DirCacheIterator i = walker.GetTree<DirCacheIterator>(treeIdx);
			if (i == null)
			{
				return true;
			}
			DirCacheEntry e = i.GetDirCacheEntry();
			return e == null || !e.IsSkipWorkTree();
		}

		public override bool ShouldBeRecursive()
		{
			return false;
		}

		public override TreeFilter Clone()
		{
			return this;
		}

		public override string ToString()
		{
			return "SkipWorkTree(" + treeIdx + ")";
		}
	}
}
