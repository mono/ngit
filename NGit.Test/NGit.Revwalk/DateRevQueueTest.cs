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
	public class DateRevQueueTest : RevQueueTestCase<DateRevQueue>
	{
		protected internal override DateRevQueue Create()
		{
			return new DateRevQueue();
		}

		/// <exception cref="System.Exception"></exception>
		public override void TestEmpty()
		{
			base.TestEmpty();
			NUnit.Framework.Assert.IsNull(q.Peek());
			NUnit.Framework.Assert.AreEqual(Generator.SORT_COMMIT_TIME_DESC, q.OutputType());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCloneEmpty()
		{
			q = new DateRevQueue(AbstractRevQueue.EMPTY_QUEUE);
			NUnit.Framework.Assert.IsNull(q.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInsertOutOfOrder()
		{
			RevCommit a = ParseBody(Commit());
			RevCommit b = ParseBody(Commit(10, a));
			RevCommit c1 = ParseBody(Commit(5, b));
			RevCommit c2 = ParseBody(Commit(-50, b));
			q.Add(c2);
			q.Add(a);
			q.Add(b);
			q.Add(c1);
			AssertCommit(c1, q.Next());
			AssertCommit(b, q.Next());
			AssertCommit(a, q.Next());
			AssertCommit(c2, q.Next());
			NUnit.Framework.Assert.IsNull(q.Next());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInsertTie()
		{
			RevCommit a = ParseBody(Commit());
			RevCommit b = ParseBody(Commit(0, a));
			{
				q = Create();
				q.Add(a);
				q.Add(b);
				AssertCommit(a, q.Next());
				AssertCommit(b, q.Next());
				NUnit.Framework.Assert.IsNull(q.Next());
			}
			{
				q = Create();
				q.Add(b);
				q.Add(a);
				AssertCommit(b, q.Next());
				AssertCommit(a, q.Next());
				NUnit.Framework.Assert.IsNull(q.Next());
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCloneFIFO()
		{
			RevCommit a = ParseBody(Commit());
			RevCommit b = ParseBody(Commit(200, a));
			RevCommit c = ParseBody(Commit(200, b));
			FIFORevQueue src = new FIFORevQueue();
			src.Add(a);
			src.Add(b);
			src.Add(c);
			q = new DateRevQueue(src);
			NUnit.Framework.Assert.IsFalse(q.EverbodyHasFlag(RevWalk.UNINTERESTING));
			NUnit.Framework.Assert.IsFalse(q.AnybodyHasFlag(RevWalk.UNINTERESTING));
			AssertCommit(c, q.Peek());
			AssertCommit(c, q.Peek());
			AssertCommit(c, q.Next());
			AssertCommit(b, q.Next());
			AssertCommit(a, q.Next());
			NUnit.Framework.Assert.IsNull(q.Next());
		}
	}
}
