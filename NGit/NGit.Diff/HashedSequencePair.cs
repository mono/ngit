using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	/// <summary>
	/// Wraps two
	/// <see cref="Sequence">Sequence</see>
	/// instances to cache their element hash codes.
	/// This pair wraps two sequences that contain cached hash codes for the input
	/// sequences.
	/// </summary>
	/// <?></?>
	public class HashedSequencePair<S> where S:Sequence
	{
		private readonly SequenceComparator<S> cmp;

		private readonly S baseA;

		private readonly S baseB;

		private HashedSequence<S> cachedA;

		private HashedSequence<S> cachedB;

		/// <summary>Construct a pair to provide fast hash codes.</summary>
		/// <remarks>Construct a pair to provide fast hash codes.</remarks>
		/// <param name="cmp">the base comparator for the sequence elements.</param>
		/// <param name="a">the A sequence.</param>
		/// <param name="b">the B sequence.</param>
		public HashedSequencePair(SequenceComparator<S> cmp, S a, S b)
		{
			this.cmp = cmp;
			this.baseA = a;
			this.baseB = b;
		}

		/// <returns>obtain a comparator that uses the cached hash codes.</returns>
		public virtual HashedSequenceComparator<S> GetComparator()
		{
			return new HashedSequenceComparator<S>(cmp);
		}

		/// <returns>wrapper around A that includes cached hash codes.</returns>
		public virtual HashedSequence<S> GetA()
		{
			if (cachedA == null)
			{
				cachedA = Wrap(baseA);
			}
			return cachedA;
		}

		/// <returns>wrapper around B that includes cached hash codes.</returns>
		public virtual HashedSequence<S> GetB()
		{
			if (cachedB == null)
			{
				cachedB = Wrap(baseB);
			}
			return cachedB;
		}

		private HashedSequence<S> Wrap(S @base)
		{
			int end = @base.Size();
			int[] hashes = new int[end];
			for (int ptr = 0; ptr < end; ptr++)
			{
				hashes[ptr] = cmp.Hash(@base, ptr);
			}
			return new HashedSequence<S>(@base, hashes);
		}
	}
}
