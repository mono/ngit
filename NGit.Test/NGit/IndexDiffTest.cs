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

using System.IO;
using NGit;
using NGit.Api;
using NGit.Dircache;
using NGit.Merge;
using NGit.Revwalk;
using NGit.Treewalk;
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class IndexDiffTest : RepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAdded()
		{
			GitIndex index = new GitIndex(db);
			WriteTrashFile("file1", "file1");
			WriteTrashFile("dir/subfile", "dir/subfile");
			Tree tree = new Tree(db);
			tree.SetId(InsertTree(tree));
			index.Add(trash, new FilePath(trash, "file1"));
			index.Add(trash, new FilePath(trash, "dir/subfile"));
			index.Write();
			FileTreeIterator iterator = new FileTreeIterator(db);
			IndexDiff diff = new IndexDiff(db, tree.GetId(), iterator);
			diff.Diff();
			NUnit.Framework.Assert.AreEqual(2, diff.GetAdded().Count);
			NUnit.Framework.Assert.IsTrue(diff.GetAdded().Contains("file1"));
			NUnit.Framework.Assert.IsTrue(diff.GetAdded().Contains("dir/subfile"));
			NUnit.Framework.Assert.AreEqual(0, diff.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetModified().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetRemoved().Count);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRemoved()
		{
			WriteTrashFile("file2", "file2");
			WriteTrashFile("dir/file3", "dir/file3");
			Tree tree = new Tree(db);
			tree.AddFile("file2");
			tree.AddFile("dir/file3");
			NUnit.Framework.Assert.AreEqual(2, tree.MemberCount());
			tree.FindBlobMember("file2").SetId(ObjectId.FromString("30d67d4672d5c05833b7192cc77a79eaafb5c7ad"
				));
			Tree tree2 = (Tree)tree.FindTreeMember("dir");
			tree2.FindBlobMember("file3").SetId(ObjectId.FromString("873fb8d667d05436d728c52b1d7a09528e6eb59b"
				));
			tree2.SetId(InsertTree(tree2));
			tree.SetId(InsertTree(tree));
			FileTreeIterator iterator = new FileTreeIterator(db);
			IndexDiff diff = new IndexDiff(db, tree.GetId(), iterator);
			diff.Diff();
			NUnit.Framework.Assert.AreEqual(2, diff.GetRemoved().Count);
			NUnit.Framework.Assert.IsTrue(diff.GetRemoved().Contains("file2"));
			NUnit.Framework.Assert.IsTrue(diff.GetRemoved().Contains("dir/file3"));
			NUnit.Framework.Assert.AreEqual(0, diff.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetModified().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetAdded().Count);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestModified()
		{
			GitIndex index = new GitIndex(db);
			index.Add(trash, WriteTrashFile("file2", "file2"));
			index.Add(trash, WriteTrashFile("dir/file3", "dir/file3"));
			index.Write();
			WriteTrashFile("dir/file3", "changed");
			Tree tree = new Tree(db);
			tree.AddFile("file2").SetId(ObjectId.FromString("0123456789012345678901234567890123456789"
				));
			tree.AddFile("dir/file3").SetId(ObjectId.FromString("0123456789012345678901234567890123456789"
				));
			NUnit.Framework.Assert.AreEqual(2, tree.MemberCount());
			Tree tree2 = (Tree)tree.FindTreeMember("dir");
			tree2.SetId(InsertTree(tree2));
			tree.SetId(InsertTree(tree));
			FileTreeIterator iterator = new FileTreeIterator(db);
			IndexDiff diff = new IndexDiff(db, tree.GetId(), iterator);
			diff.Diff();
			NUnit.Framework.Assert.AreEqual(2, diff.GetChanged().Count);
			NUnit.Framework.Assert.IsTrue(diff.GetChanged().Contains("file2"));
			NUnit.Framework.Assert.IsTrue(diff.GetChanged().Contains("dir/file3"));
			NUnit.Framework.Assert.AreEqual(1, diff.GetModified().Count);
			NUnit.Framework.Assert.IsTrue(diff.GetModified().Contains("dir/file3"));
			NUnit.Framework.Assert.AreEqual(0, diff.GetAdded().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetRemoved().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetMissing().Count);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestConflicting()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			// create side branch with two modifications
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "1\na(side)\n3\n");
			WriteTrashFile("b", "1\nb\n3\n(side)");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			// update a on master to generate conflict
			CheckoutBranch("refs/heads/master");
			WriteTrashFile("a", "1\na(main)\n3\n");
			git.Add().AddFilepattern("a").Call();
			git.Commit().SetMessage("main").Call();
			// merge side with master
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, result.GetMergeStatus());
			FileTreeIterator iterator = new FileTreeIterator(db);
			IndexDiff diff = new IndexDiff(db, Constants.HEAD, iterator);
			diff.Diff();
			NUnit.Framework.Assert.AreEqual("[b]", new TreeSet<string>(diff.GetChanged()).ToString
				());
			NUnit.Framework.Assert.AreEqual("[]", diff.GetAdded().ToString());
			NUnit.Framework.Assert.AreEqual("[]", diff.GetRemoved().ToString());
			NUnit.Framework.Assert.AreEqual("[]", diff.GetMissing().ToString());
			NUnit.Framework.Assert.AreEqual("[]", diff.GetModified().ToString());
			NUnit.Framework.Assert.AreEqual("[a]", diff.GetConflicting().ToString());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestConflictingDeletedAndModified()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			// create side branch and delete "a"
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			git.Rm().AddFilepattern("a").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			// update a on master to generate conflict
			CheckoutBranch("refs/heads/master");
			WriteTrashFile("a", "1\na(main)\n3\n");
			git.Add().AddFilepattern("a").Call();
			git.Commit().SetMessage("main").Call();
			// merge side with master
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, result.GetMergeStatus());
			FileTreeIterator iterator = new FileTreeIterator(db);
			IndexDiff diff = new IndexDiff(db, Constants.HEAD, iterator);
			diff.Diff();
			NUnit.Framework.Assert.AreEqual("[]", new TreeSet<string>(diff.GetChanged()).ToString
				());
			NUnit.Framework.Assert.AreEqual("[]", diff.GetAdded().ToString());
			NUnit.Framework.Assert.AreEqual("[]", diff.GetRemoved().ToString());
			NUnit.Framework.Assert.AreEqual("[]", diff.GetMissing().ToString());
			NUnit.Framework.Assert.AreEqual("[]", diff.GetModified().ToString());
			NUnit.Framework.Assert.AreEqual("[a]", diff.GetConflicting().ToString());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestConflictingFromMultipleCreations()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			git.Add().AddFilepattern("a").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("b", "1\nb(side)\n3\n");
			git.Add().AddFilepattern("b").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			CheckoutBranch("refs/heads/master");
			WriteTrashFile("b", "1\nb(main)\n3\n");
			git.Add().AddFilepattern("b").Call();
			git.Commit().SetMessage("main").Call();
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, result.GetMergeStatus());
			FileTreeIterator iterator = new FileTreeIterator(db);
			IndexDiff diff = new IndexDiff(db, Constants.HEAD, iterator);
			diff.Diff();
			NUnit.Framework.Assert.AreEqual("[]", new TreeSet<string>(diff.GetChanged()).ToString
				());
			NUnit.Framework.Assert.AreEqual("[]", diff.GetAdded().ToString());
			NUnit.Framework.Assert.AreEqual("[]", diff.GetRemoved().ToString());
			NUnit.Framework.Assert.AreEqual("[]", diff.GetMissing().ToString());
			NUnit.Framework.Assert.AreEqual("[]", diff.GetModified().ToString());
			NUnit.Framework.Assert.AreEqual("[b]", diff.GetConflicting().ToString());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUnchangedSimple()
		{
			GitIndex index = new GitIndex(db);
			index.Add(trash, WriteTrashFile("a.b", "a.b"));
			index.Add(trash, WriteTrashFile("a.c", "a.c"));
			index.Add(trash, WriteTrashFile("a=c", "a=c"));
			index.Add(trash, WriteTrashFile("a=d", "a=d"));
			index.Write();
			Tree tree = new Tree(db);
			// got the hash id'd from the data using echo -n a.b|git hash-object -t blob --stdin
			tree.AddFile("a.b").SetId(ObjectId.FromString("f6f28df96c2b40c951164286e08be7c38ec74851"
				));
			tree.AddFile("a.c").SetId(ObjectId.FromString("6bc0e647512d2a0bef4f26111e484dc87df7f5ca"
				));
			tree.AddFile("a=c").SetId(ObjectId.FromString("06022365ddbd7fb126761319633bf73517770714"
				));
			tree.AddFile("a=d").SetId(ObjectId.FromString("fa6414df3da87840700e9eeb7fc261dd77ccd5c2"
				));
			tree.SetId(InsertTree(tree));
			FileTreeIterator iterator = new FileTreeIterator(db);
			IndexDiff diff = new IndexDiff(db, tree.GetId(), iterator);
			diff.Diff();
			NUnit.Framework.Assert.AreEqual(0, diff.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetAdded().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetRemoved().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetMissing().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetModified().Count);
		}

		/// <summary>
		/// This test has both files and directories that involve
		/// the tricky ordering used by Git.
		/// </summary>
		/// <remarks>
		/// This test has both files and directories that involve
		/// the tricky ordering used by Git.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void TestUnchangedComplex()
		{
			GitIndex index = new GitIndex(db);
			index.Add(trash, WriteTrashFile("a.b", "a.b"));
			index.Add(trash, WriteTrashFile("a.c", "a.c"));
			index.Add(trash, WriteTrashFile("a/b.b/b", "a/b.b/b"));
			index.Add(trash, WriteTrashFile("a/b", "a/b"));
			index.Add(trash, WriteTrashFile("a/c", "a/c"));
			index.Add(trash, WriteTrashFile("a=c", "a=c"));
			index.Add(trash, WriteTrashFile("a=d", "a=d"));
			index.Write();
			Tree tree = new Tree(db);
			// got the hash id'd from the data using echo -n a.b|git hash-object -t blob --stdin
			tree.AddFile("a.b").SetId(ObjectId.FromString("f6f28df96c2b40c951164286e08be7c38ec74851"
				));
			tree.AddFile("a.c").SetId(ObjectId.FromString("6bc0e647512d2a0bef4f26111e484dc87df7f5ca"
				));
			tree.AddFile("a/b.b/b").SetId(ObjectId.FromString("8d840bd4e2f3a48ff417c8e927d94996849933fd"
				));
			tree.AddFile("a/b").SetId(ObjectId.FromString("db89c972fc57862eae378f45b74aca228037d415"
				));
			tree.AddFile("a/c").SetId(ObjectId.FromString("52ad142a008aeb39694bafff8e8f1be75ed7f007"
				));
			tree.AddFile("a=c").SetId(ObjectId.FromString("06022365ddbd7fb126761319633bf73517770714"
				));
			tree.AddFile("a=d").SetId(ObjectId.FromString("fa6414df3da87840700e9eeb7fc261dd77ccd5c2"
				));
			Tree tree3 = (Tree)tree.FindTreeMember("a/b.b");
			tree3.SetId(InsertTree(tree3));
			Tree tree2 = (Tree)tree.FindTreeMember("a");
			tree2.SetId(InsertTree(tree2));
			tree.SetId(InsertTree(tree));
			FileTreeIterator iterator = new FileTreeIterator(db);
			IndexDiff diff = new IndexDiff(db, tree.GetId(), iterator);
			diff.Diff();
			NUnit.Framework.Assert.AreEqual(0, diff.GetChanged().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetAdded().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetRemoved().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetMissing().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetModified().Count);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private ObjectId InsertTree(Tree tree)
		{
			ObjectInserter oi = db.NewObjectInserter();
			try
			{
				ObjectId id = oi.Insert(Constants.OBJ_TREE, tree.Format());
				oi.Flush();
				return id;
			}
			finally
			{
				oi.Release();
			}
		}

		/// <summary>A file is removed from the index but stays in the working directory.</summary>
		/// <remarks>
		/// A file is removed from the index but stays in the working directory. It
		/// is checked if IndexDiff detects this file as removed and untracked.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestRemovedUntracked()
		{
			Git git = new Git(db);
			string path = "file";
			WriteTrashFile(path, "content");
			git.Add().AddFilepattern(path).Call();
			git.Commit().SetMessage("commit").Call();
			RemoveFromIndex(path);
			FileTreeIterator iterator = new FileTreeIterator(db);
			IndexDiff diff = new IndexDiff(db, Constants.HEAD, iterator);
			diff.Diff();
			NUnit.Framework.Assert.IsTrue(diff.GetRemoved().Contains(path));
			NUnit.Framework.Assert.IsTrue(diff.GetUntracked().Contains(path));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAssumeUnchanged()
		{
			Git git = new Git(db);
			string path = "file";
			WriteTrashFile(path, "content");
			git.Add().AddFilepattern(path).Call();
			string path2 = "file2";
			WriteTrashFile(path2, "content");
			git.Add().AddFilepattern(path2).Call();
			git.Commit().SetMessage("commit").Call();
			AssumeUnchanged(path2);
			WriteTrashFile(path, "more content");
			WriteTrashFile(path2, "more content");
			FileTreeIterator iterator = new FileTreeIterator(db);
			IndexDiff diff = new IndexDiff(db, Constants.HEAD, iterator);
			diff.Diff();
			NUnit.Framework.Assert.AreEqual(1, diff.GetAssumeUnchanged().Count);
			NUnit.Framework.Assert.AreEqual(1, diff.GetModified().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetChanged().Count);
			NUnit.Framework.Assert.IsTrue(diff.GetAssumeUnchanged().Contains("file2"));
			NUnit.Framework.Assert.IsTrue(diff.GetModified().Contains("file"));
			git.Add().AddFilepattern(".").Call();
			iterator = new FileTreeIterator(db);
			diff = new IndexDiff(db, Constants.HEAD, iterator);
			diff.Diff();
			NUnit.Framework.Assert.AreEqual(1, diff.GetAssumeUnchanged().Count);
			NUnit.Framework.Assert.AreEqual(0, diff.GetModified().Count);
			NUnit.Framework.Assert.AreEqual(1, diff.GetChanged().Count);
			NUnit.Framework.Assert.IsTrue(diff.GetAssumeUnchanged().Contains("file2"));
			NUnit.Framework.Assert.IsTrue(diff.GetChanged().Contains("file"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void RemoveFromIndex(string path)
		{
			DirCache dirc = db.LockDirCache();
			DirCacheEditor edit = dirc.Editor();
			edit.Add(new DirCacheEditor.DeletePath(path));
			if (!edit.Commit())
			{
				throw new IOException("could not commit");
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssumeUnchanged(string path)
		{
			DirCache dirc = db.LockDirCache();
			DirCacheEntry ent = dirc.GetEntry(path);
			if (ent != null)
			{
				ent.IsAssumeValid = true;
			}
			dirc.Write();
			if (!dirc.Commit())
			{
				throw new IOException("could not commit");
			}
		}
	}
}
