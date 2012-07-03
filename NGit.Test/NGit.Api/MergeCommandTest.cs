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
using NGit.Api.Errors;
using NGit.Merge;
using NGit.Revwalk;
using NGit.Util;
using Sharpen;
using NUnit.Framework;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class MergeCommandTest : RepositoryTestCase
	{
		public static MergeStrategy[] mergeStrategies = MergeStrategy.Get();

		private GitDateFormatter dateFormatter;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			dateFormatter = new GitDateFormatter(GitDateFormatter.Format.DEFAULT);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMergeInItself()
		{
			Git git = new Git(db);
			git.Commit().SetMessage("initial commit").Call();
			MergeCommandResult result = git.Merge().Include(db.GetRef(Constants.HEAD)).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.ALREADY_UP_TO_DATE, result.GetMergeStatus
				());
			// no reflog entry written by merge
			NUnit.Framework.Assert.AreEqual("commit: initial commit", db.GetReflogReader(Constants
				.HEAD).GetLastEntry().GetComment());
			NUnit.Framework.Assert.AreEqual("commit: initial commit", db.GetReflogReader(db.GetBranch
				()).GetLastEntry().GetComment());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAlreadyUpToDate()
		{
			Git git = new Git(db);
			RevCommit first = git.Commit().SetMessage("initial commit").Call();
			CreateBranch(first, "refs/heads/branch1");
			RevCommit second = git.Commit().SetMessage("second commit").Call();
			MergeCommandResult result = git.Merge().Include(db.GetRef("refs/heads/branch1")).
				Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.ALREADY_UP_TO_DATE, result.GetMergeStatus
				());
			NUnit.Framework.Assert.AreEqual(second, result.GetNewHead());
			// no reflog entry written by merge
			NUnit.Framework.Assert.AreEqual("commit: second commit", db.GetReflogReader(Constants
				.HEAD).GetLastEntry().GetComment());
			NUnit.Framework.Assert.AreEqual("commit: second commit", db.GetReflogReader(db.GetBranch
				()).GetLastEntry().GetComment());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFastForward()
		{
			Git git = new Git(db);
			RevCommit first = git.Commit().SetMessage("initial commit").Call();
			CreateBranch(first, "refs/heads/branch1");
			RevCommit second = git.Commit().SetMessage("second commit").Call();
			CheckoutBranch("refs/heads/branch1");
			MergeCommandResult result = git.Merge().Include(db.GetRef(Constants.MASTER)).Call
				();
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAST_FORWARD, result.GetMergeStatus()
				);
			NUnit.Framework.Assert.AreEqual(second, result.GetNewHead());
			NUnit.Framework.Assert.AreEqual("merge refs/heads/master: Fast-forward", db.GetReflogReader
				(Constants.HEAD).GetLastEntry().GetComment());
			NUnit.Framework.Assert.AreEqual("merge refs/heads/master: Fast-forward", db.GetReflogReader
				(db.GetBranch()).GetLastEntry().GetComment());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFastForwardWithFiles()
		{
			Git git = new Git(db);
			WriteTrashFile("file1", "file1");
			git.Add().AddFilepattern("file1").Call();
			RevCommit first = git.Commit().SetMessage("initial commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			CreateBranch(first, "refs/heads/branch1");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			RevCommit second = git.Commit().SetMessage("second commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			CheckoutBranch("refs/heads/branch1");
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file2").Exists());
			MergeCommandResult result = git.Merge().Include(db.GetRef(Constants.MASTER)).Call
				();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAST_FORWARD, result.GetMergeStatus()
				);
			NUnit.Framework.Assert.AreEqual(second, result.GetNewHead());
			NUnit.Framework.Assert.AreEqual("merge refs/heads/master: Fast-forward", db.GetReflogReader
				(Constants.HEAD).GetLastEntry().GetComment());
			NUnit.Framework.Assert.AreEqual("merge refs/heads/master: Fast-forward", db.GetReflogReader
				(db.GetBranch()).GetLastEntry().GetComment());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMultipleHeads()
		{
			Git git = new Git(db);
			WriteTrashFile("file1", "file1");
			git.Add().AddFilepattern("file1").Call();
			RevCommit first = git.Commit().SetMessage("initial commit").Call();
			CreateBranch(first, "refs/heads/branch1");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			RevCommit second = git.Commit().SetMessage("second commit").Call();
			WriteTrashFile("file3", "file3");
			git.Add().AddFilepattern("file3").Call();
			git.Commit().SetMessage("third commit").Call();
			CheckoutBranch("refs/heads/branch1");
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file3").Exists());
			MergeCommand merge = git.Merge();
			merge.Include(second.Id);
			merge.Include(db.GetRef(Constants.MASTER));
			try
			{
				merge.Call();
				NUnit.Framework.Assert.Fail("Expected exception not thrown when merging multiple heads"
					);
			}
			catch (InvalidMergeHeadsException)
			{
			}
		}

		// expected this exception
		/// <exception cref="System.Exception"></exception>
//		[Theory]
		public virtual void TestMergeSuccessAllStrategies(MergeStrategy mergeStrategy)
		{
			Git git = new Git(db);
			RevCommit first = git.Commit().SetMessage("first").Call();
			CreateBranch(first, "refs/heads/side");
			WriteTrashFile("a", "a");
			git.Add().AddFilepattern("a").Call();
			git.Commit().SetMessage("second").Call();
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("b", "b");
			git.Add().AddFilepattern("b").Call();
			git.Commit().SetMessage("third").Call();
			MergeCommandResult result = git.Merge().SetStrategy(mergeStrategy).Include(db.GetRef
				(Constants.MASTER)).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("merge refs/heads/master: Merge made by " + mergeStrategy
				.GetName() + ".", db.GetReflogReader(Constants.HEAD).GetLastEntry().GetComment()
				);
			NUnit.Framework.Assert.AreEqual("merge refs/heads/master: Merge made by " + mergeStrategy
				.GetName() + ".", db.GetReflogReader(db.GetBranch()).GetLastEntry().GetComment()
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestContentMerge()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			WriteTrashFile("c/c/c", "1\nc\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").AddFilepattern("c/c/c").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "1\na(side)\n3\n");
			WriteTrashFile("b", "1\nb(side)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			WriteTrashFile("a", "1\na(main)\n3\n");
			WriteTrashFile("c/c/c", "1\nc(main)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("c/c/c").Call();
			git.Commit().SetMessage("main").Call();
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1\n<<<<<<< HEAD\na(main)\n=======\na(side)\n>>>>>>> 86503e7e397465588cc267b65d778538bffccb83\n3\n"
				, Read(new FilePath(db.WorkTree, "a")));
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			NUnit.Framework.Assert.AreEqual("1\nc(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"c/c/c")));
			NUnit.Framework.Assert.AreEqual(1, result.GetConflicts().Count);
			NUnit.Framework.Assert.AreEqual(3, result.GetConflicts().Get("a")[0].Length);
			NUnit.Framework.Assert.AreEqual(RepositoryState.MERGING, db.GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMergeMessage()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			git.Add().AddFilepattern("a").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "1\na(side)\n3\n");
			git.Add().AddFilepattern("a").Call();
			git.Commit().SetMessage("side").Call();
			CheckoutBranch("refs/heads/master");
			WriteTrashFile("a", "1\na(main)\n3\n");
			git.Add().AddFilepattern("a").Call();
			git.Commit().SetMessage("main").Call();
			Ref sideBranch = db.GetRef("side");
			git.Merge().Include(sideBranch).SetStrategy(MergeStrategy.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual("Merge branch 'side'\n\nConflicts:\n\ta\n", db.ReadMergeCommitMsg
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMergeNonVersionedPaths()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			WriteTrashFile("c/c/c", "1\nc\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").AddFilepattern("c/c/c").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "1\na(side)\n3\n");
			WriteTrashFile("b", "1\nb(side)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			WriteTrashFile("a", "1\na(main)\n3\n");
			WriteTrashFile("c/c/c", "1\nc(main)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("c/c/c").Call();
			git.Commit().SetMessage("main").Call();
			WriteTrashFile("d", "1\nd\n3\n");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "e").Mkdir());
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1\n<<<<<<< HEAD\na(main)\n=======\na(side)\n>>>>>>> 86503e7e397465588cc267b65d778538bffccb83\n3\n"
				, Read(new FilePath(db.WorkTree, "a")));
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			NUnit.Framework.Assert.AreEqual("1\nc(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"c/c/c")));
			NUnit.Framework.Assert.AreEqual("1\nd\n3\n", Read(new FilePath(db.WorkTree, "d"))
				);
			FilePath dir = new FilePath(db.WorkTree, "e");
			NUnit.Framework.Assert.IsTrue(dir.IsDirectory());
			NUnit.Framework.Assert.AreEqual(1, result.GetConflicts().Count);
			NUnit.Framework.Assert.AreEqual(3, result.GetConflicts().Get("a")[0].Length);
			NUnit.Framework.Assert.AreEqual(RepositoryState.MERGING, db.GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMultipleCreations()
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
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMultipleCreationsSameContent()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			git.Add().AddFilepattern("a").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("b", "1\nb(1)\n3\n");
			git.Add().AddFilepattern("b").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			CheckoutBranch("refs/heads/master");
			WriteTrashFile("b", "1\nb(1)\n3\n");
			git.Add().AddFilepattern("b").Call();
			git.Commit().SetMessage("main").Call();
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1\nb(1)\n3\n", Read(new FilePath(db.WorkTree, "b"
				)));
			NUnit.Framework.Assert.AreEqual("merge " + secondCommit.Id.GetName() + ": Merge made by resolve."
				, db.GetReflogReader(Constants.HEAD).GetLastEntry().GetComment());
			NUnit.Framework.Assert.AreEqual("merge " + secondCommit.Id.GetName() + ": Merge made by resolve."
				, db.GetReflogReader(db.GetBranch()).GetLastEntry().GetComment());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSuccessfulContentMerge()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			WriteTrashFile("c/c/c", "1\nc\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").AddFilepattern("c/c/c").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "1(side)\na\n3\n");
			WriteTrashFile("b", "1\nb(side)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			WriteTrashFile("a", "1\na\n3(main)\n");
			WriteTrashFile("c/c/c", "1\nc(main)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("c/c/c").Call();
			RevCommit thirdCommit = git.Commit().SetMessage("main").Call();
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1(side)\na\n3(main)\n", Read(new FilePath(db.WorkTree
				, "a")));
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			NUnit.Framework.Assert.AreEqual("1\nc(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"c/c/c")));
			NUnit.Framework.Assert.AreEqual(null, result.GetConflicts());
			NUnit.Framework.Assert.IsTrue(2 == result.GetMergedCommits().Length);
			NUnit.Framework.Assert.AreEqual(thirdCommit, result.GetMergedCommits()[0]);
			NUnit.Framework.Assert.AreEqual(secondCommit, result.GetMergedCommits()[1]);
			Iterator<RevCommit> it = git.Log().Call().Iterator();
			RevCommit newHead = it.Next();
			NUnit.Framework.Assert.AreEqual(newHead, result.GetNewHead());
			NUnit.Framework.Assert.AreEqual(2, newHead.ParentCount);
			NUnit.Framework.Assert.AreEqual(thirdCommit, newHead.GetParent(0));
			NUnit.Framework.Assert.AreEqual(secondCommit, newHead.GetParent(1));
			NUnit.Framework.Assert.AreEqual("Merge commit '3fa334456d236a92db020289fe0bf481d91777b4'"
				, newHead.GetFullMessage());
			// @TODO fix me
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
		}

		// test index state
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSuccessfulContentMergeAndDirtyworkingTree()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			WriteTrashFile("d", "1\nd\n3\n");
			WriteTrashFile("c/c/c", "1\nc\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").AddFilepattern("c/c/c").AddFilepattern
				("d").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "1(side)\na\n3\n");
			WriteTrashFile("b", "1\nb(side)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			WriteTrashFile("a", "1\na\n3(main)\n");
			WriteTrashFile("c/c/c", "1\nc(main)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("c/c/c").Call();
			RevCommit thirdCommit = git.Commit().SetMessage("main").Call();
			WriteTrashFile("d", "--- dirty ---");
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1(side)\na\n3(main)\n", Read(new FilePath(db.WorkTree
				, "a")));
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			NUnit.Framework.Assert.AreEqual("1\nc(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"c/c/c")));
			NUnit.Framework.Assert.AreEqual("--- dirty ---", Read(new FilePath(db.WorkTree, "d"
				)));
			NUnit.Framework.Assert.AreEqual(null, result.GetConflicts());
			NUnit.Framework.Assert.IsTrue(2 == result.GetMergedCommits().Length);
			NUnit.Framework.Assert.AreEqual(thirdCommit, result.GetMergedCommits()[0]);
			NUnit.Framework.Assert.AreEqual(secondCommit, result.GetMergedCommits()[1]);
			Iterator<RevCommit> it = git.Log().Call().Iterator();
			RevCommit newHead = it.Next();
			NUnit.Framework.Assert.AreEqual(newHead, result.GetNewHead());
			NUnit.Framework.Assert.AreEqual(2, newHead.ParentCount);
			NUnit.Framework.Assert.AreEqual(thirdCommit, newHead.GetParent(0));
			NUnit.Framework.Assert.AreEqual(secondCommit, newHead.GetParent(1));
			NUnit.Framework.Assert.AreEqual("Merge commit '064d54d98a4cdb0fed1802a21c656bfda67fe879'"
				, newHead.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSingleDeletion()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			WriteTrashFile("d", "1\nd\n3\n");
			WriteTrashFile("c/c/c", "1\nc\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").AddFilepattern("c/c/c").AddFilepattern
				("d").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "b").Delete());
			git.Add().AddFilepattern("b").SetUpdate(true).Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "b").Exists());
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "b").Exists());
			WriteTrashFile("a", "1\na\n3(main)\n");
			WriteTrashFile("c/c/c", "1\nc(main)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("c/c/c").Call();
			RevCommit thirdCommit = git.Commit().SetMessage("main").Call();
			// We are merging a deletion into our branch
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1\na\n3(main)\n", Read(new FilePath(db.WorkTree, 
				"a")));
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "b").Exists());
			NUnit.Framework.Assert.AreEqual("1\nc(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"c/c/c")));
			NUnit.Framework.Assert.AreEqual("1\nd\n3\n", Read(new FilePath(db.WorkTree, "d"))
				);
			// Do the opposite, be on a branch where we have deleted a file and
			// merge in a old commit where this file was not deleted
			CheckoutBranch("refs/heads/side");
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "b").Exists());
			result = git.Merge().Include(thirdCommit.Id).SetStrategy(MergeStrategy.RESOLVE).Call
				();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1\na\n3(main)\n", Read(new FilePath(db.WorkTree, 
				"a")));
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "b").Exists());
			NUnit.Framework.Assert.AreEqual("1\nc(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"c/c/c")));
			NUnit.Framework.Assert.AreEqual("1\nd\n3\n", Read(new FilePath(db.WorkTree, "d"))
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMultipleDeletions()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			git.Add().AddFilepattern("a").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "a").Delete());
			git.Add().AddFilepattern("a").SetUpdate(true).Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "a").Exists());
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "a").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "a").Delete());
			git.Add().AddFilepattern("a").SetUpdate(true).Call();
			git.Commit().SetMessage("main").Call();
			// We are merging a deletion into our branch
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeletionAndConflict()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			WriteTrashFile("d", "1\nd\n3\n");
			WriteTrashFile("c/c/c", "1\nc\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").AddFilepattern("c/c/c").AddFilepattern
				("d").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "b").Delete());
			WriteTrashFile("a", "1\na\n3(side)\n");
			git.Add().AddFilepattern("b").SetUpdate(true).Call();
			git.Add().AddFilepattern("a").SetUpdate(true).Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "b").Exists());
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "b").Exists());
			WriteTrashFile("a", "1\na\n3(main)\n");
			WriteTrashFile("c/c/c", "1\nc(main)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("c/c/c").Call();
			git.Commit().SetMessage("main").Call();
			// We are merging a deletion into our branch
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1\na\n<<<<<<< HEAD\n3(main)\n=======\n3(side)\n>>>>>>> 54ffed45d62d252715fc20e41da92d44c48fb0ff\n"
				, Read(new FilePath(db.WorkTree, "a")));
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "b").Exists());
			NUnit.Framework.Assert.AreEqual("1\nc(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"c/c/c")));
			NUnit.Framework.Assert.AreEqual("1\nd\n3\n", Read(new FilePath(db.WorkTree, "d"))
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeletionOnMasterConflict()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			// create side branch and modify "a"
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "1\na(side)\n3\n");
			git.Add().AddFilepattern("a").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			// delete a on master to generate conflict
			CheckoutBranch("refs/heads/master");
			git.Rm().AddFilepattern("a").Call();
			git.Commit().SetMessage("main").Call();
			// merge side with master
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, result.GetMergeStatus());
			// result should be 'a' conflicting with workspace content from side
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "a").Exists());
			NUnit.Framework.Assert.AreEqual("1\na(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"a")));
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDeletionOnSideConflict()
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
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "a").Exists());
			NUnit.Framework.Assert.AreEqual("1\na(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"a")));
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			NUnit.Framework.Assert.AreEqual(1, result.GetConflicts().Count);
			NUnit.Framework.Assert.AreEqual(3, result.GetConflicts().Get("a")[0].Length);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestModifiedAndRenamed()
		{
			// this test is essentially the same as testDeletionOnSideConflict,
			// however if once rename support is added this test should result in a
			// successful merge instead of a conflict
			Git git = new Git(db);
			WriteTrashFile("x", "add x");
			git.Add().AddFilepattern("x").Call();
			RevCommit initial = git.Commit().SetMessage("add x").Call();
			CreateBranch(initial, "refs/heads/d1");
			CreateBranch(initial, "refs/heads/d2");
			// rename x to y on d1
			CheckoutBranch("refs/heads/d1");
			new FilePath(db.WorkTree, "x").RenameTo(new FilePath(db.WorkTree, "y"));
			git.Rm().AddFilepattern("x").Call();
			git.Add().AddFilepattern("y").Call();
			RevCommit d1Commit = git.Commit().SetMessage("d1 rename x -> y").Call();
			CheckoutBranch("refs/heads/d2");
			WriteTrashFile("x", "d2 change");
			git.Add().AddFilepattern("x").Call();
			RevCommit d2Commit = git.Commit().SetMessage("d2 change in x").Call();
			CheckoutBranch("refs/heads/master");
			MergeCommandResult d1Merge = git.Merge().Include(d1Commit).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAST_FORWARD, d1Merge.GetMergeStatus(
				));
			MergeCommandResult d2Merge = git.Merge().Include(d2Commit).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, d2Merge.GetMergeStatus()
				);
			NUnit.Framework.Assert.AreEqual(1, d2Merge.GetConflicts().Count);
			NUnit.Framework.Assert.AreEqual(3, d2Merge.GetConflicts().Get("x")[0].Length);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMergeFailingWithDirtyWorkingTree()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("a", "1(side)\na\n3\n");
			WriteTrashFile("b", "1\nb(side)\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			NUnit.Framework.Assert.AreEqual("1\nb(side)\n3\n", Read(new FilePath(db.WorkTree, 
				"b")));
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			WriteTrashFile("a", "1\na\n3(main)\n");
			git.Add().AddFilepattern("a").Call();
			git.Commit().SetMessage("main").Call();
			WriteTrashFile("a", "--- dirty ---");
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAILED, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("--- dirty ---", Read(new FilePath(db.WorkTree, "a"
				)));
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			NUnit.Framework.Assert.AreEqual(null, result.GetConflicts());
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMergeConflictFileFolder()
		{
			Git git = new Git(db);
			WriteTrashFile("a", "1\na\n3\n");
			WriteTrashFile("b", "1\nb\n3\n");
			git.Add().AddFilepattern("a").AddFilepattern("b").Call();
			RevCommit initialCommit = git.Commit().SetMessage("initial").Call();
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("c/c/c", "1\nc(side)\n3\n");
			WriteTrashFile("d", "1\nd(side)\n3\n");
			git.Add().AddFilepattern("c/c/c").AddFilepattern("d").Call();
			RevCommit secondCommit = git.Commit().SetMessage("side").Call();
			CheckoutBranch("refs/heads/master");
			WriteTrashFile("c", "1\nc(main)\n3\n");
			WriteTrashFile("d/d/d", "1\nd(main)\n3\n");
			git.Add().AddFilepattern("c").AddFilepattern("d/d/d").Call();
			git.Commit().SetMessage("main").Call();
			MergeCommandResult result = git.Merge().Include(secondCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual("1\na\n3\n", Read(new FilePath(db.WorkTree, "a"))
				);
			NUnit.Framework.Assert.AreEqual("1\nb\n3\n", Read(new FilePath(db.WorkTree, "b"))
				);
			NUnit.Framework.Assert.AreEqual("1\nc(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"c")));
			NUnit.Framework.Assert.AreEqual("1\nd(main)\n3\n", Read(new FilePath(db.WorkTree, 
				"d/d/d")));
			NUnit.Framework.Assert.AreEqual(null, result.GetConflicts());
			NUnit.Framework.Assert.AreEqual(RepositoryState.MERGING, db.GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSuccessfulMergeFailsDueToDirtyIndex()
		{
			Git git = new Git(db);
			FilePath fileA = WriteTrashFile("a", "a");
			RevCommit initialCommit = AddAllAndCommit(git);
			// switch branch
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			// modify file a
			Write(fileA, "a(side)");
			WriteTrashFile("b", "b");
			RevCommit sideCommit = AddAllAndCommit(git);
			// switch branch
			CheckoutBranch("refs/heads/master");
			WriteTrashFile("c", "c");
			AddAllAndCommit(git);
			// modify and add file a
			Write(fileA, "a(modified)");
			git.Add().AddFilepattern("a").Call();
			// do not commit
			// get current index state
			string indexState = IndexState(CONTENT);
			// merge
			MergeCommandResult result = git.Merge().Include(sideCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			CheckMergeFailedResult(result, ResolveMerger.MergeFailureReason.DIRTY_INDEX, indexState
				, fileA);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestConflictingMergeFailsDueToDirtyIndex()
		{
			Git git = new Git(db);
			FilePath fileA = WriteTrashFile("a", "a");
			RevCommit initialCommit = AddAllAndCommit(git);
			// switch branch
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			// modify file a
			Write(fileA, "a(side)");
			WriteTrashFile("b", "b");
			RevCommit sideCommit = AddAllAndCommit(git);
			// switch branch
			CheckoutBranch("refs/heads/master");
			// modify file a - this will cause a conflict during merge
			Write(fileA, "a(master)");
			WriteTrashFile("c", "c");
			AddAllAndCommit(git);
			// modify and add file a
			Write(fileA, "a(modified)");
			git.Add().AddFilepattern("a").Call();
			// do not commit
			// get current index state
			string indexState = IndexState(CONTENT);
			// merge
			MergeCommandResult result = git.Merge().Include(sideCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			CheckMergeFailedResult(result, ResolveMerger.MergeFailureReason.DIRTY_INDEX, indexState
				, fileA);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSuccessfulMergeFailsDueToDirtyWorktree()
		{
			Git git = new Git(db);
			FilePath fileA = WriteTrashFile("a", "a");
			RevCommit initialCommit = AddAllAndCommit(git);
			// switch branch
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			// modify file a
			Write(fileA, "a(side)");
			WriteTrashFile("b", "b");
			RevCommit sideCommit = AddAllAndCommit(git);
			// switch branch
			CheckoutBranch("refs/heads/master");
			WriteTrashFile("c", "c");
			AddAllAndCommit(git);
			// modify file a
			Write(fileA, "a(modified)");
			// do not add and commit
			// get current index state
			string indexState = IndexState(CONTENT);
			// merge
			MergeCommandResult result = git.Merge().Include(sideCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			CheckMergeFailedResult(result, ResolveMerger.MergeFailureReason.DIRTY_WORKTREE, indexState
				, fileA);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestConflictingMergeFailsDueToDirtyWorktree()
		{
			Git git = new Git(db);
			FilePath fileA = WriteTrashFile("a", "a");
			RevCommit initialCommit = AddAllAndCommit(git);
			// switch branch
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			// modify file a
			Write(fileA, "a(side)");
			WriteTrashFile("b", "b");
			RevCommit sideCommit = AddAllAndCommit(git);
			// switch branch
			CheckoutBranch("refs/heads/master");
			// modify file a - this will cause a conflict during merge
			Write(fileA, "a(master)");
			WriteTrashFile("c", "c");
			AddAllAndCommit(git);
			// modify file a
			Write(fileA, "a(modified)");
			// do not add and commit
			// get current index state
			string indexState = IndexState(CONTENT);
			// merge
			MergeCommandResult result = git.Merge().Include(sideCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			CheckMergeFailedResult(result, ResolveMerger.MergeFailureReason.DIRTY_WORKTREE, indexState
				, fileA);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMergeRemovingFolders()
		{
			FilePath folder1 = new FilePath(db.WorkTree, "folder1");
			FilePath folder2 = new FilePath(db.WorkTree, "folder2");
			FileUtils.Mkdir(folder1);
			FileUtils.Mkdir(folder2);
			FilePath file = new FilePath(folder1, "file1.txt");
			Write(file, "folder1--file1.txt");
			file = new FilePath(folder1, "file2.txt");
			Write(file, "folder1--file2.txt");
			file = new FilePath(folder2, "file1.txt");
			Write(file, "folder--file1.txt");
			file = new FilePath(folder2, "file2.txt");
			Write(file, "folder2--file2.txt");
			Git git = new Git(db);
			git.Add().AddFilepattern(folder1.GetName()).AddFilepattern(folder2.GetName()).Call
				();
			RevCommit commit1 = git.Commit().SetMessage("adding folders").Call();
			RecursiveDelete(folder1);
			RecursiveDelete(folder2);
			git.Rm().AddFilepattern("folder1/file1.txt").AddFilepattern("folder1/file2.txt").
				AddFilepattern("folder2/file1.txt").AddFilepattern("folder2/file2.txt").Call();
			RevCommit commit2 = git.Commit().SetMessage("removing folders on 'branch'").Call(
				);
			git.Checkout().SetName(commit1.Name).Call();
			MergeCommandResult result = git.Merge().Include(commit2.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAST_FORWARD, result.GetMergeStatus()
				);
			NUnit.Framework.Assert.AreEqual(commit2, result.GetNewHead());
			NUnit.Framework.Assert.IsFalse(folder1.Exists());
			NUnit.Framework.Assert.IsFalse(folder2.Exists());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMergeRemovingFoldersWithoutFastForward()
		{
			FilePath folder1 = new FilePath(db.WorkTree, "folder1");
			FilePath folder2 = new FilePath(db.WorkTree, "folder2");
			FileUtils.Mkdir(folder1);
			FileUtils.Mkdir(folder2);
			FilePath file = new FilePath(folder1, "file1.txt");
			Write(file, "folder1--file1.txt");
			file = new FilePath(folder1, "file2.txt");
			Write(file, "folder1--file2.txt");
			file = new FilePath(folder2, "file1.txt");
			Write(file, "folder--file1.txt");
			file = new FilePath(folder2, "file2.txt");
			Write(file, "folder2--file2.txt");
			Git git = new Git(db);
			git.Add().AddFilepattern(folder1.GetName()).AddFilepattern(folder2.GetName()).Call
				();
			RevCommit @base = git.Commit().SetMessage("adding folders").Call();
			RecursiveDelete(folder1);
			RecursiveDelete(folder2);
			git.Rm().AddFilepattern("folder1/file1.txt").AddFilepattern("folder1/file2.txt").
				AddFilepattern("folder2/file1.txt").AddFilepattern("folder2/file2.txt").Call();
			RevCommit other = git.Commit().SetMessage("removing folders on 'branch'").Call();
			git.Checkout().SetName(@base.Name).Call();
			file = new FilePath(folder2, "file3.txt");
			Write(file, "folder2--file3.txt");
			git.Add().AddFilepattern(folder2.GetName()).Call();
			git.Commit().SetMessage("adding another file").Call();
			MergeCommandResult result = git.Merge().Include(other.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
			NUnit.Framework.Assert.IsFalse(folder1.Exists());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileModeMerge()
		{
			if (!FS.DETECTED.SupportsExecute())
			{
				return;
			}
			// Only Java6
			Git git = new Git(db);
			WriteTrashFile("mergeableMode", "a");
			SetExecutable(git, "mergeableMode", false);
			WriteTrashFile("conflictingModeWithBase", "a");
			SetExecutable(git, "conflictingModeWithBase", false);
			RevCommit initialCommit = AddAllAndCommit(git);
			// switch branch
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			SetExecutable(git, "mergeableMode", true);
			WriteTrashFile("conflictingModeNoBase", "b");
			SetExecutable(git, "conflictingModeNoBase", true);
			RevCommit sideCommit = AddAllAndCommit(git);
			// switch branch
			CreateBranch(initialCommit, "refs/heads/side2");
			CheckoutBranch("refs/heads/side2");
			SetExecutable(git, "mergeableMode", false);
			NUnit.Framework.Assert.IsFalse(new FilePath(git.GetRepository().WorkTree, "conflictingModeNoBase"
				).Exists());
			WriteTrashFile("conflictingModeNoBase", "b");
			SetExecutable(git, "conflictingModeNoBase", false);
			AddAllAndCommit(git);
			// merge
			MergeCommandResult result = git.Merge().Include(sideCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, result.GetMergeStatus());
			NUnit.Framework.Assert.IsTrue(CanExecute(git, "mergeableMode"));
			NUnit.Framework.Assert.IsFalse(CanExecute(git, "conflictingModeNoBase"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFileModeMergeWithDirtyWorkTree()
		{
			if (!FS.DETECTED.SupportsExecute())
			{
				return;
			}
			// Only Java6 (or set x bit in index)
			Git git = new Git(db);
			WriteTrashFile("mergeableButDirty", "a");
			SetExecutable(git, "mergeableButDirty", false);
			RevCommit initialCommit = AddAllAndCommit(git);
			// switch branch
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			SetExecutable(git, "mergeableButDirty", true);
			RevCommit sideCommit = AddAllAndCommit(git);
			// switch branch
			CreateBranch(initialCommit, "refs/heads/side2");
			CheckoutBranch("refs/heads/side2");
			SetExecutable(git, "mergeableButDirty", false);
			AddAllAndCommit(git);
			WriteTrashFile("mergeableButDirty", "b");
			// merge
			MergeCommandResult result = git.Merge().Include(sideCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAILED, result.GetMergeStatus());
			NUnit.Framework.Assert.IsFalse(CanExecute(git, "mergeableButDirty"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSquashFastForward()
		{
			Git git = new Git(db);
			WriteTrashFile("file1", "file1");
			git.Add().AddFilepattern("file1").Call();
			RevCommit first = git.Commit().SetMessage("initial commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			CreateBranch(first, "refs/heads/branch1");
			CheckoutBranch("refs/heads/branch1");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			RevCommit second = git.Commit().SetMessage("second commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			WriteTrashFile("file3", "file3");
			git.Add().AddFilepattern("file3").Call();
			RevCommit third = git.Commit().SetMessage("third commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file3").Exists());
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file3").Exists());
			MergeCommandResult result = git.Merge().Include(db.GetRef("branch1")).SetSquash(true
				).Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file3").Exists());
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAST_FORWARD_SQUASHED, result.GetMergeStatus
				());
			NUnit.Framework.Assert.AreEqual(first, result.GetNewHead());
			// HEAD didn't move
			NUnit.Framework.Assert.AreEqual(first, db.Resolve(Constants.HEAD + "^{commit}"));
			NUnit.Framework.Assert.AreEqual("Squashed commit of the following:\n\ncommit " + 
				third.GetName() + "\nAuthor: " + third.GetAuthorIdent().GetName() + " <" + third
				.GetAuthorIdent().GetEmailAddress() + ">\nDate:   " + dateFormatter.FormatDate(third
				.GetAuthorIdent()) + "\n\n\tthird commit\n\ncommit " + second.GetName() + "\nAuthor: "
				 + second.GetAuthorIdent().GetName() + " <" + second.GetAuthorIdent().GetEmailAddress
				() + ">\nDate:   " + dateFormatter.FormatDate(second.GetAuthorIdent()) + "\n\n\tsecond commit\n"
				, db.ReadSquashCommitMsg());
			NUnit.Framework.Assert.IsNull(db.ReadMergeCommitMsg());
			Status stat = git.Status().Call();
			NUnit.Framework.CollectionAssert.AreEquivalent(StatusCommandTest.Set(new [] { "file2", "file3" }), stat.GetAdded
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSquashMerge()
		{
			Git git = new Git(db);
			WriteTrashFile("file1", "file1");
			git.Add().AddFilepattern("file1").Call();
			RevCommit first = git.Commit().SetMessage("initial commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			CreateBranch(first, "refs/heads/branch1");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			RevCommit second = git.Commit().SetMessage("second commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			CheckoutBranch("refs/heads/branch1");
			WriteTrashFile("file3", "file3");
			git.Add().AddFilepattern("file3").Call();
			RevCommit third = git.Commit().SetMessage("third commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file3").Exists());
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file3").Exists());
			MergeCommandResult result = git.Merge().Include(db.GetRef("branch1")).SetSquash(true
				).Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file3").Exists());
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED_SQUASHED, result.GetMergeStatus
				());
			NUnit.Framework.Assert.AreEqual(second, result.GetNewHead());
			// HEAD didn't move
			NUnit.Framework.Assert.AreEqual(second, db.Resolve(Constants.HEAD + "^{commit}"));
			NUnit.Framework.Assert.AreEqual("Squashed commit of the following:\n\ncommit " + 
				third.GetName() + "\nAuthor: " + third.GetAuthorIdent().GetName() + " <" + third
				.GetAuthorIdent().GetEmailAddress() + ">\nDate:   " + dateFormatter.FormatDate(third
				.GetAuthorIdent()) + "\n\n\tthird commit\n", db.ReadSquashCommitMsg());
			NUnit.Framework.Assert.IsNull(db.ReadMergeCommitMsg());
			Status stat = git.Status().Call();
			NUnit.Framework.CollectionAssert.AreEquivalent(StatusCommandTest.Set("file3"), stat.GetAdded());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSquashMergeConflict()
		{
			Git git = new Git(db);
			WriteTrashFile("file1", "file1");
			git.Add().AddFilepattern("file1").Call();
			RevCommit first = git.Commit().SetMessage("initial commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			CreateBranch(first, "refs/heads/branch1");
			WriteTrashFile("file2", "master");
			git.Add().AddFilepattern("file2").Call();
			RevCommit second = git.Commit().SetMessage("second commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			CheckoutBranch("refs/heads/branch1");
			WriteTrashFile("file2", "branch");
			git.Add().AddFilepattern("file2").Call();
			RevCommit third = git.Commit().SetMessage("third commit").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			CheckoutBranch("refs/heads/master");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			MergeCommandResult result = git.Merge().Include(db.GetRef("branch1")).SetSquash(true
				).Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.AreEqual(MergeStatus.CONFLICTING, result.GetMergeStatus());
			NUnit.Framework.Assert.IsNull(result.GetNewHead());
			NUnit.Framework.Assert.AreEqual(second, db.Resolve(Constants.HEAD + "^{commit}"));
			NUnit.Framework.Assert.AreEqual("Squashed commit of the following:\n\ncommit " + 
				third.GetName() + "\nAuthor: " + third.GetAuthorIdent().GetName() + " <" + third
				.GetAuthorIdent().GetEmailAddress() + ">\nDate:   " + dateFormatter.FormatDate(third
				.GetAuthorIdent()) + "\n\n\tthird commit\n", db.ReadSquashCommitMsg());
			NUnit.Framework.Assert.AreEqual("\nConflicts:\n\tfile2\n", db.ReadMergeCommitMsg(
				));
			Status stat = git.Status().Call();
			NUnit.Framework.CollectionAssert.AreEquivalent(StatusCommandTest.Set("file2"), stat.GetConflicting
				());
		}

		private void SetExecutable(Git git, string path, bool executable)
		{
			FS.DETECTED.SetExecute(new FilePath(git.GetRepository().WorkTree, path), executable
				);
		}

		private bool CanExecute(Git git, string path)
		{
			return FS.DETECTED.CanExecute(new FilePath(git.GetRepository().WorkTree, path));
		}

		/// <exception cref="System.Exception"></exception>
		private RevCommit AddAllAndCommit(Git git)
		{
			git.Add().AddFilepattern(".").Call();
			return git.Commit().SetMessage("message").Call();
		}

		/// <exception cref="System.Exception"></exception>
		private void CheckMergeFailedResult(MergeCommandResult result, ResolveMerger.MergeFailureReason
			 reason, string indexState, FilePath fileA)
		{
			NUnit.Framework.Assert.AreEqual(MergeStatus.FAILED, result.GetMergeStatus());
			NUnit.Framework.Assert.AreEqual(reason, result.GetFailingPaths().Get("a"));
			NUnit.Framework.Assert.AreEqual("a(modified)", Read(fileA));
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "b").Exists());
			NUnit.Framework.Assert.AreEqual("c", Read(new FilePath(db.WorkTree, "c")));
			NUnit.Framework.Assert.AreEqual(indexState, IndexState(CONTENT));
			NUnit.Framework.Assert.AreEqual(null, result.GetConflicts());
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
		}
	}
}
