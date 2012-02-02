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
using NGit.Junit;
using NGit.Revwalk;
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class BranchTrackingStatusTest : RepositoryTestCase
	{
		private TestRepository<Repository> util;

		protected internal RevWalk rw;

		/// <exception cref="System.Exception"></exception>
		public override void SetUp()
		{
			base.SetUp();
			util = new TestRepository<Repository>(db);
			StoredConfig config = util.GetRepository().GetConfig();
			config.SetString(ConfigConstants.CONFIG_BRANCH_SECTION, "master", ConfigConstants
				.CONFIG_KEY_REMOTE, "origin");
			config.SetString(ConfigConstants.CONFIG_BRANCH_SECTION, "master", ConfigConstants
				.CONFIG_KEY_MERGE, "refs/heads/master");
			config.SetString(ConfigConstants.CONFIG_REMOTE_SECTION, "origin", "fetch", "+refs/heads/*:refs/remotes/origin/*"
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldWorkInNormalCase()
		{
			RevCommit remoteTracking = util.Branch("refs/remotes/origin/master").Commit().Create
				();
			util.Branch("master").Commit().Parent(remoteTracking).Create();
			util.Branch("master").Commit().Create();
			BranchTrackingStatus status = BranchTrackingStatus.Of(util.GetRepository(), "master"
				);
			NUnit.Framework.Assert.AreEqual(2, status.GetAheadCount());
			NUnit.Framework.Assert.AreEqual(0, status.GetBehindCount());
			NUnit.Framework.Assert.AreEqual("refs/remotes/origin/master", status.GetRemoteTrackingBranch
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldWorkWithoutMergeBase()
		{
			util.Branch("refs/remotes/origin/master").Commit().Create();
			util.Branch("master").Commit().Create();
			BranchTrackingStatus status = BranchTrackingStatus.Of(util.GetRepository(), "master"
				);
			NUnit.Framework.Assert.AreEqual(1, status.GetAheadCount());
			NUnit.Framework.Assert.AreEqual(1, status.GetBehindCount());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ShouldReturnNullWhenBranchDoesntExist()
		{
			BranchTrackingStatus status = BranchTrackingStatus.Of(util.GetRepository(), "doesntexist"
				);
			NUnit.Framework.Assert.IsNull(status);
		}
	}
}
