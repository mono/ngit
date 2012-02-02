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

using System.Collections.Generic;
using NGit.Diff;
using NGit.Junit;
using NGit.Revwalk;
using NUnit.Framework;
using Sharpen;

namespace NGit.Revwalk
{
	[NUnit.Framework.TestFixture]
	public class RevWalkFollowFilterTest : RevWalkTestCase
	{
		private class DiffCollector : RenameCallback
		{
			internal IList<DiffEntry> diffs = new AList<DiffEntry>();

			public override void Renamed(DiffEntry diff)
			{
				diffs.AddItem(diff);
			}
		}

		private RevWalkFollowFilterTest.DiffCollector diffCollector;

		/// <exception cref="System.Exception"></exception>
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			diffCollector = new RevWalkFollowFilterTest.DiffCollector();
		}

		protected internal virtual FollowFilter Follow(string followPath)
		{
			FollowFilter followFilter = FollowFilter.Create(followPath);
			followFilter.SetRenameCallback(diffCollector);
			rw.SetTreeFilter(followFilter);
			return followFilter;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNoRename()
		{
			RevCommit a = Commit(Tree(File("0", Blob("0"))));
			Follow("0");
			MarkStart(a);
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
			AssertNoRenames();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSingleRename()
		{
			RevCommit a = Commit(Tree(File("a", Blob("A"))));
			// rename a to b
			NGit.Junit.CommitBuilder commitBuilder = CommitBuilder().Parent(a).Add("b", Blob
				("A")).Rm("a");
			RevCommit renameCommit = commitBuilder.Create();
			Follow("b");
			MarkStart(renameCommit);
			AssertCommit(renameCommit, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
			AssertRenames("a->b");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMultiRename()
		{
			string contents = "A";
			RevCommit a = Commit(Tree(File("a", Blob(contents))));
			// rename a to b
			NGit.Junit.CommitBuilder commitBuilder = CommitBuilder().Parent(a).Add("b", Blob
				(contents)).Rm("a");
			RevCommit renameCommit1 = commitBuilder.Create();
			// rename b to c
			commitBuilder = CommitBuilder().Parent(renameCommit1).Add("c", Blob(contents)).Rm
				("b");
			RevCommit renameCommit2 = commitBuilder.Create();
			// rename c to a
			commitBuilder = CommitBuilder().Parent(renameCommit2).Add("a", Blob(contents)).Rm
				("c");
			RevCommit renameCommit3 = commitBuilder.Create();
			Follow("a");
			MarkStart(renameCommit3);
			AssertCommit(renameCommit3, rw.Next());
			AssertCommit(renameCommit2, rw.Next());
			AssertCommit(renameCommit1, rw.Next());
			AssertCommit(a, rw.Next());
			NUnit.Framework.Assert.IsNull(rw.Next());
			AssertRenames("c->a", "b->c", "a->b");
		}

		/// <summary>Assert which renames should have happened, in traversal order.</summary>
		/// <remarks>Assert which renames should have happened, in traversal order.</remarks>
		/// <param name="expectedRenames">the rename specs, each one in the form "srcPath-&gt;destPath"
		/// 	</param>
		protected internal virtual void AssertRenames(params string[] expectedRenames)
		{
			NUnit.Framework.Assert.AreEqual(expectedRenames.Length, diffCollector.diffs.Count
				, "Unexpected number of renames. Expected: " + expectedRenames.Length + ", actual: "
				 + diffCollector.diffs.Count);
			for (int i = 0; i < expectedRenames.Length; i++)
			{
				DiffEntry diff = diffCollector.diffs[i];
				NUnit.Framework.Assert.IsNotNull(diff);
				string[] split = expectedRenames[i].Split("->");
				NUnit.Framework.Assert.IsNotNull(split);
				NUnit.Framework.Assert.AreEqual(2, split.Length);
				string src = split[0];
				string target = split[1];
				NUnit.Framework.Assert.AreEqual(src, diff.GetOldPath());
				NUnit.Framework.Assert.AreEqual(target, diff.GetNewPath());
			}
		}

		protected internal virtual void AssertNoRenames()
		{
			NUnit.Framework.Assert.AreEqual(0, diffCollector.diffs.Count, "Found unexpected rename/copy diff"
				);
		}
	}
}
