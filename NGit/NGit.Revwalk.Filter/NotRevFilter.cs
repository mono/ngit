using NGit.Revwalk;
using NGit.Revwalk.Filter;
using Sharpen;

namespace NGit.Revwalk.Filter
{
	/// <summary>Includes a commit only if the subfilter does not include the commit.</summary>
	/// <remarks>Includes a commit only if the subfilter does not include the commit.</remarks>
	public class NotRevFilter : RevFilter
	{
		/// <summary>Create a filter that negates the result of another filter.</summary>
		/// <remarks>Create a filter that negates the result of another filter.</remarks>
		/// <param name="a">filter to negate.</param>
		/// <returns>a filter that does the reverse of <code>a</code>.</returns>
		public static RevFilter Create(RevFilter a)
		{
			return new NGit.Revwalk.Filter.NotRevFilter(a);
		}

		private readonly RevFilter a;

		private NotRevFilter(RevFilter one)
		{
			a = one;
		}

		public override RevFilter Negate()
		{
			return a;
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override bool Include(RevWalk walker, RevCommit c)
		{
			return !a.Include(walker, c);
		}

		public override RevFilter Clone()
		{
			return new NGit.Revwalk.Filter.NotRevFilter(a.Clone());
		}

		public override string ToString()
		{
			return "NOT " + a.ToString();
		}
	}
}
