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
using NGit.Dircache;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Api
{
	public class RebaseCommandTest : RepositoryTestCase
	{
		/// <exception cref="System.IO.IOException"></exception>
		private void CreateBranch(ObjectId objectId, string branchName)
		{
			RefUpdate updateRef = db.UpdateRef(branchName);
			updateRef.SetNewObjectId(objectId);
			updateRef.Update();
		}

		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private void CheckoutBranch(string branchName)
		{
			RevWalk walk = new RevWalk(db);
			RevCommit head = walk.ParseCommit(db.Resolve(Constants.HEAD));
			RevCommit branch = walk.ParseCommit(db.Resolve(branchName));
			DirCacheCheckout dco = new DirCacheCheckout(db, head.Tree.Id, db.LockDirCache(), 
				branch.Tree.Id);
			dco.SetFailOnConflict(true);
			dco.Checkout();
			walk.Release();
			// update the HEAD
			RefUpdate refUpdate = db.UpdateRef(Constants.HEAD);
			refUpdate.Link(branchName);
		}

		/// <exception cref="System.InvalidOperationException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private void CheckoutCommit(RevCommit commit)
		{
			RevWalk walk = new RevWalk(db);
			RevCommit head = walk.ParseCommit(db.Resolve(Constants.HEAD));
			DirCacheCheckout dco = new DirCacheCheckout(db, head.Tree, db.LockDirCache(), commit
				.Tree);
			dco.SetFailOnConflict(true);
			dco.Checkout();
			walk.Release();
			// update the HEAD
			RefUpdate refUpdate = db.UpdateRef(Constants.HEAD, true);
			refUpdate.SetNewObjectId(commit);
			refUpdate.ForceUpdate();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFastForwardWithNewFile()
		{
			Git git = new Git(db);
			// create file1 on master
			WriteTrashFile("file1", "file1");
			git.Add().AddFilepattern("file1").Call();
			RevCommit first = git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			// create a topic branch
			CreateBranch(first, "refs/heads/topic");
			// create file2 on master
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("Add file2").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			CheckoutBranch("refs/heads/topic");
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file2").Exists());
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.UP_TO_DATE, res.GetStatus());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUpToDate()
		{
			Git git = new Git(db);
			// create file1 on master
			WriteTrashFile("file1", "file1");
			git.Add().AddFilepattern("file1").Call();
			RevCommit first = git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			RebaseResult res = git.Rebase().SetUpstream(first).Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.UP_TO_DATE, res.GetStatus());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUnknownUpstream()
		{
			Git git = new Git(db);
			// create file1 on master
			WriteTrashFile("file1", "file1");
			git.Add().AddFilepattern("file1").Call();
			git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			try
			{
				git.Rebase().SetUpstream("refs/heads/xyz").Call();
				NUnit.Framework.Assert.Fail("expected exception was not thrown");
			}
			catch (RefNotFoundException)
			{
			}
		}

		// expected exception
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestConflictFreeWithSingleFile()
		{
			Git git = new Git(db);
			// create file1 on master
			FilePath theFile = WriteTrashFile("file1", "1\n2\n3\n");
			git.Add().AddFilepattern("file1").Call();
			RevCommit second = git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			// change first line in master and commit
			WriteTrashFile("file1", "1master\n2\n3\n");
			CheckFile(theFile, "1master\n2\n3\n");
			git.Add().AddFilepattern("file1").Call();
			RevCommit lastMasterChange = git.Commit().SetMessage("change file1 in master").Call
				();
			// create a topic branch based on second commit
			CreateBranch(second, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(theFile, "1\n2\n3\n");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			// change third line in topic branch
			WriteTrashFile("file1", "1\n2\n3\ntopic\n");
			git.Add().AddFilepattern("file1").Call();
			git.Commit().SetMessage("change file1 in topic").Call();
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, res.GetStatus());
			CheckFile(theFile, "1master\n2\n3\ntopic\n");
			// our old branch should be checked out again
			NUnit.Framework.Assert.AreEqual("refs/heads/topic", db.GetFullBranch());
			AssertEquals(lastMasterChange, new RevWalk(db).ParseCommit(db.Resolve(Constants.HEAD
				)).GetParent(0));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDetachedHead()
		{
			Git git = new Git(db);
			// create file1 on master
			FilePath theFile = WriteTrashFile("file1", "1\n2\n3\n");
			git.Add().AddFilepattern("file1").Call();
			RevCommit second = git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			// change first line in master and commit
			WriteTrashFile("file1", "1master\n2\n3\n");
			CheckFile(theFile, "1master\n2\n3\n");
			git.Add().AddFilepattern("file1").Call();
			RevCommit lastMasterChange = git.Commit().SetMessage("change file1 in master").Call
				();
			// create a topic branch based on second commit
			CreateBranch(second, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(theFile, "1\n2\n3\n");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			// change third line in topic branch
			WriteTrashFile("file1", "1\n2\n3\ntopic\n");
			git.Add().AddFilepattern("file1").Call();
			RevCommit topicCommit = git.Commit().SetMessage("change file1 in topic").Call();
			CheckoutBranch("refs/heads/master");
			CheckoutCommit(topicCommit);
			NUnit.Framework.Assert.AreEqual(topicCommit.Id.GetName(), db.GetFullBranch());
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, res.GetStatus());
			CheckFile(theFile, "1master\n2\n3\ntopic\n");
			AssertEquals(lastMasterChange, new RevWalk(db).ParseCommit(db.Resolve(Constants.HEAD
				)).GetParent(0));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFilesAddedFromTwoBranches()
		{
			Git git = new Git(db);
			// create file1 on master
			WriteTrashFile("file1", "file1");
			git.Add().AddFilepattern("file1").Call();
			RevCommit masterCommit = git.Commit().SetMessage("Add file1 to master").Call();
			// create a branch named file2 and add file2
			CreateBranch(masterCommit, "refs/heads/file2");
			CheckoutBranch("refs/heads/file2");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			RevCommit addFile2 = git.Commit().SetMessage("Add file2 to branch file2").Call();
			// create a branch named file3 and add file3
			CreateBranch(masterCommit, "refs/heads/file3");
			CheckoutBranch("refs/heads/file3");
			WriteTrashFile("file3", "file3");
			git.Add().AddFilepattern("file3").Call();
			git.Commit().SetMessage("Add file3 to branch file3").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file3").Exists());
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/file2").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, res.GetStatus());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file3").Exists());
			// our old branch should be checked out again
			NUnit.Framework.Assert.AreEqual("refs/heads/file3", db.GetFullBranch());
			AssertEquals(addFile2, new RevWalk(db).ParseCommit(db.Resolve(Constants.HEAD)).GetParent
				(0));
			CheckoutBranch("refs/heads/file2");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file3").Exists());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAbortOnConflict()
		{
			Git git = new Git(db);
			// create file1 on master
			FilePath theFile = WriteTrashFile("file1", "1\n2\n3\n");
			git.Add().AddFilepattern("file1").Call();
			RevCommit firstInMaster = git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			// change first line in master and commit
			WriteTrashFile("file1", "1master\n2\n3\n");
			CheckFile(theFile, "1master\n2\n3\n");
			git.Add().AddFilepattern("file1").Call();
			git.Commit().SetMessage("change file1 in master").Call();
			// create a topic branch based on second commit
			CreateBranch(firstInMaster, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(theFile, "1\n2\n3\n");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file1").Exists());
			// add a line (non-conflicting)
			WriteTrashFile("file1", "1\n2\n3\ntopic4\n");
			git.Add().AddFilepattern("file1").Call();
			git.Commit().SetMessage("add a line to file1 in topic").Call();
			// change first line (conflicting)
			WriteTrashFile("file1", "1topic\n2\n3\ntopic4\n");
			git.Add().AddFilepattern("file1").Call();
			RevCommit conflicting = git.Commit().SetMessage("change file1 in topic").Call();
			// change second line (not conflicting)
			WriteTrashFile("file1", "1topic\n2topic\n3\ntopic4\n");
			git.Add().AddFilepattern("file1").Call();
			RevCommit lastTopicCommit = git.Commit().SetMessage("change file1 in topic again"
				).Call();
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			AssertEquals(conflicting, res.GetCurrentCommit());
			CheckFile(theFile, "<<<<<<< OURS\n1master\n=======\n1topic\n>>>>>>> THEIRS\n2\n3\ntopic4\n"
				);
			NUnit.Framework.Assert.AreEqual(RepositoryState.REBASING_INTERACTIVE, db.GetRepositoryState
				());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "rebase-merge").Exists()
				);
			// the first one should be included, so we should have left two picks in
			// the file
			NUnit.Framework.Assert.AreEqual(2, CountPicks());
			// rebase should not succeed in this state
			try
			{
				git.Rebase().SetUpstream("refs/heads/master").Call();
				NUnit.Framework.Assert.Fail("Expected exception was not thrown");
			}
			catch (WrongRepositoryStateException)
			{
			}
			// expected
			// abort should reset to topic branch
			res = git.Rebase().SetOperation(RebaseCommand.Operation.ABORT).Call();
			NUnit.Framework.Assert.AreEqual(res.GetStatus(), RebaseResult.Status.ABORTED);
			NUnit.Framework.Assert.AreEqual("refs/heads/topic", db.GetFullBranch());
			CheckFile(theFile, "1topic\n2topic\n3\ntopic4\n");
			RevWalk rw = new RevWalk(db);
			AssertEquals(lastTopicCommit, rw.ParseCommit(db.Resolve(Constants.HEAD)));
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
			// rebase- dir in .git must be deleted
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "rebase-merge").Exists(
				));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAbortOnConflictFileCreationAndDeletion()
		{
			Git git = new Git(db);
			// create file1 on master
			WriteTrashFile("file1", "Hello World");
			git.Add().AddFilepattern("file1").Call();
			// create file2 on master
			FilePath file2 = WriteTrashFile("file2", "Hello World 2");
			git.Add().AddFilepattern("file2").Call();
			// create file3 on master
			FilePath file3 = WriteTrashFile("file3", "Hello World 3");
			git.Add().AddFilepattern("file3").Call();
			RevCommit firstInMaster = git.Commit().SetMessage("Add file 1, 2 and 3").Call();
			// create file4 on master
			FilePath file4 = WriteTrashFile("file4", "Hello World 4");
			git.Add().AddFilepattern("file4").Call();
			DeleteTrashFile("file2");
			git.Add().SetUpdate(true).AddFilepattern("file2").Call();
			// create folder folder6 on topic (conflicts with file folder6 on topic
			// later on)
			WriteTrashFile("folder6/file1", "Hello World folder6");
			git.Add().AddFilepattern("folder6/file1").Call();
			git.Commit().SetMessage("Add file 4 and folder folder6, delete file2 on master").
				Call();
			// create a topic branch based on second commit
			CreateBranch(firstInMaster, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			DeleteTrashFile("file3");
			git.Add().SetUpdate(true).AddFilepattern("file3").Call();
			// create file5 on topic
			FilePath file5 = WriteTrashFile("file5", "Hello World 5");
			git.Add().AddFilepattern("file5").Call();
			git.Commit().SetMessage("Delete file3 and add file5 in topic").Call();
			// create file folder6 on topic (conflicts with folder6 on master)
			WriteTrashFile("folder6", "Hello World 6");
			git.Add().AddFilepattern("folder6").Call();
			// create file7 on topic
			FilePath file7 = WriteTrashFile("file7", "Hello World 7");
			git.Add().AddFilepattern("file7").Call();
			DeleteTrashFile("file5");
			git.Add().SetUpdate(true).AddFilepattern("file5").Call();
			RevCommit conflicting = git.Commit().SetMessage("Delete file5, add file folder6 and file7 in topic"
				).Call();
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			AssertEquals(conflicting, res.GetCurrentCommit());
			NUnit.Framework.Assert.AreEqual(RepositoryState.REBASING_INTERACTIVE, db.GetRepositoryState
				());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "rebase-merge").Exists()
				);
			// the first one should be included, so we should have left two picks in
			// the file
			NUnit.Framework.Assert.AreEqual(1, CountPicks());
			NUnit.Framework.Assert.IsFalse(file2.Exists());
			NUnit.Framework.Assert.IsFalse(file3.Exists());
			NUnit.Framework.Assert.IsTrue(file4.Exists());
			NUnit.Framework.Assert.IsFalse(file5.Exists());
			NUnit.Framework.Assert.IsTrue(file7.Exists());
			// abort should reset to topic branch
			res = git.Rebase().SetOperation(RebaseCommand.Operation.ABORT).Call();
			NUnit.Framework.Assert.AreEqual(res.GetStatus(), RebaseResult.Status.ABORTED);
			NUnit.Framework.Assert.AreEqual("refs/heads/topic", db.GetFullBranch());
			RevWalk rw = new RevWalk(db);
			AssertEquals(conflicting, rw.ParseCommit(db.Resolve(Constants.HEAD)));
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
			// rebase- dir in .git must be deleted
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "rebase-merge").Exists(
				));
			NUnit.Framework.Assert.IsTrue(file2.Exists());
			NUnit.Framework.Assert.IsFalse(file3.Exists());
			NUnit.Framework.Assert.IsFalse(file4.Exists());
			NUnit.Framework.Assert.IsFalse(file5.Exists());
			NUnit.Framework.Assert.IsTrue(file7.Exists());
		}

		/// <exception cref="System.IO.IOException"></exception>
		private int CountPicks()
		{
			int count = 0;
			FilePath todoFile = new FilePath(db.Directory, "rebase-merge/git-rebase-todo");
			BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(
				todoFile), "UTF-8"));
			try
			{
				string line = br.ReadLine();
				while (line != null)
				{
					if (line.StartsWith("pick "))
					{
						count++;
					}
					line = br.ReadLine();
				}
				return count;
			}
			finally
			{
				br.Close();
			}
		}
	}
}
