using NGit.Revwalk;
using NGit.Revwalk.Filter;
using Sharpen;

namespace NGit.Revwalk.Filter
{
	/// <summary>Matches only commits with some/all RevFlags already set.</summary>
	/// <remarks>Matches only commits with some/all RevFlags already set.</remarks>
	public abstract class RevFlagFilter : RevFilter
	{
		/// <summary>Create a new filter that tests for a single flag.</summary>
		/// <remarks>Create a new filter that tests for a single flag.</remarks>
		/// <param name="a">the flag to test.</param>
		/// <returns>filter that selects only commits with flag <code>a</code>.</returns>
		public static RevFilter Has(RevFlag a)
		{
			RevFlagSet s = new RevFlagSet();
			s.AddItem(a);
			return new RevFlagFilter.HasAll(s);
		}

		/// <summary>Create a new filter that tests all flags in a set.</summary>
		/// <remarks>Create a new filter that tests all flags in a set.</remarks>
		/// <param name="a">set of flags to test.</param>
		/// <returns>filter that selects only commits with all flags in <code>a</code>.</returns>
		public static RevFilter HasAllFilter(params RevFlag[] a)
		{
			RevFlagSet set = new RevFlagSet();
			foreach (RevFlag flag in a)
			{
				set.AddItem(flag);
			}
			return new RevFlagFilter.HasAll(set);
		}

		/// <summary>Create a new filter that tests all flags in a set.</summary>
		/// <remarks>Create a new filter that tests all flags in a set.</remarks>
		/// <param name="a">set of flags to test.</param>
		/// <returns>filter that selects only commits with all flags in <code>a</code>.</returns>
		public static RevFilter HasAllFilter(RevFlagSet a)
		{
			return new RevFlagFilter.HasAll(new RevFlagSet(a));
		}

		/// <summary>Create a new filter that tests for any flag in a set.</summary>
		/// <remarks>Create a new filter that tests for any flag in a set.</remarks>
		/// <param name="a">set of flags to test.</param>
		/// <returns>filter that selects only commits with any flag in <code>a</code>.</returns>
		public static RevFilter HasAnyFilter(params RevFlag[] a)
		{
			RevFlagSet set = new RevFlagSet();
			foreach (RevFlag flag in a)
			{
				set.AddItem(flag);
			}
			return new RevFlagFilter.HasAny(set);
		}

		/// <summary>Create a new filter that tests for any flag in a set.</summary>
		/// <remarks>Create a new filter that tests for any flag in a set.</remarks>
		/// <param name="a">set of flags to test.</param>
		/// <returns>filter that selects only commits with any flag in <code>a</code>.</returns>
		public static RevFilter HasAnyFilter(RevFlagSet a)
		{
			return new RevFlagFilter.HasAny(new RevFlagSet(a));
		}

		internal readonly RevFlagSet flags;

		internal RevFlagFilter(RevFlagSet m)
		{
			flags = m;
		}

		public override RevFilter Clone()
		{
			return this;
		}

		public override string ToString()
		{
			return base.ToString() + flags;
		}

		private class HasAll : RevFlagFilter
		{
			internal HasAll(RevFlagSet m) : base(m)
			{
			}

			/// <exception cref="NGit.Errors.MissingObjectException"></exception>
			/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override bool Include(RevWalk walker, RevCommit c)
			{
				return c.HasAll(flags);
			}
		}

		private class HasAny : RevFlagFilter
		{
			internal HasAny(RevFlagSet m) : base(m)
			{
			}

			/// <exception cref="NGit.Errors.MissingObjectException"></exception>
			/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override bool Include(RevWalk walker, RevCommit c)
			{
				return c.HasAny(flags);
			}
		}
	}
}
