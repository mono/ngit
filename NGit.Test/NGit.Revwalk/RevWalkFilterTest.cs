using System;
using NGit.Revwalk;
using NGit.Revwalk.Filter;
using Sharpen;

namespace NGit.Revwalk
{
	public class RevWalkFilterTest : RevWalkTestCase
	{
		private static readonly RevWalkFilterTest.MyAll MY_ALL = new RevWalkFilterTest.MyAll
			();

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_ALL()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(RevFilter.ALL);
			MarkStart(c);
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_Negate_ALL()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(RevFilter.ALL.Negate());
			MarkStart(c);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_NOT_ALL()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(NotRevFilter.Create(RevFilter.ALL));
			MarkStart(c);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_NONE()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(RevFilter.NONE);
			MarkStart(c);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_NOT_NONE()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(NotRevFilter.Create(RevFilter.NONE));
			MarkStart(c);
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_ALL_And_NONE()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(AndRevFilter.Create(RevFilter.ALL, RevFilter.NONE));
			MarkStart(c);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_NONE_And_ALL()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(AndRevFilter.Create(RevFilter.NONE, RevFilter.ALL));
			MarkStart(c);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_ALL_Or_NONE()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(OrRevFilter.Create(RevFilter.ALL, RevFilter.NONE));
			MarkStart(c);
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_NONE_Or_ALL()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(OrRevFilter.Create(RevFilter.NONE, RevFilter.ALL));
			MarkStart(c);
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_MY_ALL_And_NONE()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(AndRevFilter.Create(MY_ALL, RevFilter.NONE));
			MarkStart(c);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_NONE_And_MY_ALL()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(AndRevFilter.Create(RevFilter.NONE, MY_ALL));
			MarkStart(c);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_MY_ALL_Or_NONE()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(OrRevFilter.Create(MY_ALL, RevFilter.NONE));
			MarkStart(c);
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_NONE_Or_MY_ALL()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			rw.SetRevFilter(OrRevFilter.Create(RevFilter.NONE, MY_ALL));
			MarkStart(c);
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilter_NO_MERGES()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c1 = Commit(b);
			RevCommit c2 = Commit(b);
			RevCommit d = Commit(c1, c2);
			RevCommit e = Commit(d);
			rw.SetRevFilter(RevFilter.NO_MERGES);
			MarkStart(e);
			AssertCommit(e, rw.Next());
			AssertCommit(c2, rw.Next());
			AssertCommit(c1, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCommitTimeRevFilter()
		{
			RevCommit a = Commit();
			Tick(100);
			RevCommit b = Commit(a);
			Tick(100);
			DateTime since = GetClock();
			RevCommit c1 = Commit(b);
			Tick(100);
			RevCommit c2 = Commit(b);
			Tick(100);
			DateTime until = GetClock();
			RevCommit d = Commit(c1, c2);
			Tick(100);
			RevCommit e = Commit(d);
			{
				RevFilter after = NGit.Revwalk.Filter.CommitTimeRevFilter.AfterFilter(since);
				NUnit.Framework.Assert.IsNotNull(after);
				rw.SetRevFilter(after);
				MarkStart(e);
				AssertCommit(e, rw.Next());
				AssertCommit(d, rw.Next());
				AssertCommit(c2, rw.Next());
				AssertCommit(c1, rw.Next());
				NUnit.Framework.Assert.IsNull(rw.Next());
			}
			{
				RevFilter before = NGit.Revwalk.Filter.CommitTimeRevFilter.BeforeFilter(until);
				NUnit.Framework.Assert.IsNotNull(before);
				rw.Reset();
				rw.SetRevFilter(before);
				MarkStart(e);
				AssertCommit(c2, rw.Next());
				AssertCommit(c1, rw.Next());
				AssertCommit(b, rw.Next());
				AssertCommit(a, rw.Next());
				NUnit.Framework.Assert.IsNull(rw.Next());
			}
			{
				RevFilter between = NGit.Revwalk.Filter.CommitTimeRevFilter.BetweenFilter(since, 
					until);
				NUnit.Framework.Assert.IsNotNull(between);
				rw.Reset();
				rw.SetRevFilter(between);
				MarkStart(e);
				AssertCommit(c2, rw.Next());
				AssertCommit(c1, rw.Next());
				NUnit.Framework.Assert.IsNull(rw.Next());
			}
		}

		private class MyAll : RevFilter
		{
			public override RevFilter Clone()
			{
				return this;
			}

			/// <exception cref="NGit.Errors.StopWalkException"></exception>
			/// <exception cref="NGit.Errors.MissingObjectException"></exception>
			/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override bool Include(RevWalk walker, RevCommit cmit)
			{
				return true;
			}
		}
	}
}
