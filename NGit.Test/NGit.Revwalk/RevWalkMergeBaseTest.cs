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
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Revwalk
{
	public class RevWalkMergeBaseTest : RevWalkTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestNone()
		{
			RevCommit c1 = Commit(Commit(Commit()));
			RevCommit c2 = Commit(Commit(Commit()));
			rw.SetRevFilter(RevFilter.MERGE_BASE);
			MarkStart(c1);
			MarkStart(c2);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDisallowTreeFilter()
		{
			RevCommit c1 = Commit();
			RevCommit c2 = Commit();
			rw.SetRevFilter(RevFilter.MERGE_BASE);
			rw.SetTreeFilter(TreeFilter.ANY_DIFF);
			MarkStart(c1);
			MarkStart(c2);
			try
			{
				NUnit.Framework.Assert.IsNull(rw.Next());
				NUnit.Framework.Assert.Fail("did not throw IllegalStateException");
			}
			catch (InvalidOperationException)
			{
			}
		}

		// expected result
		/// <exception cref="System.Exception"></exception>
		public virtual void TestSimple()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c1 = Commit(Commit(Commit(Commit(Commit(b)))));
			RevCommit c2 = Commit(Commit(Commit(Commit(Commit(b)))));
			rw.SetRevFilter(RevFilter.MERGE_BASE);
			MarkStart(c1);
			MarkStart(c2);
			AssertCommit(b, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMultipleHeads_SameBase1()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c1 = Commit(Commit(Commit(Commit(Commit(b)))));
			RevCommit c2 = Commit(Commit(Commit(Commit(Commit(b)))));
			RevCommit c3 = Commit(Commit(Commit(b)));
			rw.SetRevFilter(RevFilter.MERGE_BASE);
			MarkStart(c1);
			MarkStart(c2);
			MarkStart(c3);
			AssertCommit(b, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMultipleHeads_SameBase2()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			RevCommit d1 = Commit(Commit(Commit(Commit(Commit(b)))));
			RevCommit d2 = Commit(Commit(Commit(Commit(Commit(c)))));
			RevCommit d3 = Commit(Commit(Commit(c)));
			rw.SetRevFilter(RevFilter.MERGE_BASE);
			MarkStart(d1);
			MarkStart(d2);
			MarkStart(d3);
			AssertCommit(b, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCrissCross()
		{
			// See http://marc.info/?l=git&m=111463358500362&w=2 for a nice
			// description of what this test is creating. We don't have a
			// clean merge base for d,e as they each merged the parents b,c
			// in different orders.
			//
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(a);
			RevCommit d = Commit(b, c);
			RevCommit e = Commit(c, b);
			rw.SetRevFilter(RevFilter.MERGE_BASE);
			MarkStart(d);
			MarkStart(e);
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}
	}
}
