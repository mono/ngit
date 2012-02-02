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
	[NUnit.Framework.TestFixture]
	public class RevWalkUtilsCountTest : RevWalkTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldWorkForNormalCase()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			NUnit.Framework.Assert.AreEqual(1, Count(b, a));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldReturnZeroOnSameCommit()
		{
			RevCommit c1 = Commit(Commit(Commit()));
			NUnit.Framework.Assert.AreEqual(0, Count(c1, c1));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldReturnZeroWhenMergedInto()
		{
			RevCommit a = Commit();
			RevCommit b = Commit(a);
			NUnit.Framework.Assert.AreEqual(0, Count(a, b));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldWorkWithMerges()
		{
			RevCommit a = Commit();
			RevCommit b1 = Commit(a);
			RevCommit b2 = Commit(a);
			RevCommit c = Commit(b1, b2);
			NUnit.Framework.Assert.AreEqual(3, Count(c, a));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldWorkWithoutCommonAncestor()
		{
			RevCommit a1 = Commit();
			RevCommit a2 = Commit();
			RevCommit b = Commit(a1);
			NUnit.Framework.Assert.AreEqual(2, Count(b, a2));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldWorkWithZeroAsEnd()
		{
			RevCommit c = Commit(Commit());
			NUnit.Framework.Assert.AreEqual(2, Count(c, null));
		}

		/// <exception cref="System.Exception"></exception>
		private int Count(RevCommit start, RevCommit end)
		{
			return RevWalkUtils.Count(rw, start, end);
		}
	}
}
