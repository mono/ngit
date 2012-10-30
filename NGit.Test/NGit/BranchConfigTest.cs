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
using NGit.Errors;
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class BranchConfigTest
	{
		[NUnit.Framework.Test]
		public virtual void GetRemoteTrackingBranchShouldHandleNormalCase()
		{
			Config c = Parse(string.Empty + "[remote \"origin\"]\n" + "  fetch = +refs/heads/*:refs/remotes/origin/*\n"
				 + "[branch \"master\"]\n" + "  remote = origin\n" + "  merge = refs/heads/master\n"
				);
			//
			BranchConfig branchConfig = new BranchConfig(c, "master");
			NUnit.Framework.Assert.AreEqual("refs/remotes/origin/master", branchConfig.GetRemoteTrackingBranch
				());
		}

		[NUnit.Framework.Test]
		public virtual void GetRemoteTrackingBranchShouldHandleOtherMapping()
		{
			Config c = Parse(string.Empty + "[remote \"test\"]\n" + "  fetch = +refs/foo/*:refs/remotes/origin/foo/*\n"
				 + "  fetch = +refs/heads/*:refs/remotes/origin/*\n" + "  fetch = +refs/other/*:refs/remotes/origin/other/*\n"
				 + "[branch \"master\"]\n" + "  remote = test\n" + "  merge = refs/foo/master\n"
				 + "\n");
			//
			BranchConfig branchConfig = new BranchConfig(c, "master");
			NUnit.Framework.Assert.AreEqual("refs/remotes/origin/foo/master", branchConfig.GetRemoteTrackingBranch
				());
		}

		[NUnit.Framework.Test]
		public virtual void GetRemoteTrackingBranchShouldReturnNullWithoutFetchSpec()
		{
			Config c = Parse(string.Empty + "[remote \"origin\"]\n" + "  fetch = +refs/heads/onlyone:refs/remotes/origin/onlyone\n"
				 + "[branch \"master\"]\n" + "  remote = origin\n" + "  merge = refs/heads/master\n"
				);
			//
			BranchConfig branchConfig = new BranchConfig(c, "master");
			NUnit.Framework.Assert.IsNull(branchConfig.GetRemoteTrackingBranch());
		}

		[NUnit.Framework.Test]
		public virtual void GetRemoteTrackingBranchShouldReturnNullWithoutMergeBranch()
		{
			Config c = Parse(string.Empty + "[remote \"origin\"]\n" + "  fetch = +refs/heads/onlyone:refs/remotes/origin/onlyone\n"
				 + "[branch \"master\"]\n" + "  remote = origin\n");
			//
			BranchConfig branchConfig = new BranchConfig(c, "master");
			NUnit.Framework.Assert.IsNull(branchConfig.GetRemoteTrackingBranch());
		}

		[NUnit.Framework.Test]
		public virtual void GetTrackingBranchShouldReturnMergeBranchForLocalBranch()
		{
			Config c = Parse(string.Empty + "[remote \"origin\"]\n" + "  fetch = +refs/heads/*:refs/remotes/origin/*\n"
				 + "[branch \"master\"]\n" + "  remote = .\n" + "  merge = refs/heads/master\n");
			//
			BranchConfig branchConfig = new BranchConfig(c, "master");
			NUnit.Framework.Assert.AreEqual("refs/heads/master", branchConfig.GetTrackingBranch
				());
		}

		[NUnit.Framework.Test]
		public virtual void GetTrackingBranchShouldReturnNullWithoutMergeBranchForLocalBranch
			()
		{
			Config c = Parse(string.Empty + "[remote \"origin\"]\n" + "  fetch = +refs/heads/onlyone:refs/remotes/origin/onlyone\n"
				 + "[branch \"master\"]\n" + "  remote = .\n");
			//
			//
			BranchConfig branchConfig = new BranchConfig(c, "master");
			NUnit.Framework.Assert.IsNull(branchConfig.GetTrackingBranch());
		}

		[NUnit.Framework.Test]
		public virtual void GetTrackingBranchShouldHandleNormalCaseForRemoteTrackingBranch
			()
		{
			Config c = Parse(string.Empty + "[remote \"origin\"]\n" + "  fetch = +refs/heads/*:refs/remotes/origin/*\n"
				 + "[branch \"master\"]\n" + "  remote = origin\n" + "  merge = refs/heads/master\n"
				);
			//
			BranchConfig branchConfig = new BranchConfig(c, "master");
			NUnit.Framework.Assert.AreEqual("refs/remotes/origin/master", branchConfig.GetTrackingBranch
				());
		}

		private Config Parse(string content)
		{
			Config c = new Config(null);
			try
			{
				c.FromText(content);
			}
			catch (ConfigInvalidException e)
			{
				throw new RuntimeException(e);
			}
			return c;
		}
	}
}
