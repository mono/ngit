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
using NGit.Revwalk.Filter;
using NUnit.Framework;
using Sharpen;

namespace NGit.Revwalk
{
	[NUnit.Framework.TestFixture]
	public class RevWalkResetTest : RevWalkTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRevFilterReceivesParsedCommits()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			RevCommit c = Commit(b);
			AtomicBoolean filterRan = new AtomicBoolean();
			RevFilter testFilter = new _RevFilter_68(filterRan);
			// Do an initial run through the walk
			filterRan.Set(false);
			rw.SetRevFilter(testFilter);
			MarkStart(c);
			rw.MarkUninteresting(b);
			for (RevCommit cmit = rw.Next(); cmit != null; cmit = rw.Next())
			{
			}
			// Don't dispose the body here, because we want to test the effect
			// of marking 'b' as uninteresting.
			NUnit.Framework.Assert.IsTrue(filterRan.Get(), "filter ran");
			// Run through the walk again, this time disposing of all commits.
			filterRan.Set(false);
			rw.Reset();
			MarkStart(c);
			for (RevCommit cmit_1 = rw.Next(); cmit_1 != null; cmit_1 = rw.Next())
			{
				cmit_1.DisposeBody();
			}
			NUnit.Framework.Assert.IsTrue(filterRan.Get(), "filter ran");
			// Do the third run through the reused walk. Test that the explicitly
			// disposed commits are parsed on this walk.
			filterRan.Set(false);
			rw.Reset();
			MarkStart(c);
			for (RevCommit cmit_2 = rw.Next(); cmit_2 != null; cmit_2 = rw.Next())
			{
			}
			// spin through the walk.
			NUnit.Framework.Assert.IsTrue(filterRan.Get(), "filter ran");
		}

		private sealed class _RevFilter_68 : RevFilter
		{
			public _RevFilter_68(AtomicBoolean filterRan)
			{
				this.filterRan = filterRan;
			}

			/// <exception cref="NGit.Errors.StopWalkException"></exception>
			/// <exception cref="NGit.Errors.MissingObjectException"></exception>
			/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override bool Include(RevWalk walker, RevCommit cmit)
			{
				NUnit.Framework.Assert.IsNotNull(cmit.RawBuffer, "commit is parsed");
				filterRan.Set(true);
				return true;
			}

			public override RevFilter Clone()
			{
				return this;
			}

			public override bool RequiresCommitBody()
			{
				return true;
			}

			private readonly AtomicBoolean filterRan;
		}
	}
}
