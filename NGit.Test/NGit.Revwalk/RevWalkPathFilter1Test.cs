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
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Revwalk
{
	public class RevWalkPathFilter1Test : RevWalkTestCase
	{
		protected internal virtual void Filter(string path)
		{
			rw.SetTreeFilter(AndTreeFilter.Create(PathFilterGroup.CreateFromStrings(Collections
				.Singleton(path)), TreeFilter.ANY_DIFF));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmpty_EmptyTree()
		{
			RevCommit a = Commit();
			Filter("a");
			MarkStart(a);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmpty_NoMatch()
		{
			RevCommit a = Commit(Tree(File("0", Blob("0"))));
			Filter("a");
			MarkStart(a);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSimple1()
		{
			RevCommit a = Commit(Tree(File("0", Blob("0"))));
			Filter("0");
			MarkStart(a);
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEdits_MatchNone()
		{
			RevCommit a = Commit(Tree(File("0", Blob("a"))));
			RevCommit b = Commit(Tree(File("0", Blob("b"))), a);
			RevCommit c = Commit(Tree(File("0", Blob("c"))), b);
			RevCommit d = Commit(Tree(File("0", Blob("d"))), c);
			Filter("a");
			MarkStart(d);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEdits_MatchAll()
		{
			RevCommit a = Commit(Tree(File("0", Blob("a"))));
			RevCommit b = Commit(Tree(File("0", Blob("b"))), a);
			RevCommit c = Commit(Tree(File("0", Blob("c"))), b);
			RevCommit d = Commit(Tree(File("0", Blob("d"))), c);
			Filter("0");
			MarkStart(d);
			AssertCommit(d, rw.Next());
			AssertCommit(c, rw.Next());
			AssertCommit(b, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStringOfPearls_FilePath1()
		{
			RevCommit a = Commit(Tree(File("d/f", Blob("a"))));
			RevCommit b = Commit(Tree(File("d/f", Blob("a"))), a);
			RevCommit c = Commit(Tree(File("d/f", Blob("b"))), b);
			Filter("d/f");
			MarkStart(c);
			AssertCommit(c, rw.Next());
			NUnit.Framework.Assert.AreEqual(1, c.ParentCount);
			AssertCommit(a, c.GetParent(0));
			// b was skipped
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.AreEqual(0, a.ParentCount);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStringOfPearls_FilePath2()
		{
			RevCommit a = Commit(Tree(File("d/f", Blob("a"))));
			RevCommit b = Commit(Tree(File("d/f", Blob("a"))), a);
			RevCommit c = Commit(Tree(File("d/f", Blob("b"))), b);
			RevCommit d = Commit(Tree(File("d/f", Blob("b"))), c);
			Filter("d/f");
			MarkStart(d);
			// d was skipped
			AssertCommit(c, rw.Next());
			NUnit.Framework.Assert.AreEqual(1, c.ParentCount);
			AssertCommit(a, c.GetParent(0));
			// b was skipped
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.AreEqual(0, a.ParentCount);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStringOfPearls_DirPath2()
		{
			RevCommit a = Commit(Tree(File("d/f", Blob("a"))));
			RevCommit b = Commit(Tree(File("d/f", Blob("a"))), a);
			RevCommit c = Commit(Tree(File("d/f", Blob("b"))), b);
			RevCommit d = Commit(Tree(File("d/f", Blob("b"))), c);
			Filter("d");
			MarkStart(d);
			// d was skipped
			AssertCommit(c, rw.Next());
			NUnit.Framework.Assert.AreEqual(1, c.ParentCount);
			AssertCommit(a, c.GetParent(0));
			// b was skipped
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.AreEqual(0, a.ParentCount);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStringOfPearls_FilePath3()
		{
			RevCommit a = Commit(Tree(File("d/f", Blob("a"))));
			RevCommit b = Commit(Tree(File("d/f", Blob("a"))), a);
			RevCommit c = Commit(Tree(File("d/f", Blob("b"))), b);
			RevCommit d = Commit(Tree(File("d/f", Blob("b"))), c);
			RevCommit e = Commit(Tree(File("d/f", Blob("b"))), d);
			RevCommit f = Commit(Tree(File("d/f", Blob("b"))), e);
			RevCommit g = Commit(Tree(File("d/f", Blob("b"))), f);
			RevCommit h = Commit(Tree(File("d/f", Blob("b"))), g);
			RevCommit i = Commit(Tree(File("d/f", Blob("c"))), h);
			Filter("d/f");
			MarkStart(i);
			AssertCommit(i, rw.Next());
			NUnit.Framework.Assert.AreEqual(1, i.ParentCount);
			AssertCommit(c, i.GetParent(0));
			// h..d was skipped
			AssertCommit(c, rw.Next());
			NUnit.Framework.Assert.AreEqual(1, c.ParentCount);
			AssertCommit(a, c.GetParent(0));
			// b was skipped
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.AreEqual(0, a.ParentCount);
			NUnit.Framework.Assert.IsNull(rw.Next());
		}
	}
}
