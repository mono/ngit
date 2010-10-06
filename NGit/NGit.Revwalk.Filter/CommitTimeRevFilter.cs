using System;
using NGit.Errors;
using NGit.Revwalk;
using NGit.Revwalk.Filter;
using Sharpen;

namespace NGit.Revwalk.Filter
{
	/// <summary>Selects commits based upon the commit time field.</summary>
	/// <remarks>Selects commits based upon the commit time field.</remarks>
	public abstract class CommitTimeRevFilter : RevFilter
	{
		/// <summary>Create a new filter to select commits before a given date/time.</summary>
		/// <remarks>Create a new filter to select commits before a given date/time.</remarks>
		/// <param name="ts">the point in time to cut on.</param>
		/// <returns>a new filter to select commits on or before <code>ts</code>.</returns>
		public static RevFilter BeforeFilter(DateTime ts)
		{
			return new CommitTimeRevFilter.Before(ts.GetTime());
		}

		/// <summary>Create a new filter to select commits after a given date/time.</summary>
		/// <remarks>Create a new filter to select commits after a given date/time.</remarks>
		/// <param name="ts">the point in time to cut on.</param>
		/// <returns>a new filter to select commits on or after <code>ts</code>.</returns>
		public static RevFilter AfterFilter(DateTime ts)
		{
			return new CommitTimeRevFilter.After(ts.GetTime());
		}

		/// <summary>
		/// Create a new filter to select commits after or equal a given date/time <code>since</code>
		/// and before or equal a given date/time <code>until</code>.
		/// </summary>
		/// <remarks>
		/// Create a new filter to select commits after or equal a given date/time <code>since</code>
		/// and before or equal a given date/time <code>until</code>.
		/// </remarks>
		/// <param name="since">the point in time to cut on.</param>
		/// <param name="until">the point in time to cut off.</param>
		/// <returns>a new filter to select commits between the given date/times.</returns>
		public static RevFilter BetweenFilter(DateTime since, DateTime until)
		{
			return new CommitTimeRevFilter.Between(since.GetTime(), until.GetTime());
		}

		internal readonly int when;

		internal CommitTimeRevFilter(long ts)
		{
			when = (int)(ts / 1000);
		}

		public override RevFilter Clone()
		{
			return this;
		}

		private class Before : CommitTimeRevFilter
		{
			internal Before(long ts) : base(ts)
			{
			}

			/// <exception cref="NGit.Errors.StopWalkException"></exception>
			/// <exception cref="NGit.Errors.MissingObjectException"></exception>
			/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override bool Include(RevWalk walker, RevCommit cmit)
			{
				return cmit.CommitTime <= when;
			}

			public override string ToString()
			{
				return base.ToString() + "(" + Sharpen.Extensions.CreateDate(when * 1000L) + ")";
			}
		}

		private class After : CommitTimeRevFilter
		{
			internal After(long ts) : base(ts)
			{
			}

			/// <exception cref="NGit.Errors.StopWalkException"></exception>
			/// <exception cref="NGit.Errors.MissingObjectException"></exception>
			/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override bool Include(RevWalk walker, RevCommit cmit)
			{
				// Since the walker sorts commits by commit time we can be
				// reasonably certain there is nothing remaining worth our
				// scanning if this commit is before the point in question.
				//
				if (cmit.CommitTime < when)
				{
					throw StopWalkException.INSTANCE;
				}
				return true;
			}

			public override string ToString()
			{
				return base.ToString() + "(" + Sharpen.Extensions.CreateDate(when * 1000L) + ")";
			}
		}

		private class Between : CommitTimeRevFilter
		{
			private readonly int until;

			internal Between(long since, long until) : base(since)
			{
				this.until = (int)(until / 1000);
			}

			/// <exception cref="NGit.Errors.StopWalkException"></exception>
			/// <exception cref="NGit.Errors.MissingObjectException"></exception>
			/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override bool Include(RevWalk walker, RevCommit cmit)
			{
				return cmit.CommitTime <= until && cmit.CommitTime >= when;
			}

			public override string ToString()
			{
				return base.ToString() + "(" + Sharpen.Extensions.CreateDate(when * 1000L) + " - "
					 + Sharpen.Extensions.CreateDate(until * 1000L) + ")";
			}
		}
	}
}
