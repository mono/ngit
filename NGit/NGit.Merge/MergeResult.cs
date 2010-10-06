using System;
using System.Collections.Generic;
using NGit.Diff;
using NGit.Merge;
using NGit.Util;
using Sharpen;

namespace NGit.Merge
{
	/// <summary>
	/// The result of merging a number of
	/// <see cref="NGit.Diff.Sequence">NGit.Diff.Sequence</see>
	/// objects. These sequences
	/// have one common predecessor sequence. The result of a merge is a list of
	/// MergeChunks. Each MergeChunk contains either a range (a subsequence) from
	/// one of the merged sequences, a range from the common predecessor or a
	/// conflicting range from one of the merged sequences. A conflict will be
	/// reported as multiple chunks, one for each conflicting range. The first chunk
	/// for a conflict is marked specially to distinguish the border between two
	/// consecutive conflicts.
	/// <p>
	/// This class does not know anything about how to present the merge result to
	/// the end-user. MergeFormatters have to be used to construct something human
	/// readable.
	/// </summary>
	/// <?></?>
	public class MergeResult<S> : Iterable<MergeChunk> where S:Sequence
	{
		private IList<S> sequences;

		private IntList chunks = new IntList();

		private bool containsConflicts = false;

		static MergeResult()
		{
			states = new MergeChunk.ConflictState[] {
				MergeChunk.ConflictState.NO_CONFLICT,
				MergeChunk.ConflictState.FIRST_CONFLICTING_RANGE,
				MergeChunk.ConflictState.NEXT_CONFLICTING_RANGE
			};
		}
		
		/// <summary>Creates a new empty MergeResult</summary>
		/// <param name="sequences">
		/// contains the common predecessor sequence at position 0
		/// followed by the merged sequences. This list should not be
		/// modified anymore during the lifetime of this
		/// <see cref="MergeResult{S}">MergeResult&lt;S&gt;</see>
		/// .
		/// </param>
		public MergeResult(IList<S> sequences)
		{
			this.sequences = sequences;
		}
		
		internal MergeResult<Sequence> Upcast ()
		{
			var r = new MergeResult<Sequence> (sequences.UpcastTo<S,Sequence> ());
			r.chunks = chunks;
			r.containsConflicts = containsConflicts;
			return r;
		}

		/// <summary>
		/// Adds a new range from one of the merged sequences or from the common
		/// predecessor.
		/// </summary>
		/// <remarks>
		/// Adds a new range from one of the merged sequences or from the common
		/// predecessor. This method can add conflicting and non-conflicting ranges
		/// controlled by the conflictState parameter
		/// </remarks>
		/// <param name="srcIdx">
		/// determines from which sequence this range comes. An index of
		/// x specifies the x+1 element in the list of sequences
		/// specified to the constructor
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
		/// when set to NO_CONLICT a non-conflicting range is added.
		/// This will end implicitly all open conflicts added before.
		/// </param>
		public virtual void Add(int srcIdx, int begin, int end, MergeChunk.ConflictState 
			conflictState)
		{
			chunks.Add((int)(conflictState));
			chunks.Add(srcIdx);
			chunks.Add(begin);
			chunks.Add(end);
			if (conflictState != MergeChunk.ConflictState.NO_CONFLICT)
			{
				containsConflicts = true;
			}
		}

		/// <summary>
		/// Returns the common predecessor sequence and the merged sequence in one
		/// list.
		/// </summary>
		/// <remarks>
		/// Returns the common predecessor sequence and the merged sequence in one
		/// list. The common predecessor is is the first element in the list
		/// </remarks>
		/// <returns>
		/// the common predecessor at position 0 followed by the merged
		/// sequences.
		/// </returns>
		public virtual IList<S> GetSequences()
		{
			return sequences;
		}

		private static readonly MergeChunk.ConflictState[] states;

		/// <returns>
		/// an iterator over the MergeChunks. The iterator does not support
		/// the remove operation
		/// </returns>
		public override Sharpen.Iterator<MergeChunk> Iterator()
		{
			return new _Iterator_137(this);
		}

		private sealed class _Iterator_137 : Sharpen.Iterator<MergeChunk>
		{
			public _Iterator_137(MergeResult<S> _enclosing)
			{
				this._enclosing = _enclosing;
			}

			internal int idx;

			public override bool HasNext()
			{
				return (this.idx < this._enclosing.chunks.Size());
			}

			public override MergeChunk Next()
			{
				MergeChunk.ConflictState state = NGit.Merge.MergeResult<S>.states[this._enclosing.chunks
					.Get(this.idx++)];
				int srcIdx = this._enclosing.chunks.Get(this.idx++);
				int begin = this._enclosing.chunks.Get(this.idx++);
				int end = this._enclosing.chunks.Get(this.idx++);
				return new MergeChunk(srcIdx, begin, end, state);
			}

			public override void Remove()
			{
				throw new NotSupportedException();
			}

			private readonly MergeResult<S> _enclosing;
		}

		/// <returns>true if this merge result contains conflicts</returns>
		public virtual bool ContainsConflicts()
		{
			return containsConflicts;
		}
	}
}
