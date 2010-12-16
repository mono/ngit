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
			AssertEquals(commit.Id, db2.Resolve(commit.Id.GetName() + "^{commit}"));
			AssertEquals(tag.Id, db2.Resolve(tag.Id.GetName()));
		}
	}
}
