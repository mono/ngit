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
using NGit.Storage.File;
using NGit.Transport;
using Sharpen;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class PullCommandTest : RepositoryTestCase
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
			NUnit.Framework.Assert.IsTrue(res.GetMergeResult().GetMergeStatus().Equals(MergeStatus
				.ALREADY_UP_TO_DATE));
			AssertFileContentsEqual(targetFile, "Hello world");
			// change the source file
			WriteToFile(sourceFile, "Another change");
			source.Add().AddFilepattern("SomeFile.txt").Call();
			source.Commit().SetMessage("Some change in remote").Call();
			res = target.Pull().Call();
			NUnit.Framework.Assert.IsFalse(res.GetFetchResult().GetTrackingRefUpdates().IsEmpty
				());
			NUnit.Framework.Assert.AreEqual(res.GetMergeResult().GetMergeStatus(), MergeStatus
				.FAST_FORWARD);
			AssertFileContentsEqual(targetFile, "Another change");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPullConflict()
		{
			PullResult res = target.Pull().Call();
			// nothing to update since we don't have different data yet
			NUnit.Framework.Assert.IsTrue(res.GetFetchResult().GetTrackingRefUpdates().IsEmpty
				());
			NUnit.Framework.Assert.IsTrue(res.GetMergeResult().GetMergeStatus().Equals(MergeStatus
				.ALREADY_UP_TO_DATE));
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
			string sourceChangeString = "Source change\n>>>>>>> branch 'refs/heads/master' of "
				 + target.GetRepository().GetConfig().GetString("remote", "origin", "url");
			NUnit.Framework.Assert.IsFalse(res.GetFetchResult().GetTrackingRefUpdates().IsEmpty
				());
			NUnit.Framework.Assert.AreEqual(res.GetMergeResult().GetMergeStatus(), MergeStatus
				.CONFLICTING);
			string result = "<<<<<<< HEAD\nTarget change\n=======\n" + sourceChangeString + "\n";
			AssertFileContentsEqual(targetFile, result);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPullLocalConflict()
		{
			target.BranchCreate().SetName("basedOnMaster").SetStartPoint("refs/heads/master")
				.SetUpstreamMode(CreateBranchCommand.SetupUpstreamMode.TRACK).Call();
			target.GetRepository().UpdateRef(Constants.HEAD).Link("refs/heads/basedOnMaster");
			PullResult res = target.Pull().Call();
			// nothing to update since we don't have different data yet
			NUnit.Framework.Assert.IsNull(res.GetFetchResult());
			NUnit.Framework.Assert.IsTrue(res.GetMergeResult().GetMergeStatus().Equals(MergeStatus
				.ALREADY_UP_TO_DATE));
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
			string sourceChangeString = "Master change\n>>>>>>> branch 'refs/heads/master' of local repository";
			NUnit.Framework.Assert.IsNull(res.GetFetchResult());
			NUnit.Framework.Assert.AreEqual(res.GetMergeResult().GetMergeStatus(), MergeStatus
				.CONFLICTING);
			string result = "<<<<<<< HEAD\nSlave change\n=======\n" + sourceChangeString + "\n";
			AssertFileContentsEqual(targetFile, result);
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
			RevCommit commit = source.Commit().SetMessage("Initial commit for source").Call();
			// point the master branch to the new commit
			RefUpdate upd = dbTarget.UpdateRef("refs/heads/master");
			upd.SetNewObjectId(commit.Id);
			upd.Update();
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
			WriteToFile(targetFile, "Hello world");
			// make sure we have the same content
			target.Pull().Call();
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
				string content = Sharpen.Extensions.CreateString(bos.ToByteArray(), "UTF-8");
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
