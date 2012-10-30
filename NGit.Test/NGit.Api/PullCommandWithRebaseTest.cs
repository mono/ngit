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
using NGit.Merge;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Transport;
using Sharpen;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class PullCommandWithRebaseTest : RepositoryTestCase
	{
		/// <summary>Second Test repository</summary>
		protected internal FileRepository dbTarget;

		private Git source;

		private Git target;

		private FilePath sourceFile;

		private FilePath targetFile;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPullFastForward()
		{
			PullResult res = target.Pull().Call();
			// nothing to update since we don't have different data yet
			NUnit.Framework.Assert.IsTrue(res.GetFetchResult().GetTrackingRefUpdates().IsEmpty
				());
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.UP_TO_DATE, res.GetRebaseResult
				().GetStatus());
			AssertFileContentsEqual(targetFile, "Hello world");
			// change the source file
			WriteToFile(sourceFile, "Another change");
			source.Add().AddFilepattern("SomeFile.txt").Call();
			source.Commit().SetMessage("Some change in remote").Call();
			res = target.Pull().Call();
			NUnit.Framework.Assert.IsFalse(res.GetFetchResult().GetTrackingRefUpdates().IsEmpty
				());
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.FAST_FORWARD, res.GetRebaseResult
				().GetStatus());
			AssertFileContentsEqual(targetFile, "Another change");
			NUnit.Framework.Assert.AreEqual(RepositoryState.SAFE, target.GetRepository().GetRepositoryState
				());
			res = target.Pull().Call();
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.UP_TO_DATE, res.GetRebaseResult
				().GetStatus());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPullFastForwardWithBranchInSource()
		{
			PullResult res = target.Pull().Call();
			// nothing to update since we don't have different data yet
			NUnit.Framework.Assert.IsTrue(res.GetFetchResult().GetTrackingRefUpdates().IsEmpty
				());
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.UP_TO_DATE, res.GetRebaseResult
				().GetStatus());
			AssertFileContentsEqual(targetFile, "Hello world");
			// change the source file
			WriteToFile(sourceFile, "Another change\n\n\n\nFoo");
			source.Add().AddFilepattern("SomeFile.txt").Call();
			RevCommit initialCommit = source.Commit().SetMessage("Some change in remote").Call
				();
			// modify the source file in a branch
			CreateBranch(initialCommit, "refs/heads/side");
			CheckoutBranch("refs/heads/side");
			WriteToFile(sourceFile, "Another change\n\n\n\nBoo");
			source.Add().AddFilepattern("SomeFile.txt").Call();
			RevCommit sideCommit = source.Commit().SetMessage("Some change in remote").Call();
			// modify the source file on master
			CheckoutBranch("refs/heads/master");
			WriteToFile(sourceFile, "More change\n\n\n\nFoo");
			source.Add().AddFilepattern("SomeFile.txt").Call();
			source.Commit().SetMessage("Some change in remote").Call();
			// merge side into master
			MergeCommandResult result = source.Merge().Include(sideCommit.Id).SetStrategy(MergeStrategy
				.RESOLVE).Call();
			NUnit.Framework.Assert.AreEqual(MergeStatus.MERGED, result.GetMergeStatus());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPullConflict()
		{
			PullResult res = target.Pull().Call();
			// nothing to update since we don't have different data yet
			NUnit.Framework.Assert.IsTrue(res.GetFetchResult().GetTrackingRefUpdates().IsEmpty
				());
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.UP_TO_DATE, res.GetRebaseResult
				().GetStatus());
			AssertFileContentsEqual(targetFile, "Hello world");
			// change the source file
			WriteToFile(sourceFile, "Source change");
			source.Add().AddFilepattern("SomeFile.txt").Call();
			source.Commit().SetMessage("Source change in remote").Call();
			// change the target file
			WriteToFile(targetFile, "Target change");
			target.Add().AddFilepattern("SomeFile.txt").Call();
			target.Commit().SetMessage("Target change in local").Call();
			res = target.Pull().Call();
			string remoteUri = target.GetRepository().GetConfig().GetString(ConfigConstants.CONFIG_REMOTE_SECTION
				, "origin", ConfigConstants.CONFIG_KEY_URL);
			NUnit.Framework.Assert.IsFalse(res.GetFetchResult().GetTrackingRefUpdates().IsEmpty
				());
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetRebaseResult(
				).GetStatus());
			string result = "<<<<<<< Upstream, based on branch 'master' of " + remoteUri + "\nSource change\n=======\nTarget change\n>>>>>>> 42453fd Target change in local\n";
			AssertFileContentsEqual(targetFile, result);
			NUnit.Framework.Assert.AreEqual(RepositoryState.REBASING_INTERACTIVE, target.GetRepository
				().GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPullLocalConflict()
		{
			target.BranchCreate().SetName("basedOnMaster").SetStartPoint("refs/heads/master")
				.SetUpstreamMode(CreateBranchCommand.SetupUpstreamMode.NOTRACK).Call();
			StoredConfig config = target.GetRepository().GetConfig();
			config.SetString("branch", "basedOnMaster", "remote", ".");
			config.SetString("branch", "basedOnMaster", "merge", "refs/heads/master");
			config.SetBoolean("branch", "basedOnMaster", "rebase", true);
			config.Save();
			target.GetRepository().UpdateRef(Constants.HEAD).Link("refs/heads/basedOnMaster");
			PullResult res = target.Pull().Call();
			// nothing to update since we don't have different data yet
			NUnit.Framework.Assert.IsNull(res.GetFetchResult());
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.UP_TO_DATE, res.GetRebaseResult
				().GetStatus());
			AssertFileContentsEqual(targetFile, "Hello world");
			// change the file in master
			target.GetRepository().UpdateRef(Constants.HEAD).Link("refs/heads/master");
			WriteToFile(targetFile, "Master change");
			target.Add().AddFilepattern("SomeFile.txt").Call();
			target.Commit().SetMessage("Source change in master").Call();
			// change the file in slave
			target.GetRepository().UpdateRef(Constants.HEAD).Link("refs/heads/basedOnMaster");
			WriteToFile(targetFile, "Slave change");
			target.Add().AddFilepattern("SomeFile.txt").Call();
			target.Commit().SetMessage("Source change in based on master").Call();
			res = target.Pull().Call();
			NUnit.Framework.Assert.IsNull(res.GetFetchResult());
			NUnit.Framework.Assert.AreEqual(RebaseResult.Status.STOPPED, res.GetRebaseResult(
				).GetStatus());
			string result = "<<<<<<< Upstream, based on branch 'master' of local repository\n"
				 + "Master change\n=======\nSlave change\n>>>>>>> 4049c9e Source change in based on master\n";
			AssertFileContentsEqual(targetFile, result);
			NUnit.Framework.Assert.AreEqual(RepositoryState.REBASING_INTERACTIVE, target.GetRepository
				().GetRepositoryState());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			dbTarget = CreateWorkRepository();
			source = new Git(db);
			target = new Git(dbTarget);
			// put some file in the source repo
			sourceFile = new FilePath(db.WorkTree, "SomeFile.txt");
			WriteToFile(sourceFile, "Hello world");
			// and commit it
			source.Add().AddFilepattern("SomeFile.txt").Call();
			source.Commit().SetMessage("Initial commit for source").Call();
			// configure the target repo to connect to the source via "origin"
			StoredConfig targetConfig = ((FileBasedConfig)dbTarget.GetConfig());
			targetConfig.SetString("branch", "master", "remote", "origin");
			targetConfig.SetString("branch", "master", "merge", "refs/heads/master");
			RemoteConfig config = new RemoteConfig(targetConfig, "origin");
			config.AddURI(new URIish(source.GetRepository().WorkTree.GetPath()));
			config.AddFetchRefSpec(new RefSpec("+refs/heads/*:refs/remotes/origin/*"));
			config.Update(targetConfig);
			targetConfig.Save();
			targetFile = new FilePath(dbTarget.WorkTree, "SomeFile.txt");
			// make sure we have the same content
			target.Pull().Call();
			target.Checkout().SetStartPoint("refs/remotes/origin/master").SetName("master").Call
				();
			targetConfig.SetString("branch", "master", "merge", "refs/heads/master");
			targetConfig.SetBoolean("branch", "master", "rebase", true);
			targetConfig.Save();
			AssertFileContentsEqual(targetFile, "Hello world");
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void WriteToFile(FilePath actFile, string @string)
		{
			FileOutputStream fos = null;
			try
			{
				fos = new FileOutputStream(actFile);
				fos.Write(Sharpen.Runtime.GetBytesForString(@string, "UTF-8"));
				fos.Close();
			}
			finally
			{
				if (fos != null)
				{
					fos.Close();
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertFileContentsEqual(FilePath actFile, string @string)
		{
			ByteArrayOutputStream bos = new ByteArrayOutputStream();
			FileInputStream fis = null;
			byte[] buffer = new byte[100];
			try
			{
				fis = new FileInputStream(actFile);
				int read = fis.Read(buffer);
				while (read > 0)
				{
					bos.Write(buffer, 0, read);
					read = fis.Read(buffer);
				}
				string content = Sharpen.Runtime.GetStringForBytes(bos.ToByteArray(), "UTF-8");
				NUnit.Framework.Assert.AreEqual(@string, content);
			}
			finally
			{
				if (fis != null)
				{
					fis.Close();
				}
			}
		}
	}
}
