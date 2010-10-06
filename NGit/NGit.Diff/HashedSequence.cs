using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	/// <summary>
	/// Wraps a
	/// <see cref="Sequence">Sequence</see>
	/// to assign hash codes to elements.
	/// This sequence acts as a proxy for the real sequence, caching element hash
	/// codes so they don't need to be recomputed each time. Sequences of this type
	/// must be used with a
	/// <see cref="HashedSequenceComparator{S}">HashedSequenceComparator&lt;S&gt;</see>
	/// .
	/// To construct an instance of this type use
	/// <see cref="HashedSequencePair{S}">HashedSequencePair&lt;S&gt;</see>
	/// .
	/// </summary>
	/// <?></?>
	public sealed class HashedSequence<S> : Sequence where S:Sequence
	{
		internal readonly S @base;

		internal readonly int[] hashes;

		internal HashedSequence(S @base, int[] hashes)
		{
			this.@base = @base;
			this.hashes = hashes;
		}

		public override int Size()
		{
			return @base.Size();
		}
	}
}
