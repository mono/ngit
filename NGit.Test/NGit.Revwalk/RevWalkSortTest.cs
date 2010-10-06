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

using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public class RevWalkSortTest : RevWalkTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_Default()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(1, a);
			RevCommit c = Commit(1, b);
			RevCommit d = Commit(1, c);
			MarkStart(d);
			AssertCommit(d, rw.Next());
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_COMMIT_TIME_DESC()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			RevCommit d = Commit(c);
			rw.Sort(RevSort.COMMIT_TIME_DESC);
			MarkStart(d);
			AssertCommit(d, rw.Next());
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_REVERSE()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			RevCommit d = Commit(c);
			rw.Sort(RevSort.REVERSE);
			MarkStart(d);
			AssertCommit(a, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(c, rw.Next());
			AssertCommit(d, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_COMMIT_TIME_DESC_OutOfOrder1()
		{
			// Despite being out of order time-wise, a strand-of-pearls must
			// still maintain topological order.
			//
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(-5, b);
			RevCommit d = Commit(10, c);
			NUnit.Framework.Assert.IsTrue(ParseBody(a).CommitTime < ParseBody(d).CommitTime);
			NUnit.Framework.Assert.IsTrue(ParseBody(c).CommitTime < ParseBody(b).CommitTime);
			rw.Sort(RevSort.COMMIT_TIME_DESC);
			MarkStart(d);
			AssertCommit(d, rw.Next());
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_COMMIT_TIME_DESC_OutOfOrder2()
		{
			// c1 is back dated before its parent.
			//
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c1 = Commit(-5, b);
			RevCommit c2 = Commit(10, b);
			RevCommit d = Commit(c1, c2);
			rw.Sort(RevSort.COMMIT_TIME_DESC);
			MarkStart(d);
			AssertCommit(d, rw.Next());
			AssertCommit(c2, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			AssertCommit(c1, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_TOPO()
		{
			// c1 is back dated before its parent.
			//
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c1 = Commit(-5, b);
			RevCommit c2 = Commit(10, b);
			RevCommit d = Commit(c1, c2);
			rw.Sort(RevSort.TOPO);
			MarkStart(d);
			AssertCommit(d, rw.Next());
			AssertCommit(c2, rw.Next());
			AssertCommit(c1, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSort_TOPO_REVERSE()
		{
			// c1 is back dated before its parent.
			//
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c1 = Commit(-5, b);
			RevCommit c2 = Commit(10, b);
			RevCommit d = Commit(c1, c2);
			rw.Sort(RevSort.TOPO);
			rw.Sort(RevSort.REVERSE, true);
			MarkStart(d);
			AssertCommit(a, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(c1, rw.Next());
			AssertCommit(c2, rw.Next());
			AssertCommit(d, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}
	}
}
