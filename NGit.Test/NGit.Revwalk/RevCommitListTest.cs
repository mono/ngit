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

using NGit;
using NGit.Api;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	[NUnit.Framework.TestFixture]
	public class RevCommitListTest : RepositoryTestCase
	{
		private RevCommitList<RevCommit> list;

		/// <exception cref="System.Exception"></exception>
		public virtual void Setup(int count)
		{
			Git git = new Git(db);
			for (int i = 0; i < count; i++)
			{
				git.Commit().SetCommitter(committer).SetAuthor(author).SetMessage("commit " + i).
					Call();
			}
			list = new RevCommitList<RevCommit>();
			RevWalk w = new RevWalk(db);
			w.MarkStart(w.LookupCommit(db.Resolve(Constants.HEAD)));
			list.Source(w);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFillToHighMark2()
		{
			Setup(3);
			list.FillTo(1);
			NUnit.Framework.Assert.AreEqual(2, list.Count);
			NUnit.Framework.Assert.AreEqual("commit 2", list[0].GetFullMessage());
			NUnit.Framework.Assert.AreEqual("commit 1", list[1].GetFullMessage());
			NUnit.Framework.Assert.IsNull(list[2], "commit 0 shouldn't be loaded");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFillToHighMarkAll()
		{
			Setup(3);
			list.FillTo(2);
			NUnit.Framework.Assert.AreEqual(3, list.Count);
			NUnit.Framework.Assert.AreEqual("commit 2", list[0].GetFullMessage());
			NUnit.Framework.Assert.AreEqual("commit 0", list[2].GetFullMessage());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFillToHighMark4()
		{
			Setup(3);
			list.FillTo(3);
			NUnit.Framework.Assert.AreEqual(3, list.Count);
			NUnit.Framework.Assert.AreEqual("commit 2", list[0].GetFullMessage());
			NUnit.Framework.Assert.AreEqual("commit 0", list[2].GetFullMessage());
			NUnit.Framework.Assert.IsNull(list[3], "commit 3 can't be loaded");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFillToHighMarkMulitpleBlocks()
		{
			Setup(258);
			list.FillTo(257);
			NUnit.Framework.Assert.AreEqual(258, list.Count);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFillToCommit()
		{
			Setup(3);
			RevWalk w = new RevWalk(db);
			w.MarkStart(w.LookupCommit(db.Resolve(Constants.HEAD)));
			w.Next();
			RevCommit c = w.Next();
			NUnit.Framework.Assert.IsNotNull(c, "should have found 2. commit");
			list.FillTo(c, 5);
			NUnit.Framework.Assert.AreEqual(2, list.Count);
			NUnit.Framework.Assert.AreEqual("commit 1", list[1].GetFullMessage());
			NUnit.Framework.Assert.IsNull(list[3]);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFillToUnknownCommit()
		{
			Setup(258);
			RevCommit c = new RevCommit(ObjectId.FromString("9473095c4cb2f12aefe1db8a355fe3fafba42f67"
				));
			list.FillTo(c, 300);
			NUnit.Framework.Assert.AreEqual(258, list.Count, "loading to unknown commit should load all commits"
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFillToNullCommit()
		{
			Setup(3);
			list.FillTo(null, 1);
			NUnit.Framework.Assert.IsNull(list[0]);
		}
	}
}
