using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Treewalk.Filter
{
	/// <summary>Includes an entry only if the subfilter does not include the entry.</summary>
	/// <remarks>Includes an entry only if the subfilter does not include the entry.</remarks>
	public class NotTreeFilter : TreeFilter
	{
		/// <summary>Create a filter that negates the result of another filter.</summary>
		/// <remarks>Create a filter that negates the result of another filter.</remarks>
		/// <param name="a">filter to negate.</param>
		/// <returns>a filter that does the reverse of <code>a</code>.</returns>
		public static TreeFilter Create(TreeFilter a)
		{
			return new NGit.Treewalk.Filter.NotTreeFilter(a);
		}

		private readonly TreeFilter a;

		private NotTreeFilter(TreeFilter one)
		{
			a = one;
		}

		public override TreeFilter Negate()
		{
			return a;
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override bool Include(TreeWalk walker)
		{
			return !a.Include(walker);
		}

		public override bool ShouldBeRecursive()
		{
			return a.ShouldBeRecursive();
		}

		public override TreeFilter Clone()
		{
			TreeFilter n = a.Clone();
			return n == a ? this : new NGit.Treewalk.Filter.NotTreeFilter(n);
		}

		public override string ToString()
		{
			return "NOT " + a.ToString();
		}
	}
}
