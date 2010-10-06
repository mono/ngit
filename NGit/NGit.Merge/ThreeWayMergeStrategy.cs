using NGit;
using NGit.Merge;
using Sharpen;

namespace NGit.Merge
{
	/// <summary>A merge strategy to merge 2 trees, using a common base ancestor tree.</summary>
	/// <remarks>A merge strategy to merge 2 trees, using a common base ancestor tree.</remarks>
	public abstract class ThreeWayMergeStrategy : MergeStrategy
	{
		public abstract override Merger NewMerger(Repository db);

		public abstract override Merger NewMerger(Repository db, bool inCore);
	}
}
