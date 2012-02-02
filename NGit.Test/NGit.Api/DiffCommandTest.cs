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
using System.Collections.Generic;
using NGit;
using NGit.Api;
using NGit.Diff;
using NGit.Revwalk;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class DiffCommandTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDiffModified()
		{
			Write(new FilePath(db.WorkTree, "test.txt"), "test");
			FilePath folder = new FilePath(db.WorkTree, "folder");
			folder.Mkdir();
			Write(new FilePath(folder, "folder.txt"), "folder");
			Git git = new Git(db);
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("Initial commit").Call();
			Write(new FilePath(folder, "folder.txt"), "folder change");
			OutputStream @out = new ByteArrayOutputStream();
			IList<DiffEntry> entries = git.Diff().SetOutputStream(@out).Call();
			NUnit.Framework.Assert.AreEqual(1, entries.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, entries[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("folder/folder.txt", entries[0].GetOldPath());
			NUnit.Framework.Assert.AreEqual("folder/folder.txt", entries[0].GetNewPath());
			string actual = @out.ToString();
			string expected = "diff --git a/folder/folder.txt b/folder/folder.txt\n" + "index 0119635..95c4c65 100644\n"
				 + "--- a/folder/folder.txt\n" + "+++ b/folder/folder.txt\n" + "@@ -1 +1 @@\n" +
				 "-folder\n" + "\\ No newline at end of file\n" + "+folder change\n" + "\\ No newline at end of file\n";
			NUnit.Framework.Assert.AreEqual(expected.ToString(), actual);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDiffCached()
		{
			Write(new FilePath(db.WorkTree, "test.txt"), "test");
			FilePath folder = new FilePath(db.WorkTree, "folder");
			folder.Mkdir();
			Git git = new Git(db);
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("Initial commit").Call();
			Write(new FilePath(folder, "folder.txt"), "folder");
			git.Add().AddFilepattern(".").Call();
			OutputStream @out = new ByteArrayOutputStream();
			IList<DiffEntry> entries = git.Diff().SetOutputStream(@out).SetCached(true).Call(
				);
			NUnit.Framework.Assert.AreEqual(1, entries.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.ADD, entries[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("/dev/null", entries[0].GetOldPath());
			NUnit.Framework.Assert.AreEqual("folder/folder.txt", entries[0].GetNewPath());
			string actual = @out.ToString();
			string expected = "diff --git a/folder/folder.txt b/folder/folder.txt\n" + "new file mode 100644\n"
				 + "index 0000000..0119635\n" + "--- /dev/null\n" + "+++ b/folder/folder.txt\n" 
				+ "@@ -0,0 +1 @@\n" + "+folder\n" + "\\ No newline at end of file\n";
			NUnit.Framework.Assert.AreEqual(expected.ToString(), actual);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDiffTwoCommits()
		{
			Write(new FilePath(db.WorkTree, "test.txt"), "test");
			FilePath folder = new FilePath(db.WorkTree, "folder");
			folder.Mkdir();
			Write(new FilePath(folder, "folder.txt"), "folder");
			Git git = new Git(db);
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("Initial commit").Call();
			Write(new FilePath(folder, "folder.txt"), "folder change");
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("second commit").Call();
			Write(new FilePath(folder, "folder.txt"), "second folder change");
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("third commit").Call();
			// bad filter
			DiffCommand diff = git.Diff().SetShowNameAndStatusOnly(true).SetPathFilter(PathFilter
				.Create("test.txt")).SetOldTree(GetTreeIterator("HEAD^^")).SetNewTree(GetTreeIterator
				("HEAD^"));
			IList<DiffEntry> entries = diff.Call();
			NUnit.Framework.Assert.AreEqual(0, entries.Count);
			// no filter, two commits
			OutputStream @out = new ByteArrayOutputStream();
			diff = git.Diff().SetOutputStream(@out).SetOldTree(GetTreeIterator("HEAD^^")).SetNewTree
				(GetTreeIterator("HEAD^"));
			entries = diff.Call();
			NUnit.Framework.Assert.AreEqual(1, entries.Count);
			NUnit.Framework.Assert.AreEqual(DiffEntry.ChangeType.MODIFY, entries[0].GetChangeType
				());
			NUnit.Framework.Assert.AreEqual("folder/folder.txt", entries[0].GetOldPath());
			NUnit.Framework.Assert.AreEqual("folder/folder.txt", entries[0].GetNewPath());
			string actual = @out.ToString();
			string expected = "diff --git a/folder/folder.txt b/folder/folder.txt\n" + "index 0119635..95c4c65 100644\n"
				 + "--- a/folder/folder.txt\n" + "+++ b/folder/folder.txt\n" + "@@ -1 +1 @@\n" +
				 "-folder\n" + "\\ No newline at end of file\n" + "+folder change\n" + "\\ No newline at end of file\n";
			NUnit.Framework.Assert.AreEqual(expected.ToString(), actual);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDiffWithPrefixes()
		{
			Write(new FilePath(db.WorkTree, "test.txt"), "test");
			Git git = new Git(db);
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("Initial commit").Call();
			Write(new FilePath(db.WorkTree, "test.txt"), "test change");
			OutputStream @out = new ByteArrayOutputStream();
			git.Diff().SetOutputStream(@out).SetSourcePrefix("old/").SetDestinationPrefix("new/"
				).Call();
			string actual = @out.ToString();
			string expected = "diff --git old/test.txt new/test.txt\n" + "index 30d74d2..4dba797 100644\n"
				 + "--- old/test.txt\n" + "+++ new/test.txt\n" + "@@ -1 +1 @@\n" + "-test\n" + "\\ No newline at end of file\n"
				 + "+test change\n" + "\\ No newline at end of file\n";
			NUnit.Framework.Assert.AreEqual(expected.ToString(), actual);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDiffWithNegativeLineCount()
		{
			Write(new FilePath(db.WorkTree, "test.txt"), "0\n1\n2\n3\n4\n5\n6\n7\n8\n9");
			Git git = new Git(db);
			git.Add().AddFilepattern(".").Call();
			git.Commit().SetMessage("Initial commit").Call();
			Write(new FilePath(db.WorkTree, "test.txt"), "0\n1\n2\n3\n4a\n5\n6\n7\n8\n9");
			OutputStream @out = new ByteArrayOutputStream();
			git.Diff().SetOutputStream(@out).SetContextLines(1).Call();
			string actual = @out.ToString();
			string expected = "diff --git a/test.txt b/test.txt\n" + "index f55b5c9..c5ec8fd 100644\n"
				 + "--- a/test.txt\n" + "+++ b/test.txt\n" + "@@ -4,3 +4,3 @@\n" + " 3\n" + "-4\n"
				 + "+4a\n" + " 5\n";
			NUnit.Framework.Assert.AreEqual(expected.ToString(), actual);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private AbstractTreeIterator GetTreeIterator(string name)
		{
			ObjectId id = db.Resolve(name);
			if (id == null)
			{
				throw new ArgumentException(name);
			}
			CanonicalTreeParser p = new CanonicalTreeParser();
			ObjectReader or = db.NewObjectReader();
			try
			{
				p.Reset(or, new RevWalk(db).ParseTree(id));
				return p;
			}
			finally
			{
				or.Release();
			}
		}
	}
}
