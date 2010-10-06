using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	/// <summary>
	/// Wrap another comparator for use with
	/// <see cref="Subsequence{S}">Subsequence&lt;S&gt;</see>
	/// .
	/// This comparator acts as a proxy for the real comparator, translating element
	/// indexes on the fly by adding the subsequence's begin offset to them.
	/// Comparators of this type must be used with a
	/// <see cref="Subsequence{S}">Subsequence&lt;S&gt;</see>
	/// .
	/// </summary>
	/// <?></?>
	public sealed class SubsequenceComparator<S> : SequenceComparator<Subsequence<S>>
		 where S:Sequence
	{
		private readonly SequenceComparator<S> cmp;

		/// <summary>Construct a comparator wrapping another comparator.</summary>
		/// <remarks>Construct a comparator wrapping another comparator.</remarks>
		/// <param name="cmp">the real comparator.</param>
		public SubsequenceComparator(SequenceComparator<S> cmp)
		{
			this.cmp = cmp;
		}

		public override bool Equals(Subsequence<S> a, int ai, Subsequence<S> b, int bi)
		{
			return cmp.Equals(a.@base, ai + a.begin, b.@base, bi + b.begin);
		}

		public override int Hash(Subsequence<S> seq, int ptr)
		{
			return cmp.Hash(seq.@base, ptr + seq.begin);
		}
	}
}
