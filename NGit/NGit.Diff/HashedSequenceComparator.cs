using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	/// <summary>
	/// Wrap another comparator for use with
	/// <see cref="HashedSequence{S}">HashedSequence&lt;S&gt;</see>
	/// .
	/// This comparator acts as a proxy for the real comparator, evaluating the
	/// cached hash code before testing the underlying comparator's equality.
	/// Comparators of this type must be used with a
	/// <see cref="HashedSequence{S}">HashedSequence&lt;S&gt;</see>
	/// .
	/// To construct an instance of this type use
	/// <see cref="HashedSequencePair{S}">HashedSequencePair&lt;S&gt;</see>
	/// .
	/// </summary>
	/// <?></?>
	public sealed class HashedSequenceComparator<S> : SequenceComparator<HashedSequence
		<S>> where S:Sequence
	{
		private readonly SequenceComparator<S> cmp;

		internal HashedSequenceComparator(SequenceComparator<S> cmp)
		{
			this.cmp = cmp;
		}

		public override bool Equals(HashedSequence<S> a, int ai, HashedSequence<S> b, int
			 bi)
		{
			//
			return a.hashes[ai] == b.hashes[bi] && cmp.Equals(a.@base, ai, b.@base, bi);
		}

		public override int Hash(HashedSequence<S> seq, int ptr)
		{
			return seq.hashes[ptr];
		}
	}
}
