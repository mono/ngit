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
using NGit.Errors;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Transport;
using Sharpen;

namespace NGit.Api
{
	[NUnit.Framework.TestFixture]
	public class PushCommandTest : RepositoryTestCase
	{
		/// <exception cref="NGit.Api.Errors.JGitInternalException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPush()
		{
			// create other repository
			Repository db2 = CreateWorkRepository();
			// setup the first repository
			StoredConfig config = ((FileBasedConfig)db.GetConfig());
			RemoteConfig remoteConfig = new RemoteConfig(config, "test");
			URIish uri = new URIish(db2.Directory.ToURI().ToURL());
			remoteConfig.AddURI(uri);
			remoteConfig.Update(config);
			config.Save();
			Git git1 = new Git(db);
			// create some refs via commits and tag
			RevCommit commit = git1.Commit().SetMessage("initial commit").Call();
			RevTag tag = git1.Tag().SetName("tag").Call();
			try
			{
				db2.Resolve(commit.Id.GetName() + "^{commit}");
				NUnit.Framework.Assert.Fail("id shouldn't exist yet");
			}
			catch (MissingObjectException)
			{
			}
			// we should get here
			RefSpec spec = new RefSpec("refs/heads/master:refs/heads/x");
			git1.Push().SetRemote("test").SetRefSpecs(spec).Call();
			NUnit.Framework.Assert.AreEqual(commit.Id, db2.Resolve(commit.Id.GetName() + "^{commit}"
				));
			NUnit.Framework.Assert.AreEqual(tag.Id, db2.Resolve(tag.Id.GetName()));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTrackingUpdate()
		{
			Repository db2 = CreateBareRepository();
			string remote = "origin";
			string branch = "refs/heads/master";
			string trackingBranch = "refs/remotes/" + remote + "/master";
			Git git = new Git(db);
			RevCommit commit1 = git.Commit().SetMessage("Initial commit").Call();
			RefUpdate branchRefUpdate = db.UpdateRef(branch);
			branchRefUpdate.SetNewObjectId(commit1.Id);
			branchRefUpdate.Update();
			RefUpdate trackingBranchRefUpdate = db.UpdateRef(trackingBranch);
			trackingBranchRefUpdate.SetNewObjectId(commit1.Id);
			trackingBranchRefUpdate.Update();
			StoredConfig config = ((FileBasedConfig)db.GetConfig());
			RemoteConfig remoteConfig = new RemoteConfig(config, remote);
			URIish uri = new URIish(db2.Directory.ToURI().ToURL());
			remoteConfig.AddURI(uri);
			remoteConfig.AddFetchRefSpec(new RefSpec("+refs/heads/*:refs/remotes/" + remote +
				 "/*"));
			remoteConfig.Update(config);
			config.Save();
			RevCommit commit2 = git.Commit().SetMessage("Commit to push").Call();
			RefSpec spec = new RefSpec(branch + ":" + branch);
			Iterable<PushResult> resultIterable = git.Push().SetRemote(remote).SetRefSpecs(spec
				).Call();
			PushResult result = resultIterable.Iterator().Next();
			TrackingRefUpdate trackingRefUpdate = result.GetTrackingRefUpdate(trackingBranch);
			NUnit.Framework.Assert.IsNotNull(trackingRefUpdate);
			NUnit.Framework.Assert.AreEqual(trackingBranch, trackingRefUpdate.GetLocalName());
			NUnit.Framework.Assert.AreEqual(branch, trackingRefUpdate.GetRemoteName());
			NUnit.Framework.Assert.AreEqual(commit2.Id, trackingRefUpdate.GetNewObjectId());
			NUnit.Framework.Assert.AreEqual(commit2.Id, db.Resolve(trackingBranch));
			NUnit.Framework.Assert.AreEqual(commit2.Id, db2.Resolve(branch));
		}
	}
}
