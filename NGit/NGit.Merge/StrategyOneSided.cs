using NGit;
using NGit.Merge;
using Sharpen;

namespace NGit.Merge
{
	/// <summary>Trivial merge strategy to make the resulting tree exactly match an input.
	/// 	</summary>
	/// <remarks>
	/// Trivial merge strategy to make the resulting tree exactly match an input.
	/// <p>
	/// This strategy can be used to cauterize an entire side branch of history, by
	/// setting the output tree to one of the inputs, and ignoring any of the paths
	/// of the other inputs.
	/// </remarks>
	public class StrategyOneSided : MergeStrategy
	{
		private readonly string strategyName;

		private readonly int treeIndex;

		/// <summary>Create a new merge strategy to select a specific input tree.</summary>
		/// <remarks>Create a new merge strategy to select a specific input tree.</remarks>
		/// <param name="name">name of this strategy.</param>
		/// <param name="index">the position of the input tree to accept as the result.</param>
		protected internal StrategyOneSided(string name, int index)
		{
			strategyName = name;
			treeIndex = index;
		}

		public override string GetName()
		{
			return strategyName;
		}

		public override Merger NewMerger(Repository db)
		{
			return new StrategyOneSided.OneSide(db, treeIndex);
		}

		public override Merger NewMerger(Repository db, bool inCore)
		{
			return new StrategyOneSided.OneSide(db, treeIndex);
		}

		internal class OneSide : Merger
		{
			private readonly int treeIndex;

			protected internal OneSide(Repository local, int index) : base(local)
			{
				treeIndex = index;
			}

			/// <exception cref="System.IO.IOException"></exception>
			protected internal override bool MergeImpl()
			{
				return treeIndex < sourceTrees.Length;
			}

			public override ObjectId GetResultTreeId()
			{
				return sourceTrees[treeIndex];
			}
		}
	}
}
