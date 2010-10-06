using NGit;
using NGit.Merge;
using Sharpen;

namespace NGit.Merge
{
	/// <summary>A three-way merge strategy performing a content-merge if necessary</summary>
	public class StrategyResolve : ThreeWayMergeStrategy
	{
		public override Merger NewMerger(Repository db)
		{
			return new ResolveMerger(db, false);
		}

		public override Merger NewMerger(Repository db, bool inCore)
		{
			return new ResolveMerger(db, inCore);
		}

		public override string GetName()
		{
			return "resolve";
		}
	}
}
