using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Treewalk.Filter
{
	/// <summary>
	/// Skip
	/// <see cref="NGit.Treewalk.WorkingTreeIterator">NGit.Treewalk.WorkingTreeIterator</see>
	/// entries that appear in gitignore files.
	/// </summary>
	public class NotIgnoredFilter : TreeFilter
	{
		private readonly int index;

		/// <summary>Construct a filter to ignore paths known to a particular iterator.</summary>
		/// <remarks>Construct a filter to ignore paths known to a particular iterator.</remarks>
		/// <param name="workdirTreeIndex">index of the workdir tree in the tree walk</param>
		public NotIgnoredFilter(int workdirTreeIndex)
		{
			this.index = workdirTreeIndex;
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override bool Include(TreeWalk tw)
		{
			WorkingTreeIterator i = tw.GetTree<WorkingTreeIterator>(index);
			return i == null || !i.IsEntryIgnored();
		}

		public override bool ShouldBeRecursive()
		{
			return false;
		}

		public override TreeFilter Clone()
		{
			// immutable
			return this;
		}

		public override string ToString()
		{
			return "NotIgnored(" + index + ")";
		}
	}
}
