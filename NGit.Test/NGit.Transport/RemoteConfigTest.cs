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
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	[NUnit.Framework.TestFixture]
	public class RemoteConfigTest
	{
		private Config config;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			config = new Config();
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		private void ReadConfig(string dat)
		{
			config = new Config();
			config.FromText(dat);
		}

		private void CheckConfig(string exp)
		{
			NUnit.Framework.Assert.AreEqual(exp, config.ToText());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSimple()
		{
			ReadConfig("[remote \"spearce\"]\n" + "url = http://www.spearce.org/egit.git\n" +
				 "fetch = +refs/heads/*:refs/remotes/spearce/*\n");
			RemoteConfig rc = new RemoteConfig(config, "spearce");
			IList<URIish> allURIs = rc.URIs;
			RefSpec spec;
			NUnit.Framework.Assert.AreEqual("spearce", rc.Name);
			NUnit.Framework.Assert.IsNotNull(allURIs);
			NUnit.Framework.Assert.IsNotNull(rc.FetchRefSpecs);
			NUnit.Framework.Assert.IsNotNull(rc.PushRefSpecs);
			NUnit.Framework.Assert.IsNotNull(rc.TagOpt);
			NUnit.Framework.Assert.AreEqual(0, rc.Timeout);
			NUnit.Framework.Assert.AreEqual(TagOpt.AUTO_FOLLOW, rc.TagOpt);
			NUnit.Framework.Assert.AreEqual(1, allURIs.Count);
			NUnit.Framework.Assert.AreEqual("http://www.spearce.org/egit.git", allURIs[0].ToString
				());
			NUnit.Framework.Assert.AreEqual(1, rc.FetchRefSpecs.Count);
			spec = rc.FetchRefSpecs[0];
			NUnit.Framework.Assert.IsTrue(spec.IsForceUpdate());
			NUnit.Framework.Assert.IsTrue(spec.IsWildcard());
			NUnit.Framework.Assert.AreEqual("refs/heads/*", spec.GetSource());
			NUnit.Framework.Assert.AreEqual("refs/remotes/spearce/*", spec.GetDestination());
			NUnit.Framework.Assert.AreEqual(0, rc.PushRefSpecs.Count);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSimpleNoTags()
		{
			ReadConfig("[remote \"spearce\"]\n" + "url = http://www.spearce.org/egit.git\n" +
				 "fetch = +refs/heads/*:refs/remotes/spearce/*\n" + "tagopt = --no-tags\n");
			RemoteConfig rc = new RemoteConfig(config, "spearce");
			NUnit.Framework.Assert.AreEqual(TagOpt.NO_TAGS, rc.TagOpt);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSimpleAlwaysTags()
		{
			ReadConfig("[remote \"spearce\"]\n" + "url = http://www.spearce.org/egit.git\n" +
				 "fetch = +refs/heads/*:refs/remotes/spearce/*\n" + "tagopt = --tags\n");
			RemoteConfig rc = new RemoteConfig(config, "spearce");
			NUnit.Framework.Assert.AreEqual(TagOpt.FETCH_TAGS, rc.TagOpt);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMirror()
		{
			ReadConfig("[remote \"spearce\"]\n" + "url = http://www.spearce.org/egit.git\n" +
				 "fetch = +refs/heads/*:refs/heads/*\n" + "fetch = refs/tags/*:refs/tags/*\n");
			RemoteConfig rc = new RemoteConfig(config, "spearce");
			IList<URIish> allURIs = rc.URIs;
			RefSpec spec;
			NUnit.Framework.Assert.AreEqual("spearce", rc.Name);
			NUnit.Framework.Assert.IsNotNull(allURIs);
			NUnit.Framework.Assert.IsNotNull(rc.FetchRefSpecs);
			NUnit.Framework.Assert.IsNotNull(rc.PushRefSpecs);
			NUnit.Framework.Assert.AreEqual(1, allURIs.Count);
			NUnit.Framework.Assert.AreEqual("http://www.spearce.org/egit.git", allURIs[0].ToString
				());
			NUnit.Framework.Assert.AreEqual(2, rc.FetchRefSpecs.Count);
			spec = rc.FetchRefSpecs[0];
			NUnit.Framework.Assert.IsTrue(spec.IsForceUpdate());
			NUnit.Framework.Assert.IsTrue(spec.IsWildcard());
			NUnit.Framework.Assert.AreEqual("refs/heads/*", spec.GetSource());
			NUnit.Framework.Assert.AreEqual("refs/heads/*", spec.GetDestination());
			spec = rc.FetchRefSpecs[1];
			NUnit.Framework.Assert.IsFalse(spec.IsForceUpdate());
			NUnit.Framework.Assert.IsTrue(spec.IsWildcard());
			NUnit.Framework.Assert.AreEqual("refs/tags/*", spec.GetSource());
			NUnit.Framework.Assert.AreEqual("refs/tags/*", spec.GetDestination());
			NUnit.Framework.Assert.AreEqual(0, rc.PushRefSpecs.Count);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBackup()
		{
			ReadConfig("[remote \"backup\"]\n" + "url = http://www.spearce.org/egit.git\n" + 
				"url = user@repo.or.cz:/srv/git/egit.git\n" + "push = +refs/heads/*:refs/heads/*\n"
				 + "push = refs/tags/*:refs/tags/*\n");
			RemoteConfig rc = new RemoteConfig(config, "backup");
			IList<URIish> allURIs = rc.URIs;
			RefSpec spec;
			NUnit.Framework.Assert.AreEqual("backup", rc.Name);
			NUnit.Framework.Assert.IsNotNull(allURIs);
			NUnit.Framework.Assert.IsNotNull(rc.FetchRefSpecs);
			NUnit.Framework.Assert.IsNotNull(rc.PushRefSpecs);
			NUnit.Framework.Assert.AreEqual(2, allURIs.Count);
			NUnit.Framework.Assert.AreEqual("http://www.spearce.org/egit.git", allURIs[0].ToString
				());
			NUnit.Framework.Assert.AreEqual("user@repo.or.cz:/srv/git/egit.git", allURIs[1].ToString
				());
			NUnit.Framework.Assert.AreEqual(0, rc.FetchRefSpecs.Count);
			NUnit.Framework.Assert.AreEqual(2, rc.PushRefSpecs.Count);
			spec = rc.PushRefSpecs[0];
			NUnit.Framework.Assert.IsTrue(spec.IsForceUpdate());
			NUnit.Framework.Assert.IsTrue(spec.IsWildcard());
			NUnit.Framework.Assert.AreEqual("refs/heads/*", spec.GetSource());
			NUnit.Framework.Assert.AreEqual("refs/heads/*", spec.GetDestination());
			spec = rc.PushRefSpecs[1];
			NUnit.Framework.Assert.IsFalse(spec.IsForceUpdate());
			NUnit.Framework.Assert.IsTrue(spec.IsWildcard());
			NUnit.Framework.Assert.AreEqual("refs/tags/*", spec.GetSource());
			NUnit.Framework.Assert.AreEqual("refs/tags/*", spec.GetDestination());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUploadPack()
		{
			ReadConfig("[remote \"example\"]\n" + "url = user@example.com:egit.git\n" + "fetch = +refs/heads/*:refs/remotes/example/*\n"
				 + "uploadpack = /path/to/git/git-upload-pack\n" + "receivepack = /path/to/git/git-receive-pack\n"
				);
			RemoteConfig rc = new RemoteConfig(config, "example");
			IList<URIish> allURIs = rc.URIs;
			RefSpec spec;
			NUnit.Framework.Assert.AreEqual("example", rc.Name);
			NUnit.Framework.Assert.IsNotNull(allURIs);
			NUnit.Framework.Assert.IsNotNull(rc.FetchRefSpecs);
			NUnit.Framework.Assert.IsNotNull(rc.PushRefSpecs);
			NUnit.Framework.Assert.AreEqual(1, allURIs.Count);
			NUnit.Framework.Assert.AreEqual("user@example.com:egit.git", allURIs[0].ToString(
				));
			NUnit.Framework.Assert.AreEqual(1, rc.FetchRefSpecs.Count);
			spec = rc.FetchRefSpecs[0];
			NUnit.Framework.Assert.IsTrue(spec.IsForceUpdate());
			NUnit.Framework.Assert.IsTrue(spec.IsWildcard());
			NUnit.Framework.Assert.AreEqual("refs/heads/*", spec.GetSource());
			NUnit.Framework.Assert.AreEqual("refs/remotes/example/*", spec.GetDestination());
			NUnit.Framework.Assert.AreEqual(0, rc.PushRefSpecs.Count);
			NUnit.Framework.Assert.AreEqual("/path/to/git/git-upload-pack", rc.UploadPack);
			NUnit.Framework.Assert.AreEqual("/path/to/git/git-receive-pack", rc.ReceivePack);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUnknown()
		{
			ReadConfig(string.Empty);
			RemoteConfig rc = new RemoteConfig(config, "backup");
			NUnit.Framework.Assert.AreEqual(0, rc.URIs.Count);
			NUnit.Framework.Assert.AreEqual(0, rc.FetchRefSpecs.Count);
			NUnit.Framework.Assert.AreEqual(0, rc.PushRefSpecs.Count);
			NUnit.Framework.Assert.AreEqual("git-upload-pack", rc.UploadPack);
			NUnit.Framework.Assert.AreEqual("git-receive-pack", rc.ReceivePack);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAddURI()
		{
			ReadConfig(string.Empty);
			URIish uri = new URIish("/some/dir");
			RemoteConfig rc = new RemoteConfig(config, "backup");
			NUnit.Framework.Assert.AreEqual(0, rc.URIs.Count);
			NUnit.Framework.Assert.IsTrue(rc.AddURI(uri));
			NUnit.Framework.Assert.AreEqual(1, rc.URIs.Count);
			NUnit.Framework.Assert.AreSame(uri, rc.URIs[0]);
			NUnit.Framework.Assert.IsFalse(rc.AddURI(new URIish(uri.ToString())));
			NUnit.Framework.Assert.AreEqual(1, rc.URIs.Count);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRemoveFirstURI()
		{
			ReadConfig(string.Empty);
			URIish a = new URIish("/some/dir");
			URIish b = new URIish("/another/dir");
			URIish c = new URIish("/more/dirs");
			RemoteConfig rc = new RemoteConfig(config, "backup");
			NUnit.Framework.Assert.IsTrue(rc.AddURI(a));
			NUnit.Framework.Assert.IsTrue(rc.AddURI(b));
			NUnit.Framework.Assert.IsTrue(rc.AddURI(c));
			NUnit.Framework.Assert.AreEqual(3, rc.URIs.Count);
			NUnit.Framework.Assert.AreSame(a, rc.URIs[0]);
			NUnit.Framework.Assert.AreSame(b, rc.URIs[1]);
			NUnit.Framework.Assert.AreSame(c, rc.URIs[2]);
			NUnit.Framework.Assert.IsTrue(rc.RemoveURI(a));
			NUnit.Framework.Assert.AreEqual(2, rc.URIs.Count);
			NUnit.Framework.Assert.AreSame(b, rc.URIs[0]);
			NUnit.Framework.Assert.AreSame(c, rc.URIs[1]);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRemoveMiddleURI()
		{
			ReadConfig(string.Empty);
			URIish a = new URIish("/some/dir");
			URIish b = new URIish("/another/dir");
			URIish c = new URIish("/more/dirs");
			RemoteConfig rc = new RemoteConfig(config, "backup");
			NUnit.Framework.Assert.IsTrue(rc.AddURI(a));
			NUnit.Framework.Assert.IsTrue(rc.AddURI(b));
			NUnit.Framework.Assert.IsTrue(rc.AddURI(c));
			NUnit.Framework.Assert.AreEqual(3, rc.URIs.Count);
			NUnit.Framework.Assert.AreSame(a, rc.URIs[0]);
			NUnit.Framework.Assert.AreSame(b, rc.URIs[1]);
			NUnit.Framework.Assert.AreSame(c, rc.URIs[2]);
			NUnit.Framework.Assert.IsTrue(rc.RemoveURI(b));
			NUnit.Framework.Assert.AreEqual(2, rc.URIs.Count);
			NUnit.Framework.Assert.AreSame(a, rc.URIs[0]);
			NUnit.Framework.Assert.AreSame(c, rc.URIs[1]);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRemoveLastURI()
		{
			ReadConfig(string.Empty);
			URIish a = new URIish("/some/dir");
			URIish b = new URIish("/another/dir");
			URIish c = new URIish("/more/dirs");
			RemoteConfig rc = new RemoteConfig(config, "backup");
			NUnit.Framework.Assert.IsTrue(rc.AddURI(a));
			NUnit.Framework.Assert.IsTrue(rc.AddURI(b));
			NUnit.Framework.Assert.IsTrue(rc.AddURI(c));
			NUnit.Framework.Assert.AreEqual(3, rc.URIs.Count);
			NUnit.Framework.Assert.AreSame(a, rc.URIs[0]);
			NUnit.Framework.Assert.AreSame(b, rc.URIs[1]);
			NUnit.Framework.Assert.AreSame(c, rc.URIs[2]);
			NUnit.Framework.Assert.IsTrue(rc.RemoveURI(c));
			NUnit.Framework.Assert.AreEqual(2, rc.URIs.Count);
			NUnit.Framework.Assert.AreSame(a, rc.URIs[0]);
			NUnit.Framework.Assert.AreSame(b, rc.URIs[1]);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestRemoveOnlyURI()
		{
			ReadConfig(string.Empty);
			URIish a = new URIish("/some/dir");
			RemoteConfig rc = new RemoteConfig(config, "backup");
			NUnit.Framework.Assert.IsTrue(rc.AddURI(a));
			NUnit.Framework.Assert.AreEqual(1, rc.URIs.Count);
			NUnit.Framework.Assert.AreSame(a, rc.URIs[0]);
			NUnit.Framework.Assert.IsTrue(rc.RemoveURI(a));
			NUnit.Framework.Assert.AreEqual(0, rc.URIs.Count);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCreateOrigin()
		{
			RemoteConfig rc = new RemoteConfig(config, "origin");
			rc.AddURI(new URIish("/some/dir"));
			rc.AddFetchRefSpec(new RefSpec("+refs/heads/*:refs/remotes/" + rc.Name + "/*"));
			rc.Update(config);
			CheckConfig("[remote \"origin\"]\n" + "\turl = /some/dir\n" + "\tfetch = +refs/heads/*:refs/remotes/origin/*\n"
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSaveAddURI()
		{
			ReadConfig("[remote \"spearce\"]\n" + "url = http://www.spearce.org/egit.git\n" +
				 "fetch = +refs/heads/*:refs/remotes/spearce/*\n");
			RemoteConfig rc = new RemoteConfig(config, "spearce");
			rc.AddURI(new URIish("/some/dir"));
			NUnit.Framework.Assert.AreEqual(2, rc.URIs.Count);
			rc.Update(config);
			CheckConfig("[remote \"spearce\"]\n" + "\turl = http://www.spearce.org/egit.git\n"
				 + "\turl = /some/dir\n" + "\tfetch = +refs/heads/*:refs/remotes/spearce/*\n");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSaveRemoveLastURI()
		{
			ReadConfig("[remote \"spearce\"]\n" + "url = http://www.spearce.org/egit.git\n" +
				 "url = /some/dir\n" + "fetch = +refs/heads/*:refs/remotes/spearce/*\n");
			RemoteConfig rc = new RemoteConfig(config, "spearce");
			NUnit.Framework.Assert.AreEqual(2, rc.URIs.Count);
			rc.RemoveURI(new URIish("/some/dir"));
			NUnit.Framework.Assert.AreEqual(1, rc.URIs.Count);
			rc.Update(config);
			CheckConfig("[remote \"spearce\"]\n" + "\turl = http://www.spearce.org/egit.git\n"
				 + "\tfetch = +refs/heads/*:refs/remotes/spearce/*\n");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSaveRemoveFirstURI()
		{
			ReadConfig("[remote \"spearce\"]\n" + "url = http://www.spearce.org/egit.git\n" +
				 "url = /some/dir\n" + "fetch = +refs/heads/*:refs/remotes/spearce/*\n");
			RemoteConfig rc = new RemoteConfig(config, "spearce");
			NUnit.Framework.Assert.AreEqual(2, rc.URIs.Count);
			rc.RemoveURI(new URIish("http://www.spearce.org/egit.git"));
			NUnit.Framework.Assert.AreEqual(1, rc.URIs.Count);
			rc.Update(config);
			CheckConfig("[remote \"spearce\"]\n" + "\turl = /some/dir\n" + "\tfetch = +refs/heads/*:refs/remotes/spearce/*\n"
				);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSaveNoTags()
		{
			RemoteConfig rc = new RemoteConfig(config, "origin");
			rc.AddURI(new URIish("/some/dir"));
			rc.AddFetchRefSpec(new RefSpec("+refs/heads/*:refs/remotes/" + rc.Name + "/*"));
			rc.TagOpt = TagOpt.NO_TAGS;
			rc.Update(config);
			CheckConfig("[remote \"origin\"]\n" + "\turl = /some/dir\n" + "\tfetch = +refs/heads/*:refs/remotes/origin/*\n"
				 + "\ttagopt = --no-tags\n");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSaveAllTags()
		{
			RemoteConfig rc = new RemoteConfig(config, "origin");
			rc.AddURI(new URIish("/some/dir"));
			rc.AddFetchRefSpec(new RefSpec("+refs/heads/*:refs/remotes/" + rc.Name + "/*"));
			rc.TagOpt = TagOpt.FETCH_TAGS;
			rc.Update(config);
			CheckConfig("[remote \"origin\"]\n" + "\turl = /some/dir\n" + "\tfetch = +refs/heads/*:refs/remotes/origin/*\n"
				 + "\ttagopt = --tags\n");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSimpleTimeout()
		{
			ReadConfig("[remote \"spearce\"]\n" + "url = http://www.spearce.org/egit.git\n" +
				 "fetch = +refs/heads/*:refs/remotes/spearce/*\n" + "timeout = 12\n");
			RemoteConfig rc = new RemoteConfig(config, "spearce");
			NUnit.Framework.Assert.AreEqual(12, rc.Timeout);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSaveTimeout()
		{
			RemoteConfig rc = new RemoteConfig(config, "origin");
			rc.AddURI(new URIish("/some/dir"));
			rc.AddFetchRefSpec(new RefSpec("+refs/heads/*:refs/remotes/" + rc.Name + "/*"));
			rc.Timeout = 60;
			rc.Update(config);
			CheckConfig("[remote \"origin\"]\n" + "\turl = /some/dir\n" + "\tfetch = +refs/heads/*:refs/remotes/origin/*\n"
				 + "\ttimeout = 60\n");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void NoInsteadOf()
		{
			config.SetString("remote", "origin", "url", "short:project.git");
			config.SetString("url", "https://server/repos/", "name", "short:");
			RemoteConfig rc = new RemoteConfig(config, "origin");
			NUnit.Framework.Assert.IsFalse(rc.URIs.IsEmpty());
			NUnit.Framework.Assert.AreEqual("short:project.git", rc.URIs[0].ToASCIIString());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void SingleInsteadOf()
		{
			config.SetString("remote", "origin", "url", "short:project.git");
			config.SetString("url", "https://server/repos/", "insteadOf", "short:");
			RemoteConfig rc = new RemoteConfig(config, "origin");
			NUnit.Framework.Assert.IsFalse(rc.URIs.IsEmpty());
			NUnit.Framework.Assert.AreEqual("https://server/repos/project.git", rc.URIs[0].ToASCIIString
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void MultipleInsteadOf()
		{
			config.SetString("remote", "origin", "url", "prefixproject.git");
			config.SetStringList("url", "https://server/repos/", "insteadOf", Arrays.AsList("pre"
				, "prefix", "pref", "perf"));
			RemoteConfig rc = new RemoteConfig(config, "origin");
			NUnit.Framework.Assert.IsFalse(rc.URIs.IsEmpty());
			NUnit.Framework.Assert.AreEqual("https://server/repos/project.git", rc.URIs[0].ToASCIIString
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void NoPushInsteadOf()
		{
			config.SetString("remote", "origin", "pushurl", "short:project.git");
			config.SetString("url", "https://server/repos/", "name", "short:");
			RemoteConfig rc = new RemoteConfig(config, "origin");
			NUnit.Framework.Assert.IsFalse(rc.PushURIs.IsEmpty());
			NUnit.Framework.Assert.AreEqual("short:project.git", rc.PushURIs[0].ToASCIIString
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void SinglePushInsteadOf()
		{
			config.SetString("remote", "origin", "pushurl", "short:project.git");
			config.SetString("url", "https://server/repos/", "pushInsteadOf", "short:");
			RemoteConfig rc = new RemoteConfig(config, "origin");
			NUnit.Framework.Assert.IsFalse(rc.PushURIs.IsEmpty());
			NUnit.Framework.Assert.AreEqual("https://server/repos/project.git", rc.PushURIs[0
				].ToASCIIString());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void MultiplePushInsteadOf()
		{
			config.SetString("remote", "origin", "pushurl", "prefixproject.git");
			config.SetStringList("url", "https://server/repos/", "pushInsteadOf", Arrays.AsList
				("pre", "prefix", "pref", "perf"));
			RemoteConfig rc = new RemoteConfig(config, "origin");
			NUnit.Framework.Assert.IsFalse(rc.PushURIs.IsEmpty());
			NUnit.Framework.Assert.AreEqual("https://server/repos/project.git", rc.PushURIs[0
				].ToASCIIString());
		}
	}
}
