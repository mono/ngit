using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	/// <summary>Arbitrary sequence of elements.</summary>
	/// <remarks>
	/// Arbitrary sequence of elements.
	/// A sequence of elements is defined to contain elements in the index range
	/// <code>[0,
	/// <see cref="Size()">Size()</see>
	/// )</code>, like a standard Java List implementation.
	/// Unlike a List, the members of the sequence are not directly obtainable.
	/// Implementations of Sequence are primarily intended for use in content
	/// difference detection algorithms, to produce an
	/// <see cref="EditList">EditList</see>
	/// of
	/// <see cref="Edit">Edit</see>
	/// instances describing how two Sequence instances differ.
	/// To be compared against another Sequence of the same type, a supporting
	/// <see cref="SequenceComparator{S}">SequenceComparator&lt;S&gt;</see>
	/// must also be supplied.
	/// </remarks>
	public abstract class Sequence
	{
		/// <returns>total number of items in the sequence.</returns>
		public abstract int Size();
	}
}
