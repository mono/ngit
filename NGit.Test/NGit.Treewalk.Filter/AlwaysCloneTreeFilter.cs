using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Treewalk.Filter
{
	internal class AlwaysCloneTreeFilter : TreeFilter
	{
		public override TreeFilter Clone()
		{
			return new AlwaysCloneTreeFilter();
		}

		public override bool Include(TreeWalk walker)
		{
			return false;
		}

		public override bool ShouldBeRecursive()
		{
			return false;
		}
	}
}
