using System;
using NGit.Diff;
using Sharpen;

namespace NGit.Diff
{
	/// <summary>
	/// Compares two
	/// <see cref="Sequence">Sequence</see>
	/// s to create an
	/// <see cref="EditList">EditList</see>
	/// of changes.
	/// An algorithm's
	/// <code>diff</code>
	/// method must be callable from concurrent threads
	/// without data collisions. This permits some algorithms to use a singleton
	/// pattern, with concurrent invocations using the same singleton. Other
	/// algorithms may support parameterization, in which case the caller can create
	/// a unique instance per thread.
	/// </summary>
	public abstract class DiffAlgorithm
	{
		/// <summary>Compare two sequences and identify a list of edits between them.</summary>
		/// <remarks>Compare two sequences and identify a list of edits between them.</remarks>
		/// <?></?>
		/// <param name="cmp">the comparator supplying the element equivalence function.</param>
		/// <param name="a">
		/// the first (also known as old or pre-image) sequence. Edits
		/// returned by this algorithm will reference indexes using the
		/// 'A' side:
		/// <see cref="Edit.GetBeginA()">Edit.GetBeginA()</see>
		/// ,
		/// <see cref="Edit.GetEndA()">Edit.GetEndA()</see>
		/// .
		/// </param>
		/// <param name="b">
		/// the second (also known as new or post-image) sequence. Edits
		/// returned by this algorithm will reference indexes using the
		/// 'B' side:
		/// <see cref="Edit.GetBeginB()">Edit.GetBeginB()</see>
		/// ,
		/// <see cref="Edit.GetEndB()">Edit.GetEndB()</see>
		/// .
		/// </param>
		/// <returns>
		/// a modifiable edit list comparing the two sequences. If empty, the
		/// sequences are identical according to
		/// <code>cmp</code>
		/// 's rules. The
		/// result list is never null.
		/// </returns>
		public virtual EditList Diff<S>(SequenceComparator<S> cmp, S a, S b) where 
			S:Sequence
		{
			Edit region = cmp.ReduceCommonStartEnd(a, b, CoverEdit(a, b));
			switch (region.GetType())
			{
				case Edit.Type.INSERT:
				case Edit.Type.DELETE:
				{
					return EditList.Singleton(region);
				}

				case Edit.Type.REPLACE:
				{
					SubsequenceComparator<S> cs = new SubsequenceComparator<S>(cmp);
					Subsequence<S> @as = Subsequence<S>.A(a, region);
					Subsequence<S> bs = Subsequence<S>.B(b, region);
					return Subsequence<S>.ToBase(DiffNonCommon(cs, @as, bs), @as, bs);
				}

				case Edit.Type.EMPTY:
				{
					return new EditList(0);
				}

				default:
				{
					throw new InvalidOperationException();
				}
			}
		}

		private static Edit CoverEdit<S>(S a, S b) where S:Sequence
		{
			return new Edit(0, a.Size(), 0, b.Size());
		}

		/// <summary>Compare two sequences and identify a list of edits between them.</summary>
		/// <remarks>
		/// Compare two sequences and identify a list of edits between them.
		/// This method should be invoked only after the two sequences have been
		/// proven to have no common starting or ending elements. The expected
		/// elimination of common starting and ending elements is automatically
		/// performed by the
		/// <see cref="Diff{S}(SequenceComparator{S}, Sequence, Sequence)">Diff&lt;S&gt;(SequenceComparator&lt;S&gt;, Sequence, Sequence)
		/// 	</see>
		/// method, which invokes this method using
		/// <see cref="Subsequence{S}">Subsequence&lt;S&gt;</see>
		/// s.
		/// </remarks>
		/// <?></?>
		/// <param name="cmp">the comparator supplying the element equivalence function.</param>
		/// <param name="a">
		/// the first (also known as old or pre-image) sequence. Edits
		/// returned by this algorithm will reference indexes using the
		/// 'A' side:
		/// <see cref="Edit.GetBeginA()">Edit.GetBeginA()</see>
		/// ,
		/// <see cref="Edit.GetEndA()">Edit.GetEndA()</see>
		/// .
		/// </param>
		/// <param name="b">
		/// the second (also known as new or post-image) sequence. Edits
		/// returned by this algorithm will reference indexes using the
		/// 'B' side:
		/// <see cref="Edit.GetBeginB()">Edit.GetBeginB()</see>
		/// ,
		/// <see cref="Edit.GetEndB()">Edit.GetEndB()</see>
		/// .
		/// </param>
		/// <returns>a modifiable edit list comparing the two sequences.</returns>
		public abstract EditList DiffNonCommon<S>(SequenceComparator<S> cmp, S a, 
			S b) where S:Sequence;
	}
}
