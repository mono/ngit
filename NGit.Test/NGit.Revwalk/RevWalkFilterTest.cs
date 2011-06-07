/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using NGit.Revwalk;
using NGit.Revwalk.Filter;
using Sharpen;

namespace NGit.Revwalk
{
	[NUnit.Framework.TestFixture]
	public class RevWalkFilterTest : RevWalkTestCase
	{
		private static readonly RevWalkFilterTest.MyAll MY_ALL = new RevWalkFilterTest.MyAll
			();

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
		[NUnit.Framework.Test]
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
				RevFilter after = CommitTimeRevFilter.After(since);
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
				RevFilter before = CommitTimeRevFilter.Before(until);
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
				RevFilter between = CommitTimeRevFilter.Between(since, until);
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

			public override bool RequiresCommitBody()
			{
				return false;
			}
		}
	}
}
