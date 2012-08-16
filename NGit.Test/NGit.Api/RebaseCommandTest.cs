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
using System.Text;
using NGit;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Dircache;
using NGit.Merge;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class RebaseCommandTest : RepositoryTestCase
	{
		private static readonly string FILE1 = "file1";

		protected internal Git git;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			this.git = new Git(db);
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
			// create file1 on master
			WriteTrashFile(FILE1, FILE1);
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit first = git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
			// create a topic branch
			CreateBranch(first, "refs/heads/topic");
			// create file2 on master
			FilePath file2 = WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("Add file2").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			CheckoutBranch("refs/heads/topic");
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file2").Exists());
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			CheckFile(file2, "file2");
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.FAST_FORWARD, res.GetStatus()
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFastForwardWithMultipleCommits()
		{
			// create file1 on master
			WriteTrashFile(FILE1, FILE1);
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit first = git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
			// create a topic branch
			CreateBranch(first, "refs/heads/topic");
			// create file2 on master
			FilePath file2 = WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("Add file2").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			// write a second commit
			WriteTrashFile("file2", "file2 new content");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("Change content of file2").Call();
			CheckoutBranch("refs/heads/topic");
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file2").Exists());
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			CheckFile(file2, "file2 new content");
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.FAST_FORWARD, res.GetStatus()
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseFailsCantCherryPickMergeCommits()
		{
			// create file1 on master
			WriteTrashFile(FILE1, FILE1);
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit first = git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
			// create a topic branch
			CreateBranch(first, "refs/heads/topic");
			// update FILE1 on master
			WriteTrashFile(FILE1, "blah");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("updated file1 on master").Call();
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file3", "more changess");
			git.Add().AddFilepattern("file3").Call();
			RevCommit topicCommit = git.Commit().SetMessage("update file3 on topic").Call();
			// create a branch from the topic commit
			CreateBranch(topicCommit, "refs/heads/side");
			// second commit on topic
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("Add file2").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			// switch to side branch and update file2
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("file3", "more change");
			git.Add().AddFilepattern("file3").Call();
			RevCommit sideCommit = git.Commit().SetMessage("update file2 on side").Call();
			// switch back to topic and merge in side
			CheckoutBranch("refs/heads/topic");
			MergeCommandResult result = git.Merge().Include(sideCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
			try
			{
				RebaseResult rebase = git.Rebase().SetUpstream("refs/heads/master").Call();
				NUnit.Framework.Assert.Fail("MultipleParentsNotAllowedException expected: " + rebase
					.GetStatus());
			}
			catch (JGitInternalException)
			{
			}
		}

		// expected
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseParentOntoHeadShouldBeUptoDate()
		{
			WriteTrashFile(FILE1, FILE1);
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit parent = git.Commit().SetMessage("parent comment").Call();
			WriteTrashFile(FILE1, "another change");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("head commit").Call();
			RebaseResult result = git.Rebase().SetUpstream(parent).Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.UP_TO_DATE, result.GetStatus(
				));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUpToDate()
		{
			// create file1 on master
			WriteTrashFile(FILE1, FILE1);
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit first = git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
			RebaseResult res = git.Rebase().SetUpstream(first).Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.UP_TO_DATE, res.GetStatus());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUnknownUpstream()
		{
			// create file1 on master
			WriteTrashFile(FILE1, FILE1);
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
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
			// create file1 on master
			FilePath theFile = WriteTrashFile(FILE1, "1\n2\n3\n");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit second = git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
			// change first line in master and commit
			WriteTrashFile(FILE1, "1master\n2\n3\n");
			CheckFile(theFile, "1master\n2\n3\n");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit lastMasterChange = git.Commit().SetMessage("change file1 in master").Call
				();
			// create a topic branch based on second commit
			CreateBranch(second, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(theFile, "1\n2\n3\n");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
			// change third line in topic branch
			WriteTrashFile(FILE1, "1\n2\n3\ntopic\n");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit origHead = git.Commit().SetMessage("change file1 in topic").Call();
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, res.GetStatus());
			CheckFile(theFile, "1master\n2\n3\ntopic\n");
			// our old branch should be checked out again
			NUnit.Framework.Assert.AreEqual("refs/heads/topic", db.GetFullBranch());
			NUnit.Framework.Assert.AreEqual(lastMasterChange, new RevWalk(db).ParseCommit(db.
				Resolve(Constants.HEAD)).GetParent(0));
			NUnit.Framework.Assert.AreEqual(origHead, db.ReadOrigHead());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDetachedHead()
		{
			// create file1 on master
			FilePath theFile = WriteTrashFile(FILE1, "1\n2\n3\n");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit second = git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
			// change first line in master and commit
			WriteTrashFile(FILE1, "1master\n2\n3\n");
			CheckFile(theFile, "1master\n2\n3\n");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit lastMasterChange = git.Commit().SetMessage("change file1 in master").Call
				();
			// create a topic branch based on second commit
			CreateBranch(second, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(theFile, "1\n2\n3\n");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
			// change third line in topic branch
			WriteTrashFile(FILE1, "1\n2\n3\ntopic\n");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit topicCommit = git.Commit().SetMessage("change file1 in topic").Call();
			CheckoutBranch("refs/heads/master");
			CheckoutCommit(topicCommit);
			NUnit.Framework.Assert.AreEqual(topicCommit.Id.GetName(), db.GetFullBranch());
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, res.GetStatus());
			CheckFile(theFile, "1master\n2\n3\ntopic\n");
			NUnit.Framework.Assert.AreEqual(lastMasterChange, new RevWalk(db).ParseCommit(db.
				Resolve(Constants.HEAD)).GetParent(0));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFilesAddedFromTwoBranches()
		{
			// create file1 on master
			WriteTrashFile(FILE1, FILE1);
			git.Add().AddFilepattern(FILE1).Call();
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
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file3").Exists());
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/file2").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, res.GetStatus());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file3").Exists());
			// our old branch should be checked out again
			NUnit.Framework.Assert.AreEqual("refs/heads/file3", db.GetFullBranch());
			NUnit.Framework.Assert.AreEqual(addFile2, new RevWalk(db).ParseCommit(db.Resolve(
				Constants.HEAD)).GetParent(0));
			CheckoutBranch("refs/heads/file2");
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			NUnit.Framework.Assert.IsFalse(new FilePath(db.WorkTree, "file3").Exists());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStopOnConflict()
		{
			// create file1 on master
			RevCommit firstInMaster = WriteFileAndCommit(FILE1, "Add file1", "1", "2", "3");
			// change first line in master
			WriteFileAndCommit(FILE1, "change file1 in master", "1master", "2", "3");
			CheckFile(FILE1, "1master", "2", "3");
			// create a topic branch based on second commit
			CreateBranch(firstInMaster, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(FILE1, "1", "2", "3");
			// add a line (non-conflicting)
			WriteFileAndCommit(FILE1, "add a line to file1 in topic", "1", "2", "3", "topic4"
				);
			// change first line (conflicting)
			RevCommit conflicting = WriteFileAndCommit(FILE1, "change file1 in topic", "1topic"
				, "2", "3", "topic4");
			RevCommit lastTopicCommit = WriteFileAndCommit(FILE1, "change file1 in topic again"
				, "1topic", "2", "3", "topic4");
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			NUnit.Framework.Assert.AreEqual(conflicting, res.GetCurrentCommit());
			CheckFile(FILE1, "<<<<<<< OURS\n1master\n=======\n1topic\n>>>>>>> THEIRS\n2\n3\ntopic4"
				);
			NUnit.Framework.Assert.AreEqual(RepositoryState.REBASING_INTERACTIVE, db.GetRepositoryState
				());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "rebase-merge").Exists()
				);
			// the first one should be included, so we should have left two picks in
			// the file
			NUnit.Framework.Assert.AreEqual(1, CountPicks());
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
			CheckFile(FILE1, "1topic", "2", "3", "topic4");
			RevWalk rw = new RevWalk(db);
			NUnit.Framework.Assert.AreEqual(lastTopicCommit, rw.ParseCommit(db.Resolve(Constants
				.HEAD)));
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
			// rebase- dir in .git must be deleted
			NUnit.Framework.Assert.IsFalse(new FilePath(db.Directory, "rebase-merge").Exists(
				));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStopOnConflictAndContinue()
		{
			// create file1 on master
			RevCommit firstInMaster = WriteFileAndCommit(FILE1, "Add file1", "1", "2", "3");
			// change in master
			WriteFileAndCommit(FILE1, "change file1 in master", "1master", "2", "3");
			CheckFile(FILE1, "1master", "2", "3");
			// create a topic branch based on the first commit
			CreateBranch(firstInMaster, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(FILE1, "1", "2", "3");
			// add a line (non-conflicting)
			WriteFileAndCommit(FILE1, "add a line to file1 in topic", "1", "2", "3", "4topic"
				);
			// change first line (conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic\n\nThis is conflicting", "1topic"
				, "2", "3", "4topic");
			// change second line (not conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic again", "1topic", "2topic", "3", 
				"4topic");
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			// continue should throw a meaningful exception
			try
			{
				res = git.Rebase().SetOperation(RebaseCommand.Operation.CONTINUE).Call();
				NUnit.Framework.Assert.Fail("Expected Exception not thrown");
			}
			catch (UnmergedPathsException)
			{
			}
			// expected
			// merge the file; the second topic commit should go through
			WriteFileAndAdd(FILE1, "1topic", "2", "3", "4topic");
			res = git.Rebase().SetOperation(RebaseCommand.Operation.CONTINUE).Call();
			NUnit.Framework.Assert.IsNotNull(res);
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, res.GetStatus());
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
			ObjectId headId = db.Resolve(Constants.HEAD);
			RevWalk rw = new RevWalk(db);
			RevCommit rc = rw.ParseCommit(headId);
			RevCommit parent = rw.ParseCommit(rc.GetParent(0));
			NUnit.Framework.Assert.AreEqual("change file1 in topic\n\nThis is conflicting", parent
				.GetFullMessage());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStopOnConflictAndContinueWithNoDeltaToMaster()
		{
			// create file1 on master
			RevCommit firstInMaster = WriteFileAndCommit(FILE1, "Add file1", "1", "2", "3");
			// change in master
			WriteFileAndCommit(FILE1, "change file1 in master", "1master", "2", "3");
			CheckFile(FILE1, "1master", "2", "3");
			// create a topic branch based on the first commit
			CreateBranch(firstInMaster, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(FILE1, "1", "2", "3");
			// change first line (conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic\n\nThis is conflicting", "1topic"
				, "2", "3", "4topic");
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			// continue should throw a meaningful exception
			try
			{
				res = git.Rebase().SetOperation(RebaseCommand.Operation.CONTINUE).Call();
				NUnit.Framework.Assert.Fail("Expected Exception not thrown");
			}
			catch (UnmergedPathsException)
			{
			}
			// expected
			// merge the file; the second topic commit should go through
			WriteFileAndAdd(FILE1, "1master", "2", "3");
			res = git.Rebase().SetOperation(RebaseCommand.Operation.CONTINUE).Call();
			NUnit.Framework.Assert.IsNotNull(res);
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.NOTHING_TO_COMMIT, res.GetStatus
				());
			NUnit.Framework.Assert.AreEqual(RepositoryState.REBASING_INTERACTIVE, db.GetRepositoryState
				());
			git.Rebase().SetOperation(RebaseCommand.Operation.SKIP).Call();
			ObjectId headId = db.Resolve(Constants.HEAD);
			RevWalk rw = new RevWalk(db);
			RevCommit rc = rw.ParseCommit(headId);
			NUnit.Framework.Assert.AreEqual("change file1 in master", rc.GetFullMessage());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStopOnConflictAndFailContinueIfFileIsDirty()
		{
			// create file1 on master
			RevCommit firstInMaster = WriteFileAndCommit(FILE1, "Add file1", "1", "2", "3");
			// change in master
			WriteFileAndCommit(FILE1, "change file1 in master", "1master", "2", "3");
			CheckFile(FILE1, "1master", "2", "3");
			// create a topic branch based on the first commit
			CreateBranch(firstInMaster, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(FILE1, "1", "2", "3");
			// add a line (non-conflicting)
			WriteFileAndCommit(FILE1, "add a line to file1 in topic", "1", "2", "3", "4topic"
				);
			// change first line (conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic\n\nThis is conflicting", "1topic"
				, "2", "3", "4topic");
			// change second line (not conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic again", "1topic", "2topic", "3", 
				"4topic");
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			git.Add().AddFilepattern(FILE1).Call();
			FilePath trashFile = WriteTrashFile(FILE1, "Some local change");
			res = git.Rebase().SetOperation(RebaseCommand.Operation.CONTINUE).Call();
			NUnit.Framework.Assert.IsNotNull(res);
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			CheckFile(trashFile, "Some local change");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStopOnLastConflictAndContinue()
		{
			// create file1 on master
			RevCommit firstInMaster = WriteFileAndCommit(FILE1, "Add file1", "1", "2", "3");
			// change in master
			WriteFileAndCommit(FILE1, "change file1 in master", "1master", "2", "3");
			CheckFile(FILE1, "1master", "2", "3");
			// create a topic branch based on the first commit
			CreateBranch(firstInMaster, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(FILE1, "1", "2", "3");
			// add a line (non-conflicting)
			WriteFileAndCommit(FILE1, "add a line to file1 in topic", "1", "2", "3", "4topic"
				);
			// change first line (conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic\n\nThis is conflicting", "1topic"
				, "2", "3", "4topic");
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			// merge the file; the second topic commit should go through
			WriteFileAndAdd(FILE1, "1topic", "2", "3", "4topic");
			res = git.Rebase().SetOperation(RebaseCommand.Operation.CONTINUE).Call();
			NUnit.Framework.Assert.IsNotNull(res);
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, res.GetStatus());
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStopOnLastConflictAndSkip()
		{
			// create file1 on master
			RevCommit firstInMaster = WriteFileAndCommit(FILE1, "Add file1", "1", "2", "3");
			// change in master
			WriteFileAndCommit(FILE1, "change file1 in master", "1master", "2", "3");
			CheckFile(FILE1, "1master", "2", "3");
			// create a topic branch based on the first commit
			CreateBranch(firstInMaster, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(FILE1, "1", "2", "3");
			// add a line (non-conflicting)
			WriteFileAndCommit(FILE1, "add a line to file1 in topic", "1", "2", "3", "4topic"
				);
			// change first line (conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic\n\nThis is conflicting", "1topic"
				, "2", "3", "4topic");
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			// merge the file; the second topic commit should go through
			WriteFileAndAdd(FILE1, "1topic", "2", "3", "4topic");
			res = git.Rebase().SetOperation(RebaseCommand.Operation.SKIP).Call();
			NUnit.Framework.Assert.IsNotNull(res);
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, res.GetStatus());
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMergeFirstStopOnLastConflictAndSkip()
		{
			// create file1 on master
			RevCommit firstInMaster = WriteFileAndCommit(FILE1, "Add file1", "1", "2", "3");
			// change in master
			WriteFileAndCommit(FILE1, "change file1 in master", "1master", "2", "3");
			CheckFile(FILE1, "1master", "2", "3");
			// create a topic branch based on the first commit
			CreateBranch(firstInMaster, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(FILE1, "1", "2", "3");
			// add a line (conflicting)
			WriteFileAndCommit(FILE1, "add a line to file1 in topic", "1topic", "2", "3", "4topic"
				);
			// change first line (conflicting again)
			WriteFileAndCommit(FILE1, "change file1 in topic\n\nThis is conflicting", "1topicagain"
				, "2", "3", "4topic");
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			WriteFileAndAdd(FILE1, "merged");
			res = git.Rebase().SetOperation(RebaseCommand.Operation.CONTINUE).Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			res = git.Rebase().SetOperation(RebaseCommand.Operation.SKIP).Call();
			NUnit.Framework.Assert.IsNotNull(res);
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, res.GetStatus());
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
			CheckFile(FILE1, "merged");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStopOnConflictAndSkipNoConflict()
		{
			// create file1 on master
			RevCommit firstInMaster = WriteFileAndCommit(FILE1, "Add file1", "1", "2", "3");
			// change in master
			WriteFileAndCommit(FILE1, "change file1 in master", "1master", "2", "3");
			CheckFile(FILE1, "1master", "2", "3");
			// create a topic branch based on the first commit
			CreateBranch(firstInMaster, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(FILE1, "1", "2", "3");
			// add a line (non-conflicting)
			WriteFileAndCommit(FILE1, "add a line to file1 in topic", "1", "2", "3", "4topic"
				);
			// change first line (conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic\n\nThis is conflicting", "1topic"
				, "2", "3", "4topic");
			// change third line (not conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic again", "1topic", "2", "3topic", 
				"4topic");
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			res = git.Rebase().SetOperation(RebaseCommand.Operation.SKIP).Call();
			CheckFile(FILE1, "1master", "2", "3topic", "4topic");
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, res.GetStatus());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStopOnConflictAndSkipWithConflict()
		{
			// create file1 on master
			RevCommit firstInMaster = WriteFileAndCommit(FILE1, "Add file1", "1", "2", "3", "4"
				);
			// change in master
			WriteFileAndCommit(FILE1, "change file1 in master", "1master", "2", "3master", "4"
				);
			CheckFile(FILE1, "1master", "2", "3master", "4");
			// create a topic branch based on the first commit
			CreateBranch(firstInMaster, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(FILE1, "1", "2", "3", "4");
			// add a line (non-conflicting)
			WriteFileAndCommit(FILE1, "add a line to file1 in topic", "1", "2", "3", "4", "5topic"
				);
			// change first line (conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic\n\nThis is conflicting", "1topic"
				, "2", "3", "4", "5topic");
			// change third line (conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic again", "1topic", "2", "3topic", 
				"4", "5topic");
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			res = git.Rebase().SetOperation(RebaseCommand.Operation.SKIP).Call();
			// TODO is this correct? It is what the command line returns
			CheckFile(FILE1, "1master\n2\n<<<<<<< OURS\n3master\n=======\n3topic\n>>>>>>> THEIRS\n4\n5topic"
				);
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStopOnConflictCommitAndContinue()
		{
			// create file1 on master
			RevCommit firstInMaster = WriteFileAndCommit(FILE1, "Add file1", "1", "2", "3");
			// change in master
			WriteFileAndCommit(FILE1, "change file1 in master", "1master", "2", "3");
			CheckFile(FILE1, "1master", "2", "3");
			// create a topic branch based on the first commit
			CreateBranch(firstInMaster, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			// we have the old content again
			CheckFile(FILE1, "1", "2", "3");
			// add a line (non-conflicting)
			WriteFileAndCommit(FILE1, "add a line to file1 in topic", "1", "2", "3", "4topic"
				);
			// change first line (conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic\n\nThis is conflicting", "1topic"
				, "2", "3", "4topic");
			// change second line (not conflicting)
			WriteFileAndCommit(FILE1, "change file1 in topic again", "1topic", "2", "3topic", 
				"4topic");
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetStatus());
			// continue should throw a meaningful exception
			try
			{
				res = git.Rebase().SetOperation(RebaseCommand.Operation.CONTINUE).Call();
				NUnit.Framework.Assert.Fail("Expected Exception not thrown");
			}
			catch (UnmergedPathsException)
			{
			}
			// expected
			// merge the file; the second topic commit should go through
			WriteFileAndCommit(FILE1, "A different commit message", "1topic", "2", "3", "4topic"
				);
			res = git.Rebase().SetOperation(RebaseCommand.Operation.CONTINUE).Call();
			NUnit.Framework.Assert.IsNotNull(res);
			// nothing to commit. this leaves the repo state in rebase, so that the
			// user can decide what to do. if he accidentally committed, reset soft,
			// and continue, if he really has nothing to commit, skip.
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.NOTHING_TO_COMMIT, res.GetStatus
				());
			NUnit.Framework.Assert.AreEqual(RepositoryState.REBASING_INTERACTIVE, db.GetRepositoryState
				());
			git.Rebase().SetOperation(RebaseCommand.Operation.SKIP).Call();
			ObjectId headId = db.Resolve(Constants.HEAD);
			RevWalk rw = new RevWalk(db);
			RevCommit rc = rw.ParseCommit(headId);
			RevCommit parent = rw.ParseCommit(rc.GetParent(0));
			NUnit.Framework.Assert.AreEqual("A different commit message", parent.GetFullMessage
				());
		}

		/// <exception cref="System.Exception"></exception>
		private RevCommit WriteFileAndCommit(string fileName, string commitMessage, params 
			string[] lines)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string line in lines)
			{
				sb.Append(line);
				sb.Append('\n');
			}
			WriteTrashFile(fileName, sb.ToString());
			git.Add().AddFilepattern(fileName).Call();
			return git.Commit().SetMessage(commitMessage).Call();
		}

		/// <exception cref="System.Exception"></exception>
		private void WriteFileAndAdd(string fileName, params string[] lines)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string line in lines)
			{
				sb.Append(line);
				sb.Append('\n');
			}
			WriteTrashFile(fileName, sb.ToString());
			git.Add().AddFilepattern(fileName).Call();
		}

		/// <exception cref="System.Exception"></exception>
		private void CheckFile(string fileName, params string[] lines)
		{
			FilePath file = new FilePath(db.WorkTree, fileName);
			StringBuilder sb = new StringBuilder();
			foreach (string line in lines)
			{
				sb.Append(line);
				sb.Append('\n');
			}
			RepositoryTestCase.CheckFile(file, sb.ToString());
		}

		private void CheckFile(FilePath f, string text)
		{
			RepositoryTestCase.CheckFile(f, text);
		}
		
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStopOnConflictFileCreationAndDeletion()
		{
			// create file1 on master
			WriteTrashFile(FILE1, "Hello World");
			git.Add().AddFilepattern(FILE1).Call();
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
			NUnit.Framework.Assert.AreEqual(conflicting, res.GetCurrentCommit());
			NUnit.Framework.Assert.AreEqual(RepositoryState.REBASING_INTERACTIVE, db.GetRepositoryState
				());
			NUnit.Framework.Assert.IsTrue(new FilePath(db.Directory, "rebase-merge").Exists()
				);
			// the first one should be included, so we should have left two picks in
			// the file
			NUnit.Framework.Assert.AreEqual(0, CountPicks());
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
			NUnit.Framework.Assert.AreEqual(conflicting, rw.ParseCommit(db.Resolve(Constants.
				HEAD)));
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

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAuthorScriptConverter()
		{
			// -1 h timezone offset
			PersonIdent ident = new PersonIdent("Author name", "a.mail@some.com", 123456789123L
				, -60);
			string convertedAuthor = git.Rebase().ToAuthorScript(ident);
			string[] lines = convertedAuthor.Split("\n");
			NUnit.Framework.Assert.AreEqual("GIT_AUTHOR_NAME='Author name'", lines[0]);
			NUnit.Framework.Assert.AreEqual("GIT_AUTHOR_EMAIL='a.mail@some.com'", lines[1]);
			NUnit.Framework.Assert.AreEqual("GIT_AUTHOR_DATE='@123456789 -0100'", lines[2]);
			PersonIdent parsedIdent = git.Rebase().ParseAuthor(Sharpen.Runtime.GetBytesForString
				(convertedAuthor, "UTF-8"));
			NUnit.Framework.Assert.AreEqual(ident.GetName(), parsedIdent.GetName());
			NUnit.Framework.Assert.AreEqual(ident.GetEmailAddress(), parsedIdent.GetEmailAddress
				());
			// this is rounded to the last second
			NUnit.Framework.Assert.AreEqual(123456789000L, parsedIdent.GetWhen().GetTime());
			NUnit.Framework.Assert.AreEqual(ident.GetTimeZoneOffset(), parsedIdent.GetTimeZoneOffset
				());
			// + 9.5h timezone offset
			ident = new PersonIdent("Author name", "a.mail@some.com", 123456789123L, +570);
			convertedAuthor = git.Rebase().ToAuthorScript(ident);
			lines = convertedAuthor.Split("\n");
			NUnit.Framework.Assert.AreEqual("GIT_AUTHOR_NAME='Author name'", lines[0]);
			NUnit.Framework.Assert.AreEqual("GIT_AUTHOR_EMAIL='a.mail@some.com'", lines[1]);
			NUnit.Framework.Assert.AreEqual("GIT_AUTHOR_DATE='@123456789 +0930'", lines[2]);
			parsedIdent = git.Rebase().ParseAuthor(Sharpen.Runtime.GetBytesForString(convertedAuthor
				, "UTF-8"));
			NUnit.Framework.Assert.AreEqual(ident.GetName(), parsedIdent.GetName());
			NUnit.Framework.Assert.AreEqual(ident.GetEmailAddress(), parsedIdent.GetEmailAddress
				());
			NUnit.Framework.Assert.AreEqual(123456789000L, parsedIdent.GetWhen().GetTime());
			NUnit.Framework.Assert.AreEqual(ident.GetTimeZoneOffset(), parsedIdent.GetTimeZoneOffset
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRepositoryStateChecks()
		{
			try
			{
				git.Rebase().SetOperation(RebaseCommand.Operation.ABORT).Call();
				NUnit.Framework.Assert.Fail("Expected Exception not thrown");
			}
			catch (WrongRepositoryStateException)
			{
			}
			// expected
			try
			{
				git.Rebase().SetOperation(RebaseCommand.Operation.SKIP).Call();
				NUnit.Framework.Assert.Fail("Expected Exception not thrown");
			}
			catch (WrongRepositoryStateException)
			{
			}
			// expected
			try
			{
				git.Rebase().SetOperation(RebaseCommand.Operation.CONTINUE).Call();
				NUnit.Framework.Assert.Fail("Expected Exception not thrown");
			}
			catch (WrongRepositoryStateException)
			{
			}
		}

		// expected
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseWithUntrackedFile()
		{
			// create file1, add and commit
			WriteTrashFile(FILE1, "file1");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit commit = git.Commit().SetMessage("commit1").Call();
			// create topic branch and checkout / create file2, add and commit
			CreateBranch(commit, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("commit2").Call();
			// checkout master branch / modify file1, add and commit
			CheckoutBranch("refs/heads/master");
			WriteTrashFile(FILE1, "modified file1");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("commit3").Call();
			// checkout topic branch / create untracked file3
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file3", "untracked file3");
			// rebase
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, git.Rebase().SetUpstream(
				"refs/heads/master").Call().GetStatus());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseWithUnstagedTopicChange()
		{
			// create file1, add and commit
			WriteTrashFile(FILE1, "file1");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit commit = git.Commit().SetMessage("commit1").Call();
			// create topic branch and checkout / create file2, add and commit
			CreateBranch(commit, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("commit2").Call();
			// checkout master branch / modify file1, add and commit
			CheckoutBranch("refs/heads/master");
			WriteTrashFile(FILE1, "modified file1");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("commit3").Call();
			// checkout topic branch / modify file2
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file2", "unstaged file2");
			// rebase
			JGitInternalException exception = null;
			try
			{
				git.Rebase().SetUpstream("refs/heads/master").Call();
			}
			catch (JGitInternalException e)
			{
				exception = e;
			}
			NUnit.Framework.Assert.IsNotNull(exception);
			NUnit.Framework.Assert.AreEqual("Checkout conflict with files: \nfile2", exception
				.Message);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseWithUncommittedTopicChange()
		{
			// create file1, add and commit
			WriteTrashFile(FILE1, "file1");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit commit = git.Commit().SetMessage("commit1").Call();
			// create topic branch and checkout / create file2, add and commit
			CreateBranch(commit, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("commit2").Call();
			// checkout master branch / modify file1, add and commit
			CheckoutBranch("refs/heads/master");
			WriteTrashFile(FILE1, "modified file1");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("commit3").Call();
			// checkout topic branch / modify file2 and add
			CheckoutBranch("refs/heads/topic");
			FilePath uncommittedFile = WriteTrashFile("file2", "uncommitted file2");
			git.Add().AddFilepattern("file2").Call();
			// do not commit
			// rebase
			JGitInternalException exception = null;
			try
			{
				git.Rebase().SetUpstream("refs/heads/master").Call();
			}
			catch (JGitInternalException e)
			{
				exception = e;
			}
			NUnit.Framework.Assert.IsNotNull(exception);
			NUnit.Framework.Assert.AreEqual("Checkout conflict with files: \nfile2", exception
				.Message);
			CheckFile(uncommittedFile, "uncommitted file2");
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, git.GetRepository().GetRepositoryState
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseWithUnstagedMasterChange()
		{
			// create file1, add and commit
			WriteTrashFile(FILE1, "file1");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit commit = git.Commit().SetMessage("commit1").Call();
			// create topic branch and checkout / create file2, add and commit
			CreateBranch(commit, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("commit2").Call();
			// checkout master branch / modify file1, add and commit
			CheckoutBranch("refs/heads/master");
			WriteTrashFile(FILE1, "modified file1");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("commit3").Call();
			// checkout topic branch / modify file1
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile(FILE1, "unstaged modified file1");
			// rebase
			JGitInternalException exception = null;
			try
			{
				git.Rebase().SetUpstream("refs/heads/master").Call();
			}
			catch (JGitInternalException e)
			{
				exception = e;
			}
			NUnit.Framework.Assert.IsNotNull(exception);
			NUnit.Framework.Assert.AreEqual("Checkout conflict with files: \nfile1", exception
				.Message);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseWithUncommittedMasterChange()
		{
			// create file1, add and commit
			WriteTrashFile(FILE1, "file1");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit commit = git.Commit().SetMessage("commit1").Call();
			// create topic branch and checkout / create file2, add and commit
			CreateBranch(commit, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("commit2").Call();
			// checkout master branch / modify file1, add and commit
			CheckoutBranch("refs/heads/master");
			WriteTrashFile(FILE1, "modified file1");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("commit3").Call();
			// checkout topic branch / modify file1 and add
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile(FILE1, "uncommitted modified file1");
			git.Add().AddFilepattern(FILE1).Call();
			// do not commit
			// rebase
			JGitInternalException exception = null;
			try
			{
				git.Rebase().SetUpstream("refs/heads/master").Call();
			}
			catch (JGitInternalException e)
			{
				exception = e;
			}
			NUnit.Framework.Assert.IsNotNull(exception);
			NUnit.Framework.Assert.AreEqual("Checkout conflict with files: \nfile1", exception
				.Message);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseWithUnstagedMasterChangeBaseCommit()
		{
			// create file0 + file1, add and commit
			WriteTrashFile("file0", "file0");
			WriteTrashFile(FILE1, "file1");
			git.Add().AddFilepattern("file0").AddFilepattern(FILE1).Call();
			RevCommit commit = git.Commit().SetMessage("commit1").Call();
			// create topic branch and checkout / create file2, add and commit
			CreateBranch(commit, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("commit2").Call();
			// checkout master branch / modify file1, add and commit
			CheckoutBranch("refs/heads/master");
			WriteTrashFile(FILE1, "modified file1");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("commit3").Call();
			// checkout topic branch / modify file0
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file0", "unstaged modified file0");
			// rebase
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, git.Rebase().SetUpstream(
				"refs/heads/master").Call().GetStatus());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseWithUncommittedMasterChangeBaseCommit()
		{
			// create file0 + file1, add and commit
			FilePath file0 = WriteTrashFile("file0", "file0");
			WriteTrashFile(FILE1, "file1");
			git.Add().AddFilepattern("file0").AddFilepattern(FILE1).Call();
			RevCommit commit = git.Commit().SetMessage("commit1").Call();
			// create topic branch and checkout / create file2, add and commit
			CreateBranch(commit, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("commit2").Call();
			// checkout master branch / modify file1, add and commit
			CheckoutBranch("refs/heads/master");
			WriteTrashFile(FILE1, "modified file1");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("commit3").Call();
			// checkout topic branch / modify file0 and add
			CheckoutBranch("refs/heads/topic");
			Write(file0, "unstaged modified file0");
			git.Add().AddFilepattern("file0").Call();
			// do not commit
			// get current index state
			string indexState = IndexState(CONTENT);
			// rebase
			RebaseResult result = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.FAILED, result.GetStatus());
			// staged file0 causes DIRTY_INDEX
			NUnit.Framework.Assert.AreEqual(1, result.GetFailingPaths().Count);
			NUnit.Framework.Assert.AreEqual(ResolveMerger.MergeFailureReason.DIRTY_INDEX, result
				.GetFailingPaths().Get("file0"));
			NUnit.Framework.Assert.AreEqual("unstaged modified file0", Read(file0));
			// index shall be unchanged
			NUnit.Framework.Assert.AreEqual(indexState, IndexState(CONTENT));
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseWithUnstagedMasterChangeOtherCommit()
		{
			// create file0, add and commit
			WriteTrashFile("file0", "file0");
			git.Add().AddFilepattern("file0").Call();
			git.Commit().SetMessage("commit0").Call();
			// create file1, add and commit
			WriteTrashFile(FILE1, "file1");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit commit = git.Commit().SetMessage("commit1").Call();
			// create topic branch and checkout / create file2, add and commit
			CreateBranch(commit, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("commit2").Call();
			// checkout master branch / modify file1, add and commit
			CheckoutBranch("refs/heads/master");
			WriteTrashFile(FILE1, "modified file1");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("commit3").Call();
			// checkout topic branch / modify file0
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file0", "unstaged modified file0");
			// rebase
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.OK, git.Rebase().SetUpstream(
				"refs/heads/master").Call().GetStatus());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseWithUncommittedMasterChangeOtherCommit()
		{
			// create file0, add and commit
			FilePath file0 = WriteTrashFile("file0", "file0");
			git.Add().AddFilepattern("file0").Call();
			git.Commit().SetMessage("commit0").Call();
			// create file1, add and commit
			WriteTrashFile(FILE1, "file1");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit commit = git.Commit().SetMessage("commit1").Call();
			// create topic branch and checkout / create file2, add and commit
			CreateBranch(commit, "refs/heads/topic");
			CheckoutBranch("refs/heads/topic");
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			git.Commit().SetMessage("commit2").Call();
			// checkout master branch / modify file1, add and commit
			CheckoutBranch("refs/heads/master");
			WriteTrashFile(FILE1, "modified file1");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("commit3").Call();
			// checkout topic branch / modify file0 and add
			CheckoutBranch("refs/heads/topic");
			Write(file0, "unstaged modified file0");
			git.Add().AddFilepattern("file0").Call();
			// do not commit
			// get current index state
			string indexState = IndexState(CONTENT);
			// rebase
			RebaseResult result = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.FAILED, result.GetStatus());
			// staged file0 causes DIRTY_INDEX
			NUnit.Framework.Assert.AreEqual(1, result.GetFailingPaths().Count);
			NUnit.Framework.Assert.AreEqual(ResolveMerger.MergeFailureReason.DIRTY_INDEX, result
				.GetFailingPaths().Get("file0"));
			NUnit.Framework.Assert.AreEqual("unstaged modified file0", Read(file0));
			// index shall be unchanged
			NUnit.Framework.Assert.AreEqual(indexState, IndexState(CONTENT));
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, db.GetRepositoryState());
		}

		/// <exception cref="System.IO.IOException"></exception>
		private int CountPicks()
		{
			int count = 0;
			FilePath todoFile = GetTodoFile();
			BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(
				todoFile), "UTF-8"));
			try
			{
				string line = br.ReadLine();
				while (line != null)
				{
					string actionToken = Sharpen.Runtime.Substring(line, 0, line.IndexOf(' '));
					RebaseCommand.Action action = null;
					try
					{
						action = RebaseCommand.Action.Parse(actionToken);
					}
					catch (Exception)
					{
					}
					// ignore
					if (action != null)
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

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFastForwardWithMultipleCommitsOnDifferentBranches()
		{
			// create file1 on master
			WriteTrashFile(FILE1, FILE1);
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit first = git.Commit().SetMessage("Add file1").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, FILE1).Exists());
			// create a topic branch
			CreateBranch(first, "refs/heads/topic");
			// create file2 on master
			WriteTrashFile("file2", "file2");
			git.Add().AddFilepattern("file2").Call();
			RevCommit second = git.Commit().SetMessage("Add file2").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			// create side branch
			CreateBranch(second, "refs/heads/side");
			// update FILE1 on master
			WriteTrashFile(FILE1, "blah");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("updated file1 on master").Call();
			// switch to side branch and update file2
			CheckoutBranch("refs/heads/side");
			WriteTrashFile("file2", "more change");
			git.Add().AddFilepattern("file2").Call();
			RevCommit fourth = git.Commit().SetMessage("update file2 on side").Call();
			// switch back to master and merge in side
			CheckoutBranch("refs/heads/master");
			MergeCommandResult result = git.Merge().Include(fourth.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
			// switch back to topic branch and rebase it onto master
			CheckoutBranch("refs/heads/topic");
			RebaseResult res = git.Rebase().SetUpstream("refs/heads/master").Call();
			NUnit.Framework.Assert.IsTrue(new FilePath(db.WorkTree, "file2").Exists());
			CheckFile(new FilePath(db.WorkTree, "file2"), "more change");
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.FAST_FORWARD, res.GetStatus()
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseShouldLeaveWorkspaceUntouchedWithUnstagedChangesConflict
			()
		{
			WriteTrashFile(FILE1, "initial file");
			git.Add().AddFilepattern(FILE1).Call();
			RevCommit initial = git.Commit().SetMessage("initial commit").Call();
			CreateBranch(initial, "refs/heads/side");
			WriteTrashFile(FILE1, "updated file");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("updated FILE1 on master").Call();
			// switch to side, modify the file
			CheckoutBranch("refs/heads/side");
			WriteTrashFile(FILE1, "side update");
			git.Add().AddFilepattern(FILE1).Call();
			git.Commit().SetMessage("updated FILE1 on side").Call();
			FilePath theFile = WriteTrashFile(FILE1, "dirty the file");
			// and attempt to rebase
			try
			{
				RebaseResult rebaseResult = git.Rebase().SetUpstream("refs/heads/master").Call();
				NUnit.Framework.Assert.Fail("Checkout with conflict should have occured, not " + 
					rebaseResult.GetStatus());
			}
			catch (JGitInternalException)
			{
				CheckFile(theFile, "dirty the file");
			}
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, git.GetRepository().GetRepositoryState
				());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRebaseShouldBeAbleToHandleEmptyLinesInRebaseTodoFile()
		{
			string emptyLine = "\n";
			string todo = "pick 1111111 Commit 1\n" + emptyLine + "pick 2222222 Commit 2\n" +
				 emptyLine + "# Comment line at end\n";
			Write(GetTodoFile(), todo);
			RebaseCommand rebaseCommand = git.Rebase();
			IList<RebaseCommand.Step> steps = rebaseCommand.LoadSteps();
			NUnit.Framework.Assert.AreEqual(2, steps.Count);
			NUnit.Framework.Assert.AreEqual("1111111", steps[0].commit.Name);
			NUnit.Framework.Assert.AreEqual("2222222", steps[1].commit.Name);
		}

		private FilePath GetTodoFile()
		{
			FilePath todoFile = new FilePath(db.Directory, "rebase-merge/git-rebase-todo");
			return todoFile;
		}
	}
}
