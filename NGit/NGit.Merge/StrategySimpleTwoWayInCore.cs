using NGit;
using NGit.Dircache;
using NGit.Errors;
using NGit.Merge;
using NGit.Treewalk;
using Sharpen;

namespace NGit.Merge
{
	/// <summary>Merges two commits together in-memory, ignoring any working directory.</summary>
	/// <remarks>
	/// Merges two commits together in-memory, ignoring any working directory.
	/// <p>
	/// The strategy chooses a path from one of the two input trees if the path is
	/// unchanged in the other relative to their common merge base tree. This is a
	/// trivial 3-way merge (at the file path level only).
	/// <p>
	/// Modifications of the same file path (content and/or file mode) by both input
	/// trees will cause a merge conflict, as this strategy does not attempt to merge
	/// file contents.
	/// </remarks>
	public class StrategySimpleTwoWayInCore : ThreeWayMergeStrategy
	{
		/// <summary>Create a new instance of the strategy.</summary>
		/// <remarks>Create a new instance of the strategy.</remarks>
		public StrategySimpleTwoWayInCore()
		{
		}

		//
		public override string GetName()
		{
			return "simple-two-way-in-core";
		}

		public override Merger NewMerger(Repository db)
		{
			return new StrategySimpleTwoWayInCore.InCoreMerger(db);
		}

		public override Merger NewMerger(Repository db, bool inCore)
		{
			// This class is always inCore, so ignore the parameter
			return ((ThreeWayMerger)NewMerger(db));
		}

		private class InCoreMerger : ThreeWayMerger
		{
			private const int T_BASE = 0;

			private const int T_OURS = 1;

			private const int T_THEIRS = 2;

			private readonly NameConflictTreeWalk tw;

			private readonly DirCache cache;

			private DirCacheBuilder builder;

			private ObjectId resultTree;

			protected internal InCoreMerger(Repository local) : base(local)
			{
				tw = new NameConflictTreeWalk(reader);
				cache = DirCache.NewInCore();
			}

			/// <exception cref="System.IO.IOException"></exception>
			protected internal override bool MergeImpl()
			{
				tw.Reset();
				tw.AddTree(MergeBase());
				tw.AddTree(sourceTrees[0]);
				tw.AddTree(sourceTrees[1]);
				bool hasConflict = false;
				builder = cache.Builder();
				while (tw.Next())
				{
					int modeO = tw.GetRawMode(T_OURS);
					int modeT = tw.GetRawMode(T_THEIRS);
					if (modeO == modeT && tw.IdEqual(T_OURS, T_THEIRS))
					{
						Add(T_OURS, DirCacheEntry.STAGE_0);
						continue;
					}
					int modeB = tw.GetRawMode(T_BASE);
					if (modeB == modeO && tw.IdEqual(T_BASE, T_OURS))
					{
						Add(T_THEIRS, DirCacheEntry.STAGE_0);
					}
					else
					{
						if (modeB == modeT && tw.IdEqual(T_BASE, T_THEIRS))
						{
							Add(T_OURS, DirCacheEntry.STAGE_0);
						}
						else
						{
							if (tw.IsSubtree)
							{
								if (NonTree(modeB))
								{
									Add(T_BASE, DirCacheEntry.STAGE_1);
									hasConflict = true;
								}
								if (NonTree(modeO))
								{
									Add(T_OURS, DirCacheEntry.STAGE_2);
									hasConflict = true;
								}
								if (NonTree(modeT))
								{
									Add(T_THEIRS, DirCacheEntry.STAGE_3);
									hasConflict = true;
								}
								tw.EnterSubtree();
							}
							else
							{
								Add(T_BASE, DirCacheEntry.STAGE_1);
								Add(T_OURS, DirCacheEntry.STAGE_2);
								Add(T_THEIRS, DirCacheEntry.STAGE_3);
								hasConflict = true;
							}
						}
					}
				}
				builder.Finish();
				builder = null;
				if (hasConflict)
				{
					return false;
				}
				try
				{
					ObjectInserter odi = GetObjectInserter();
					resultTree = cache.WriteTree(odi);
					odi.Flush();
					return true;
				}
				catch (UnmergedPathException)
				{
					resultTree = null;
					return false;
				}
			}

			private static bool NonTree(int mode)
			{
				return mode != 0 && !FileMode.TREE.Equals(mode);
			}

			/// <exception cref="System.IO.IOException"></exception>
			private void Add(int tree, int stage)
			{
				AbstractTreeIterator i = GetTree(tree);
				if (i != null)
				{
					if (FileMode.TREE.Equals(tw.GetRawMode(tree)))
					{
						builder.AddTree(tw.RawPath, stage, reader, tw.GetObjectId(tree));
					}
					else
					{
						DirCacheEntry e;
						e = new DirCacheEntry(tw.RawPath, stage);
						e.SetObjectIdFromRaw(i.IdBuffer(), i.IdOffset());
						e.SetFileMode(tw.GetFileMode(tree));
						builder.Add(e);
					}
				}
			}

			private AbstractTreeIterator GetTree(int tree)
			{
				return tw.GetTree<AbstractTreeIterator>(tree);
			}

			public override ObjectId GetResultTreeId()
			{
				return resultTree;
			}
		}
	}
}
