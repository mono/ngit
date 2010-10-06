using System.Collections.Generic;
using System.Text;
using NGit;
using NGit.Api;
using NGit.Diff;
using NGit.Merge;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Encapsulates the result of a
	/// <see cref="MergeCommand">MergeCommand</see>
	/// .
	/// </summary>
	public class MergeCommandResult
	{
		private ObjectId[] mergedCommits;

		private ObjectId @base;

		private ObjectId newHead;

		private IDictionary<string, int[][]> conflicts;

		private MergeStatus mergeStatus;

		private string description;

		private MergeStrategy mergeStrategy;

		/// <param name="newHead">the object the head points at after the merge</param>
		/// <param name="base">
		/// the common base which was used to produce a content-merge. May
		/// be <code>null</code> if the merge-result was produced without
		/// computing a common base
		/// </param>
		/// <param name="mergedCommits">all the commits which have been merged together</param>
		/// <param name="mergeStatus">the status the merge resulted in</param>
		/// <param name="mergeStrategy">
		/// the used
		/// <see cref="NGit.Merge.MergeStrategy">NGit.Merge.MergeStrategy</see>
		/// </param>
		/// <param name="lowLevelResults">
		/// merge results as returned by
		/// <see cref="NGit.Merge.ResolveMerger.GetMergeResults()">NGit.Merge.ResolveMerger.GetMergeResults()
		/// 	</see>
		/// </param>
		public MergeCommandResult(ObjectId newHead, ObjectId @base, ObjectId[] mergedCommits
			, MergeStatus mergeStatus, IDictionary<string, NGit.Merge.MergeResult<Sequence>> lowLevelResults
			, MergeStrategy mergeStrategy) : this(newHead, @base, mergedCommits, mergeStatus
			, mergeStrategy, lowLevelResults, null)
		{
		}

		/// <param name="newHead">the object the head points at after the merge</param>
		/// <param name="base">
		/// the common base which was used to produce a content-merge. May
		/// be <code>null</code> if the merge-result was produced without
		/// computing a common base
		/// </param>
		/// <param name="mergedCommits">all the commits which have been merged together</param>
		/// <param name="mergeStatus">the status the merge resulted in</param>
		/// <param name="mergeStrategy">
		/// the used
		/// <see cref="NGit.Merge.MergeStrategy">NGit.Merge.MergeStrategy</see>
		/// </param>
		/// <param name="lowLevelResults">
		/// merge results as returned by
		/// <see cref="NGit.Merge.ResolveMerger.GetMergeResults()">NGit.Merge.ResolveMerger.GetMergeResults()
		/// 	</see>
		/// </param>
		/// <param name="description">a user friendly description of the merge result</param>
		public MergeCommandResult(ObjectId newHead, ObjectId @base, ObjectId[] mergedCommits
			, MergeStatus mergeStatus, MergeStrategy mergeStrategy, IDictionary<string, NGit.Merge.MergeResult
			<Sequence>> lowLevelResults, string description)
		{
			this.newHead = newHead;
			this.mergedCommits = mergedCommits;
			this.@base = @base;
			this.mergeStatus = mergeStatus;
			this.mergeStrategy = mergeStrategy;
			this.description = description;
			if (lowLevelResults != null)
			{
				foreach (string path in lowLevelResults.Keys)
				{
					AddConflict(path, lowLevelResults.Get(path));
				}
			}
		}

		/// <returns>the object the head points at after the merge</returns>
		public virtual ObjectId GetNewHead()
		{
			return newHead;
		}

		/// <returns>the status the merge resulted in</returns>
		public virtual MergeStatus GetMergeStatus()
		{
			return mergeStatus;
		}

		/// <returns>all the commits which have been merged together</returns>
		public virtual ObjectId[] GetMergedCommits()
		{
			return mergedCommits;
		}

		/// <returns>
		/// base the common base which was used to produce a content-merge.
		/// May be <code>null</code> if the merge-result was produced without
		/// computing a common base
		/// </returns>
		public virtual ObjectId GetBase()
		{
			return @base;
		}

		public override string ToString()
		{
			bool first = true;
			StringBuilder commits = new StringBuilder();
			foreach (ObjectId commit in mergedCommits)
			{
				if (!first)
				{
					commits.Append(", ");
				}
				else
				{
					first = false;
				}
				commits.Append(ObjectId.ToString(commit));
			}
			return MessageFormat.Format(JGitText.Get().mergeUsingStrategyResultedInDescription
				, commits, ObjectId.ToString(@base), mergeStrategy.GetName(), mergeStatus, (description
				 == null ? string.Empty : ", " + description));
		}

		/// <param name="conflicts">the conflicts to set</param>
		public virtual void SetConflicts(IDictionary<string, int[][]> conflicts)
		{
			this.conflicts = conflicts;
		}

		/// <param name="path"></param>
		/// <param name="conflictingRanges">the conflicts to set</param>
		public virtual void AddConflict(string path, int[][] conflictingRanges)
		{
			if (conflicts == null)
			{
				conflicts = new Dictionary<string, int[][]>();
			}
			conflicts.Put(path, conflictingRanges);
		}

		/// <param name="path"></param>
		/// <param name="lowLevelResult"></param>
		public virtual void AddConflict<_T0>(string path, NGit.Merge.MergeResult<_T0> lowLevelResult
			) where _T0:Sequence
		{
			if (conflicts == null)
			{
				conflicts = new Dictionary<string, int[][]>();
			}
			int nrOfConflicts = 0;
			// just counting
			foreach (MergeChunk mergeChunk in lowLevelResult)
			{
				if (mergeChunk.GetConflictState().Equals(MergeChunk.ConflictState.FIRST_CONFLICTING_RANGE
					))
				{
					nrOfConflicts++;
				}
			}
			int currentConflict = -1;
			int[][] ret = new int[nrOfConflicts][];
			for (int n = 0; n < nrOfConflicts; n++)
			{
				ret[n] = new int[mergedCommits.Length + 1];
			}
			foreach (MergeChunk mergeChunk_1 in lowLevelResult)
			{
				// to store the end of this chunk (end of the last conflicting range)
				int endOfChunk = 0;
				if (mergeChunk_1.GetConflictState().Equals(MergeChunk.ConflictState.FIRST_CONFLICTING_RANGE
					))
				{
					if (currentConflict > -1)
					{
						// there was a previous conflicting range for which the end
						// is not set yet - set it!
						ret[currentConflict][mergedCommits.Length] = endOfChunk;
					}
					currentConflict++;
					endOfChunk = mergeChunk_1.GetEnd();
					ret[currentConflict][mergeChunk_1.GetSequenceIndex()] = mergeChunk_1.GetBegin();
				}
				if (mergeChunk_1.GetConflictState().Equals(MergeChunk.ConflictState.NEXT_CONFLICTING_RANGE
					))
				{
					if (mergeChunk_1.GetEnd() > endOfChunk)
					{
						endOfChunk = mergeChunk_1.GetEnd();
					}
					ret[currentConflict][mergeChunk_1.GetSequenceIndex()] = mergeChunk_1.GetBegin();
				}
			}
			conflicts.Put(path, ret);
		}

		/// <summary>
		/// Returns information about the conflicts which occurred during a
		/// <see cref="MergeCommand">MergeCommand</see>
		/// . The returned value maps the path of a conflicting
		/// file to a two-dimensional int-array of line-numbers telling where in the
		/// file conflict markers for which merged commit can be found.
		/// <p>
		/// If the returned value contains a mapping "path"-&gt;[x][y]=z then this means
		/// <ul>
		/// <li>the file with path "path" contains conflicts</li>
		/// <li>if y &lt; "number of merged commits": for conflict number x in this file
		/// the chunk which was copied from commit number y starts on line number z.
		/// All numberings and line numbers start with 0.</li>
		/// <li>if y == "number of merged commits": the first non-conflicting line
		/// after conflict number x starts at line number z</li>
		/// </ul>
		/// <p>
		/// Example code how to parse this data:
		/// <pre> MergeResult m=...;
		/// Map<String, int[][]> allConflicts = m.getConflicts();
		/// for (String path : allConflicts.keySet()) {
		/// int[][] c = allConflicts.get(path);
		/// System.out.println("Conflicts in file " + path);
		/// for (int i = 0; i &lt; c.length; ++i) {
		/// System.out.println("  Conflict #" + i);
		/// for (int j = 0; j &lt; (c[i].length) - 1; ++j) {
		/// if (c[i][j] &gt;= 0)
		/// System.out.println("    Chunk for "
		/// + m.getMergedCommits()[j] + " starts on line #"
		/// + c[i][j]);
		/// }
		/// }
		/// }</pre>
		/// </summary>
		/// <returns>the conflicts or <code>null</code> if no conflict occured</returns>
		public virtual IDictionary<string, int[][]> GetConflicts()
		{
			return conflicts;
		}
	}

	/// <summary>The status the merge resulted in.</summary>
	/// <remarks>The status the merge resulted in.</remarks>
	public enum MergeStatus
	{
		FAST_FORWARD,
		ALREADY_UP_TO_DATE,
		FAILED,
		MERGED,
		CONFLICTING,
		NOT_SUPPORTED
	}
}
