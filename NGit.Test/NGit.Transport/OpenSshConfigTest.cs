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
using NGit.Transport;
using NGit.Util;
using Sharpen;

namespace NGit.Transport
{
	[NUnit.Framework.TestFixture]
	public class OpenSshConfigTest : RepositoryTestCase
	{
		private FilePath home;

		private FilePath configFile;

		private OpenSshConfig osc;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			home = new FilePath(trash, "home");
			FileUtils.Mkdir(home);
			configFile = new FilePath(new FilePath(home, ".ssh"), Constants.CONFIG);
			FileUtils.Mkdir(configFile.GetParentFile());
			Runtime.SetProperty("user.name", "jex_junit");
			osc = new OpenSshConfig(home, configFile);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Config(string data)
		{
			OutputStreamWriter fw = new OutputStreamWriter(new FileOutputStream(configFile), 
				"UTF-8");
			fw.Write(data);
			fw.Close();
		}

		[NUnit.Framework.Test]
		public virtual void TestNoConfig()
		{
			OpenSshConfig.Host h = osc.Lookup("repo.or.cz");
			NUnit.Framework.Assert.IsNotNull(h);
			NUnit.Framework.Assert.AreEqual("repo.or.cz", h.GetHostName());
			NUnit.Framework.Assert.AreEqual("jex_junit", h.GetUser());
			NUnit.Framework.Assert.AreEqual(22, h.GetPort());
			NUnit.Framework.Assert.IsNull(h.GetIdentityFile());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSeparatorParsing()
		{
			Config("Host\tfirst\n" + "\tHostName\tfirst.tld\n" + "\n" + "Host second\n" + " HostName\tsecond.tld\n"
				 + "Host=third\n" + "HostName=third.tld\n\n\n" + "\t Host = fourth\n\n\n" + " \t HostName\t=fourth.tld\n"
				 + "Host\t =     last\n" + "HostName  \t    last.tld");
			NUnit.Framework.Assert.IsNotNull(osc.Lookup("first"));
			NUnit.Framework.Assert.AreEqual("first.tld", osc.Lookup("first").GetHostName());
			NUnit.Framework.Assert.IsNotNull(osc.Lookup("second"));
			NUnit.Framework.Assert.AreEqual("second.tld", osc.Lookup("second").GetHostName());
			NUnit.Framework.Assert.IsNotNull(osc.Lookup("third"));
			NUnit.Framework.Assert.AreEqual("third.tld", osc.Lookup("third").GetHostName());
			NUnit.Framework.Assert.IsNotNull(osc.Lookup("fourth"));
			NUnit.Framework.Assert.AreEqual("fourth.tld", osc.Lookup("fourth").GetHostName());
			NUnit.Framework.Assert.IsNotNull(osc.Lookup("last"));
			NUnit.Framework.Assert.AreEqual("last.tld", osc.Lookup("last").GetHostName());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestQuoteParsing()
		{
			Config("Host \"good\"\n" + " HostName=\"good.tld\"\n" + " Port=\"6007\"\n" + " User=\"gooduser\"\n"
				 + "Host multiple unquoted and \"quoted\" \"hosts\"\n" + " Port=\"2222\"\n" + "Host \"spaced\"\n"
				 + "# Bad host name, but testing preservation of spaces\n" + " HostName=\" spaced\ttld \"\n"
				 + "# Misbalanced quotes\n" + "Host \"bad\"\n" + "# OpenSSH doesn't allow this but ...\n"
				 + " HostName=bad.tld\"\n");
			NUnit.Framework.Assert.AreEqual("good.tld", osc.Lookup("good").GetHostName());
			NUnit.Framework.Assert.AreEqual("gooduser", osc.Lookup("good").GetUser());
			NUnit.Framework.Assert.AreEqual(6007, osc.Lookup("good").GetPort());
			NUnit.Framework.Assert.AreEqual(2222, osc.Lookup("multiple").GetPort());
			NUnit.Framework.Assert.AreEqual(2222, osc.Lookup("quoted").GetPort());
			NUnit.Framework.Assert.AreEqual(2222, osc.Lookup("and").GetPort());
			NUnit.Framework.Assert.AreEqual(2222, osc.Lookup("unquoted").GetPort());
			NUnit.Framework.Assert.AreEqual(2222, osc.Lookup("hosts").GetPort());
			NUnit.Framework.Assert.AreEqual(" spaced\ttld ", osc.Lookup("spaced").GetHostName
				());
			NUnit.Framework.Assert.AreEqual("bad.tld\"", osc.Lookup("bad").GetHostName());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAlias_DoesNotMatch()
		{
			Config("Host orcz\n" + "\tHostName repo.or.cz\n");
			OpenSshConfig.Host h = osc.Lookup("repo.or.cz");
			NUnit.Framework.Assert.IsNotNull(h);
			NUnit.Framework.Assert.AreEqual("repo.or.cz", h.GetHostName());
			NUnit.Framework.Assert.AreEqual("jex_junit", h.GetUser());
			NUnit.Framework.Assert.AreEqual(22, h.GetPort());
			NUnit.Framework.Assert.IsNull(h.GetIdentityFile());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAlias_OptionsSet()
		{
			Config("Host orcz\n" + "\tHostName repo.or.cz\n" + "\tPort 2222\n" + "\tUser jex\n"
				 + "\tIdentityFile .ssh/id_jex\n" + "\tForwardX11 no\n");
			OpenSshConfig.Host h = osc.Lookup("orcz");
			NUnit.Framework.Assert.IsNotNull(h);
			NUnit.Framework.Assert.AreEqual("repo.or.cz", h.GetHostName());
			NUnit.Framework.Assert.AreEqual("jex", h.GetUser());
			NUnit.Framework.Assert.AreEqual(2222, h.GetPort());
			NUnit.Framework.Assert.AreEqual(new FilePath(home, ".ssh/id_jex"), h.GetIdentityFile
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAlias_OptionsKeywordCaseInsensitive()
		{
			Config("hOsT orcz\n" + "\thOsTnAmE repo.or.cz\n" + "\tPORT 2222\n" + "\tuser jex\n"
				 + "\tidentityfile .ssh/id_jex\n" + "\tForwardX11 no\n");
			OpenSshConfig.Host h = osc.Lookup("orcz");
			NUnit.Framework.Assert.IsNotNull(h);
			NUnit.Framework.Assert.AreEqual("repo.or.cz", h.GetHostName());
			NUnit.Framework.Assert.AreEqual("jex", h.GetUser());
			NUnit.Framework.Assert.AreEqual(2222, h.GetPort());
			NUnit.Framework.Assert.AreEqual(new FilePath(home, ".ssh/id_jex"), h.GetIdentityFile
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAlias_OptionsInherit()
		{
			Config("Host orcz\n" + "\tHostName repo.or.cz\n" + "\n" + "Host *\n" + "\tHostName not.a.host.example.com\n"
				 + "\tPort 2222\n" + "\tUser jex\n" + "\tIdentityFile .ssh/id_jex\n" + "\tForwardX11 no\n"
				);
			OpenSshConfig.Host h = osc.Lookup("orcz");
			NUnit.Framework.Assert.IsNotNull(h);
			NUnit.Framework.Assert.AreEqual("repo.or.cz", h.GetHostName());
			NUnit.Framework.Assert.AreEqual("jex", h.GetUser());
			NUnit.Framework.Assert.AreEqual(2222, h.GetPort());
			NUnit.Framework.Assert.AreEqual(new FilePath(home, ".ssh/id_jex"), h.GetIdentityFile
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAlias_PreferredAuthenticationsDefault()
		{
			OpenSshConfig.Host h = osc.Lookup("orcz");
			NUnit.Framework.Assert.IsNotNull(h);
			NUnit.Framework.Assert.IsNull(h.GetPreferredAuthentications());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAlias_PreferredAuthentications()
		{
			Config("Host orcz\n" + "\tPreferredAuthentications publickey\n");
			OpenSshConfig.Host h = osc.Lookup("orcz");
			NUnit.Framework.Assert.IsNotNull(h);
			NUnit.Framework.Assert.AreEqual("publickey", h.GetPreferredAuthentications());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAlias_InheritPreferredAuthentications()
		{
			Config("Host orcz\n" + "\tHostName repo.or.cz\n" + "\n" + "Host *\n" + "\tPreferredAuthentications publickey, hostbased\n"
				);
			OpenSshConfig.Host h = osc.Lookup("orcz");
			NUnit.Framework.Assert.IsNotNull(h);
			NUnit.Framework.Assert.AreEqual("publickey,hostbased", h.GetPreferredAuthentications
				());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAlias_BatchModeDefault()
		{
			OpenSshConfig.Host h = osc.Lookup("orcz");
			NUnit.Framework.Assert.IsNotNull(h);
			NUnit.Framework.Assert.AreEqual(false, h.IsBatchMode());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAlias_BatchModeYes()
		{
			Config("Host orcz\n" + "\tBatchMode yes\n");
			OpenSshConfig.Host h = osc.Lookup("orcz");
			NUnit.Framework.Assert.IsNotNull(h);
			NUnit.Framework.Assert.AreEqual(true, h.IsBatchMode());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAlias_InheritBatchMode()
		{
			Config("Host orcz\n" + "\tHostName repo.or.cz\n" + "\n" + "Host *\n" + "\tBatchMode yes\n"
				);
			OpenSshConfig.Host h = osc.Lookup("orcz");
			NUnit.Framework.Assert.IsNotNull(h);
			NUnit.Framework.Assert.AreEqual(true, h.IsBatchMode());
		}
	}
}
