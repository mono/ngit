using NGit.Merge;
using Sharpen;

namespace NGit.Merge
{
	/// <summary>One chunk from a merge result.</summary>
	/// <remarks>
	/// One chunk from a merge result. Each chunk contains a range from a
	/// single sequence. In case of conflicts multiple chunks are reported for one
	/// conflict. The conflictState tells when conflicts start and end.
	/// </remarks>
	public class MergeChunk
	{
		/// <summary>A state telling whether a MergeChunk belongs to a conflict or not.</summary>
		/// <remarks>
		/// A state telling whether a MergeChunk belongs to a conflict or not. The
		/// first chunk of a conflict is reported with a special state to be able to
		/// distinguish the border between two consecutive conflicts
		/// </remarks>
		public enum ConflictState
		{
			NO_CONFLICT,
			FIRST_CONFLICTING_RANGE,
			NEXT_CONFLICTING_RANGE
		}

		private readonly int sequenceIndex;

		private readonly int begin;

		private readonly int end;

		private readonly MergeChunk.ConflictState conflictState;

		/// <summary>Creates a new empty MergeChunk</summary>
		/// <param name="sequenceIndex">
		/// determines to which sequence this chunks belongs to. Same as
		/// in
		/// <see cref="MergeResult{S}.Add(int, int, int, ConflictState)">MergeResult&lt;S&gt;.Add(int, int, int, ConflictState)
		/// 	</see>
		/// </param>
		/// <param name="begin">
		/// the first element from the specified sequence which should be
		/// included in the merge result. Indexes start with 0.
		/// </param>
		/// <param name="end">
		/// specifies the end of the range to be added. The element this
		/// index points to is the first element which not added to the
		/// merge result. All elements between begin (including begin) and
		/// this element are added.
		/// </param>
		/// <param name="conflictState">
		/// the state of this chunk. See
		/// <see cref="ConflictState">ConflictState</see>
		/// </param>
		protected internal MergeChunk(int sequenceIndex, int begin, int end, MergeChunk.ConflictState
			 conflictState)
		{
			this.sequenceIndex = sequenceIndex;
			this.begin = begin;
			this.end = end;
			this.conflictState = conflictState;
		}

		/// <returns>
		/// the index of the sequence to which sequence this chunks belongs
		/// to. Same as in
		/// <see cref="MergeResult{S}.Add(int, int, int, ConflictState)">MergeResult&lt;S&gt;.Add(int, int, int, ConflictState)
		/// 	</see>
		/// </returns>
		public virtual int GetSequenceIndex()
		{
			return sequenceIndex;
		}

		/// <returns>
		/// the first element from the specified sequence which should be
		/// included in the merge result. Indexes start with 0.
		/// </returns>
		public virtual int GetBegin()
		{
			return begin;
		}

		/// <returns>
		/// the end of the range of this chunk. The element this index
		/// points to is the first element which not added to the merge
		/// result. All elements between begin (including begin) and this
		/// element are added.
		/// </returns>
		public virtual int GetEnd()
		{
			return end;
		}

		/// <returns>
		/// the state of this chunk. See
		/// <see cref="ConflictState">ConflictState</see>
		/// </returns>
		public virtual MergeChunk.ConflictState GetConflictState()
		{
			return conflictState;
		}
	}
}
