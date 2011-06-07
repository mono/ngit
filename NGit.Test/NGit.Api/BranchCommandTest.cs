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
using NGit;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Revwalk;
using NGit.Transport;
using Sharpen;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class BranchCommandTest : RepositoryTestCase
	{
		private Git git;

		internal RevCommit initialCommit;

		internal RevCommit secondCommit;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			git = new Git(db);
			// checkout master
			git.Commit().SetMessage("initial commit").Call();
			// commit something
			WriteTrashFile("Test.txt", "Hello world");
			git.Add().AddFilepattern("Test.txt").Call();
			initialCommit = git.Commit().SetMessage("Initial commit").Call();
			WriteTrashFile("Test.txt", "Some change");
			git.Add().AddFilepattern("Test.txt").Call();
			secondCommit = git.Commit().SetMessage("Second commit").Call();
			// create a master branch
			RefUpdate rup = db.UpdateRef("refs/heads/master");
			rup.SetNewObjectId(initialCommit.Id);
			rup.SetForceUpdate(true);
			rup.Update();
		}

		/// <exception cref="System.Exception"></exception>
		private Git SetUpRepoWithRemote()
		{
			Repository remoteRepository = CreateWorkRepository();
			Git remoteGit = new Git(remoteRepository);
			// commit something
			WriteTrashFile("Test.txt", "Hello world");
			remoteGit.Add().AddFilepattern("Test.txt").Call();
			initialCommit = remoteGit.Commit().SetMessage("Initial commit").Call();
			WriteTrashFile("Test.txt", "Some change");
			remoteGit.Add().AddFilepattern("Test.txt").Call();
			secondCommit = remoteGit.Commit().SetMessage("Second commit").Call();
			// create a master branch
			RefUpdate rup = remoteRepository.UpdateRef("refs/heads/master");
			rup.SetNewObjectId(initialCommit.Id);
			rup.ForceUpdate();
			Repository localRepository = CreateWorkRepository();
			Git localGit = new Git(localRepository);
			StoredConfig config = localRepository.GetConfig();
			RemoteConfig rc = new RemoteConfig(config, "origin");
			rc.AddURI(new URIish(remoteRepository.Directory.GetPath()));
			rc.AddFetchRefSpec(new RefSpec("+refs/heads/*:refs/remotes/origin/*"));
			rc.Update(config);
			config.Save();
			FetchResult res = localGit.Fetch().SetRemote("origin").Call();
			NUnit.Framework.Assert.IsFalse(res.GetTrackingRefUpdates().IsEmpty());
			rup = localRepository.UpdateRef("refs/heads/master");
			rup.SetNewObjectId(initialCommit.Id);
			rup.ForceUpdate();
			rup = localRepository.UpdateRef(Constants.HEAD);
			rup.Link("refs/heads/master");
			rup.SetNewObjectId(initialCommit.Id);
			rup.Update();
			return localGit;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCreateAndList()
		{
			int localBefore;
			int remoteBefore;
			int allBefore;
			// invalid name not allowed
			try
			{
				git.BranchCreate().SetName("In va lid").Call();
				NUnit.Framework.Assert.Fail("Create branch with invalid ref name should fail");
			}
			catch (InvalidRefNameException)
			{
			}
			// expected
			// existing name not allowed w/o force
			try
			{
				git.BranchCreate().SetName("master").Call();
				NUnit.Framework.Assert.Fail("Create branch with existing ref name should fail");
			}
			catch (RefAlreadyExistsException)
			{
			}
			// expected
			localBefore = git.BranchList().Call().Count;
			remoteBefore = git.BranchList().SetListMode(ListBranchCommand.ListMode.REMOTE).Call
				().Count;
			allBefore = git.BranchList().SetListMode(ListBranchCommand.ListMode.ALL).Call().Count;
			NUnit.Framework.Assert.AreEqual(localBefore + remoteBefore, allBefore);
			Ref newBranch = CreateBranch(git, "NewForTestList", false, "master", null);
			NUnit.Framework.Assert.AreEqual("refs/heads/NewForTestList", newBranch.GetName());
			NUnit.Framework.Assert.AreEqual(1, git.BranchList().Call().Count - localBefore);
			NUnit.Framework.Assert.AreEqual(0, git.BranchList().SetListMode(ListBranchCommand.ListMode
				.REMOTE).Call().Count - remoteBefore);
			NUnit.Framework.Assert.AreEqual(1, git.BranchList().SetListMode(ListBranchCommand.ListMode
				.ALL).Call().Count - allBefore);
			// we can only create local branches
			newBranch = CreateBranch(git, "refs/remotes/origin/NewRemoteForTestList", false, 
				"master", null);
			NUnit.Framework.Assert.AreEqual("refs/heads/refs/remotes/origin/NewRemoteForTestList"
				, newBranch.GetName());
			NUnit.Framework.Assert.AreEqual(2, git.BranchList().Call().Count - localBefore);
			NUnit.Framework.Assert.AreEqual(0, git.BranchList().SetListMode(ListBranchCommand.ListMode
				.REMOTE).Call().Count - remoteBefore);
			NUnit.Framework.Assert.AreEqual(2, git.BranchList().SetListMode(ListBranchCommand.ListMode
				.ALL).Call().Count - allBefore);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestListAllBranchesShouldNotDie()
		{
			Git git = SetUpRepoWithRemote();
			git.BranchList().SetListMode(ListBranchCommand.ListMode.ALL).Call();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCreateFromCommit()
		{
			Ref branch = git.BranchCreate().SetName("FromInitial").SetStartPoint(initialCommit
				).Call();
			NUnit.Framework.Assert.AreEqual(initialCommit.Id, branch.GetObjectId());
			branch = git.BranchCreate().SetName("FromInitial2").SetStartPoint(initialCommit.Id
				.Name).Call();
			NUnit.Framework.Assert.AreEqual(initialCommit.Id, branch.GetObjectId());
			try
			{
				git.BranchCreate().SetName("FromInitial").SetStartPoint(secondCommit).Call();
			}
			catch (RefAlreadyExistsException)
			{
			}
			// expected
			branch = git.BranchCreate().SetName("FromInitial").SetStartPoint(secondCommit).SetForce
				(true).Call();
			NUnit.Framework.Assert.AreEqual(secondCommit.Id, branch.GetObjectId());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCreateForce()
		{
			// using commits
			Ref newBranch = CreateBranch(git, "NewForce", false, secondCommit.Id.Name, null);
			NUnit.Framework.Assert.AreEqual(newBranch.GetTarget().GetObjectId(), secondCommit
				.Id);
			try
			{
				newBranch = CreateBranch(git, "NewForce", false, initialCommit.Id.Name, null);
				NUnit.Framework.Assert.Fail("Should have failed");
			}
			catch (RefAlreadyExistsException)
			{
			}
			// expected
			newBranch = CreateBranch(git, "NewForce", true, initialCommit.Id.Name, null);
			NUnit.Framework.Assert.AreEqual(newBranch.GetTarget().GetObjectId(), initialCommit
				.Id);
			git.BranchDelete().SetBranchNames("NewForce").Call();
			// using names
			git.BranchCreate().SetName("NewForce").SetStartPoint("master").Call();
			NUnit.Framework.Assert.AreEqual(newBranch.GetTarget().GetObjectId(), initialCommit
				.Id);
			try
			{
				git.BranchCreate().SetName("NewForce").SetStartPoint("master").Call();
				NUnit.Framework.Assert.Fail("Should have failed");
			}
			catch (RefAlreadyExistsException)
			{
			}
			// expected
			git.BranchCreate().SetName("NewForce").SetStartPoint("master").SetForce(true).Call
				();
			NUnit.Framework.Assert.AreEqual(newBranch.GetTarget().GetObjectId(), initialCommit
				.Id);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDelete()
		{
			CreateBranch(git, "ForDelete", false, "master", null);
			git.BranchDelete().SetBranchNames("ForDelete").Call();
			// now point the branch to a non-merged commit
			CreateBranch(git, "ForDelete", false, secondCommit.Id.Name, null);
			try
			{
				git.BranchDelete().SetBranchNames("ForDelete").Call();
				NUnit.Framework.Assert.Fail("Deletion of a non-merged branch without force should have failed"
					);
			}
			catch (NotMergedException)
			{
			}
			// expected
			IList<string> deleted = git.BranchDelete().SetBranchNames("ForDelete").SetForce(true
				).Call();
			NUnit.Framework.Assert.AreEqual(1, deleted.Count);
			NUnit.Framework.Assert.AreEqual(Constants.R_HEADS + "ForDelete", deleted[0]);
			CreateBranch(git, "ForDelete", false, "master", null);
			try
			{
				CreateBranch(git, "ForDelete", false, "master", null);
				NUnit.Framework.Assert.Fail("Repeated creation of same branch without force should fail"
					);
			}
			catch (RefAlreadyExistsException)
			{
			}
			// expected
			// change starting point
			Ref newBranch = CreateBranch(git, "ForDelete", true, initialCommit.Name, null);
			NUnit.Framework.Assert.AreEqual(newBranch.GetTarget().GetObjectId(), initialCommit
				.Id);
			newBranch = CreateBranch(git, "ForDelete", true, secondCommit.Name, null);
			NUnit.Framework.Assert.AreEqual(newBranch.GetTarget().GetObjectId(), secondCommit
				.Id);
			git.BranchDelete().SetBranchNames("ForDelete").SetForce(true);
			try
			{
				git.BranchDelete().SetBranchNames("master").Call();
				NUnit.Framework.Assert.Fail("Deletion of checked out branch without force should have failed"
					);
			}
			catch (CannotDeleteCurrentBranchException)
			{
			}
			// expected
			try
			{
				git.BranchDelete().SetBranchNames("master").SetForce(true).Call();
				NUnit.Framework.Assert.Fail("Deletion of checked out branch with force should have failed"
					);
			}
			catch (CannotDeleteCurrentBranchException)
			{
			}
		}

		// expected
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPullConfigRemoteBranch()
		{
			Git localGit = SetUpRepoWithRemote();
			Ref remote = localGit.BranchList().SetListMode(ListBranchCommand.ListMode.REMOTE)
				.Call()[0];
			NUnit.Framework.Assert.AreEqual("refs/remotes/origin/master", remote.GetName());
			// by default, we should create pull configuration
			CreateBranch(localGit, "newFromRemote", false, remote.GetName(), null);
			NUnit.Framework.Assert.AreEqual("origin", localGit.GetRepository().GetConfig().GetString
				("branch", "newFromRemote", "remote"));
			localGit.BranchDelete().SetBranchNames("newFromRemote").Call();
			// the pull configuration should be gone after deletion
			NUnit.Framework.Assert.IsNull(localGit.GetRepository().GetConfig().GetString("branch"
				, "newFromRemote", "remote"));
			CreateBranch(localGit, "newFromRemote", false, remote.GetName(), null);
			NUnit.Framework.Assert.AreEqual("origin", localGit.GetRepository().GetConfig().GetString
				("branch", "newFromRemote", "remote"));
			localGit.BranchDelete().SetBranchNames("refs/heads/newFromRemote").Call();
			// the pull configuration should be gone after deletion
			NUnit.Framework.Assert.IsNull(localGit.GetRepository().GetConfig().GetString("branch"
				, "newFromRemote", "remote"));
			// use --no-track
			CreateBranch(localGit, "newFromRemote", false, remote.GetName(), CreateBranchCommand.SetupUpstreamMode
				.NOTRACK);
			NUnit.Framework.Assert.IsNull(localGit.GetRepository().GetConfig().GetString("branch"
				, "newFromRemote", "remote"));
			localGit.BranchDelete().SetBranchNames("newFromRemote").Call();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPullConfigLocalBranch()
		{
			Git localGit = SetUpRepoWithRemote();
			// by default, we should not create pull configuration
			CreateBranch(localGit, "newFromMaster", false, "master", null);
			NUnit.Framework.Assert.IsNull(localGit.GetRepository().GetConfig().GetString("branch"
				, "newFromMaster", "remote"));
			localGit.BranchDelete().SetBranchNames("newFromMaster").Call();
			// use --track
			CreateBranch(localGit, "newFromMaster", false, "master", CreateBranchCommand.SetupUpstreamMode
				.TRACK);
			NUnit.Framework.Assert.AreEqual(".", localGit.GetRepository().GetConfig().GetString
				("branch", "newFromMaster", "remote"));
			localGit.BranchDelete().SetBranchNames("refs/heads/newFromMaster").Call();
			// the pull configuration should be gone after deletion
			NUnit.Framework.Assert.IsNull(localGit.GetRepository().GetConfig().GetString("branch"
				, "newFromRemote", "remote"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPullConfigRenameLocalBranch()
		{
			Git localGit = SetUpRepoWithRemote();
			// by default, we should not create pull configuration
			CreateBranch(localGit, "newFromMaster", false, "master", null);
			NUnit.Framework.Assert.IsNull(localGit.GetRepository().GetConfig().GetString("branch"
				, "newFromMaster", "remote"));
			localGit.BranchDelete().SetBranchNames("newFromMaster").Call();
			// use --track
			CreateBranch(localGit, "newFromMaster", false, "master", CreateBranchCommand.SetupUpstreamMode
				.TRACK);
			NUnit.Framework.Assert.AreEqual(".", localGit.GetRepository().GetConfig().GetString
				("branch", "newFromMaster", "remote"));
			localGit.BranchRename().SetOldName("newFromMaster").SetNewName("renamed").Call();
			NUnit.Framework.Assert.IsNull(localGit.GetRepository().GetConfig().GetString("branch"
				, "newFromMaster", "remote"), ".");
			NUnit.Framework.Assert.AreEqual(".", localGit.GetRepository().GetConfig().GetString
				("branch", "renamed", "remote"));
			localGit.BranchDelete().SetBranchNames("renamed").Call();
			// the pull configuration should be gone after deletion
			NUnit.Framework.Assert.IsNull(localGit.GetRepository().GetConfig().GetString("branch"
				, "newFromRemote", "remote"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameLocalBranch()
		{
			// null newName not allowed
			try
			{
				git.BranchRename().Call();
			}
			catch (InvalidRefNameException)
			{
			}
			// expected
			// invalid newName not allowed
			try
			{
				git.BranchRename().SetNewName("In va lid").Call();
			}
			catch (InvalidRefNameException)
			{
			}
			// expected
			// not existing name not allowed
			try
			{
				git.BranchRename().SetOldName("notexistingbranch").SetNewName("newname").Call();
			}
			catch (RefNotFoundException)
			{
			}
			// expected
			// create some branch
			CreateBranch(git, "existing", false, "master", null);
			// a local branch
			Ref branch = CreateBranch(git, "fromMasterForRename", false, "master", null);
			NUnit.Framework.Assert.AreEqual(Constants.R_HEADS + "fromMasterForRename", branch
				.GetName());
			Ref renamed = git.BranchRename().SetOldName("fromMasterForRename").SetNewName("newName"
				).Call();
			NUnit.Framework.Assert.AreEqual(Constants.R_HEADS + "newName", renamed.GetName());
			try
			{
				git.BranchRename().SetOldName(renamed.GetName()).SetNewName("existing").Call();
				NUnit.Framework.Assert.Fail("Should have failed");
			}
			catch (RefAlreadyExistsException)
			{
			}
			// expected
			try
			{
				git.BranchRename().SetNewName("In va lid").Call();
				NUnit.Framework.Assert.Fail("Rename with invalid ref name should fail");
			}
			catch (InvalidRefNameException)
			{
			}
			// expected
			// rename without old name and detached head not allowed
			RefUpdate rup = git.GetRepository().UpdateRef(Constants.HEAD, true);
			rup.SetNewObjectId(initialCommit);
			rup.ForceUpdate();
			try
			{
				git.BranchRename().SetNewName("detached").Call();
			}
			catch (DetachedHeadException)
			{
			}
		}

		// expected
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRenameRemoteTrackingBranch()
		{
			Git localGit = SetUpRepoWithRemote();
			Ref remoteBranch = localGit.BranchList().SetListMode(ListBranchCommand.ListMode.REMOTE
				).Call()[0];
			Ref renamed = localGit.BranchRename().SetOldName(remoteBranch.GetName()).SetNewName
				("newRemote").Call();
			NUnit.Framework.Assert.AreEqual(Constants.R_REMOTES + "newRemote", renamed.GetName
				());
		}

		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCreationImplicitStart()
		{
			git.BranchCreate().SetName("topic").Call();
		}

		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="NGit.Api.Errors.RefAlreadyExistsException"></exception>
		/// <exception cref="NGit.Api.Errors.RefNotFoundException"></exception>
		/// <exception cref="NGit.Api.Errors.InvalidRefNameException"></exception>
		public virtual Ref CreateBranch(Git actGit, string name, bool force, string startPoint
			, CreateBranchCommand.SetupUpstreamMode? mode)
		{
			CreateBranchCommand cmd = actGit.BranchCreate();
			cmd.SetName(name);
			cmd.SetForce(force);
			cmd.SetStartPoint(startPoint);
			cmd.SetUpstreamMode(mode != null ? mode.Value : CreateBranchCommand.SetupUpstreamMode.NOT_SET);
			return cmd.Call();
		}
	}
}
