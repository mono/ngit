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
using NGit;
using NGit.Junit;
using NGit.Util;
using Sharpen;

namespace NGit
{
	/// <summary>Test reading of git config</summary>
	[NUnit.Framework.TestFixture]
	public class ConfigTest
	{
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test001_ReadBareKey()
		{
			Config c = Parse("[foo]\nbar\n");
			NUnit.Framework.Assert.AreEqual(true, c.GetBoolean("foo", null, "bar", false));
			NUnit.Framework.Assert.AreEqual(string.Empty, c.GetString("foo", null, "bar"));
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test002_ReadWithSubsection()
		{
			Config c = Parse("[foo \"zip\"]\nbar\n[foo \"zap\"]\nbar=false\nn=3\n");
			NUnit.Framework.Assert.AreEqual(true, c.GetBoolean("foo", "zip", "bar", false));
			NUnit.Framework.Assert.AreEqual(string.Empty, c.GetString("foo", "zip", "bar"));
			NUnit.Framework.Assert.AreEqual(false, c.GetBoolean("foo", "zap", "bar", true));
			NUnit.Framework.Assert.AreEqual("false", c.GetString("foo", "zap", "bar"));
			NUnit.Framework.Assert.AreEqual(3, c.GetInt("foo", "zap", "n", 4));
			NUnit.Framework.Assert.AreEqual(4, c.GetInt("foo", "zap", "m", 4));
		}

		[NUnit.Framework.Test]
		public virtual void Test003_PutRemote()
		{
			Config c = new Config();
			c.SetString("sec", "ext", "name", "value");
			c.SetString("sec", "ext", "name2", "value2");
			string expText = "[sec \"ext\"]\n\tname = value\n\tname2 = value2\n";
			NUnit.Framework.Assert.AreEqual(expText, c.ToText());
		}

		[NUnit.Framework.Test]
		public virtual void Test004_PutGetSimple()
		{
			Config c = new Config();
			c.SetString("my", null, "somename", "false");
			NUnit.Framework.Assert.AreEqual("false", c.GetString("my", null, "somename"));
			NUnit.Framework.Assert.AreEqual("[my]\n\tsomename = false\n", c.ToText());
		}

		[NUnit.Framework.Test]
		public virtual void Test005_PutGetStringList()
		{
			Config c = new Config();
			List<string> values = new List<string>();
			values.AddItem("value1");
			values.AddItem("value2");
			c.SetStringList("my", null, "somename", values);
			object[] expArr = Sharpen.Collections.ToArray(values);
			string[] actArr = c.GetStringList("my", null, "somename");
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(expArr, actArr));
			string expText = "[my]\n\tsomename = value1\n\tsomename = value2\n";
			NUnit.Framework.Assert.AreEqual(expText, c.ToText());
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test006_readCaseInsensitive()
		{
			Config c = Parse("[Foo]\nBar\n");
			NUnit.Framework.Assert.AreEqual(true, c.GetBoolean("foo", null, "bar", false));
			NUnit.Framework.Assert.AreEqual(string.Empty, c.GetString("foo", null, "bar"));
		}

		[NUnit.Framework.Test]
		public virtual void Test007_readUserConfig()
		{
			MockSystemReader mockSystemReader = new MockSystemReader();
			SystemReader.SetInstance(mockSystemReader);
			string hostname = mockSystemReader.GetHostname();
			Config userGitConfig = mockSystemReader.OpenUserConfig(null, FS.DETECTED);
			Config localConfig = new Config(userGitConfig);
			mockSystemReader.ClearProperties();
			string authorName;
			string authorEmail;
			// no values defined nowhere
			authorName = localConfig.Get(UserConfig.KEY).GetAuthorName();
			authorEmail = localConfig.Get(UserConfig.KEY).GetAuthorEmail();
			NUnit.Framework.Assert.AreEqual(Constants.UNKNOWN_USER_DEFAULT, authorName);
			NUnit.Framework.Assert.AreEqual(Constants.UNKNOWN_USER_DEFAULT + "@" + hostname, 
				authorEmail);
			NUnit.Framework.Assert.IsTrue(localConfig.Get(UserConfig.KEY).IsAuthorNameImplicit
				());
			NUnit.Framework.Assert.IsTrue(localConfig.Get(UserConfig.KEY).IsAuthorEmailImplicit
				());
			// the system user name is defined
			mockSystemReader.SetProperty(Constants.OS_USER_NAME_KEY, "os user name");
			localConfig.Uncache(UserConfig.KEY);
			authorName = localConfig.Get(UserConfig.KEY).GetAuthorName();
			NUnit.Framework.Assert.AreEqual("os user name", authorName);
			NUnit.Framework.Assert.IsTrue(localConfig.Get(UserConfig.KEY).IsAuthorNameImplicit
				());
			if (hostname != null && hostname.Length != 0)
			{
				authorEmail = localConfig.Get(UserConfig.KEY).GetAuthorEmail();
				NUnit.Framework.Assert.AreEqual("os user name@" + hostname, authorEmail);
			}
			NUnit.Framework.Assert.IsTrue(localConfig.Get(UserConfig.KEY).IsAuthorEmailImplicit
				());
			// the git environment variables are defined
			mockSystemReader.SetProperty(Constants.GIT_AUTHOR_NAME_KEY, "git author name");
			mockSystemReader.SetProperty(Constants.GIT_AUTHOR_EMAIL_KEY, "author@email");
			localConfig.Uncache(UserConfig.KEY);
			authorName = localConfig.Get(UserConfig.KEY).GetAuthorName();
			authorEmail = localConfig.Get(UserConfig.KEY).GetAuthorEmail();
			NUnit.Framework.Assert.AreEqual("git author name", authorName);
			NUnit.Framework.Assert.AreEqual("author@email", authorEmail);
			NUnit.Framework.Assert.IsFalse(localConfig.Get(UserConfig.KEY).IsAuthorNameImplicit
				());
			NUnit.Framework.Assert.IsFalse(localConfig.Get(UserConfig.KEY).IsAuthorEmailImplicit
				());
			// the values are defined in the global configuration
			userGitConfig.SetString("user", null, "name", "global username");
			userGitConfig.SetString("user", null, "email", "author@globalemail");
			authorName = localConfig.Get(UserConfig.KEY).GetAuthorName();
			authorEmail = localConfig.Get(UserConfig.KEY).GetAuthorEmail();
			NUnit.Framework.Assert.AreEqual("global username", authorName);
			NUnit.Framework.Assert.AreEqual("author@globalemail", authorEmail);
			NUnit.Framework.Assert.IsFalse(localConfig.Get(UserConfig.KEY).IsAuthorNameImplicit
				());
			NUnit.Framework.Assert.IsFalse(localConfig.Get(UserConfig.KEY).IsAuthorEmailImplicit
				());
			// the values are defined in the local configuration
			localConfig.SetString("user", null, "name", "local username");
			localConfig.SetString("user", null, "email", "author@localemail");
			authorName = localConfig.Get(UserConfig.KEY).GetAuthorName();
			authorEmail = localConfig.Get(UserConfig.KEY).GetAuthorEmail();
			NUnit.Framework.Assert.AreEqual("local username", authorName);
			NUnit.Framework.Assert.AreEqual("author@localemail", authorEmail);
			NUnit.Framework.Assert.IsFalse(localConfig.Get(UserConfig.KEY).IsAuthorNameImplicit
				());
			NUnit.Framework.Assert.IsFalse(localConfig.Get(UserConfig.KEY).IsAuthorEmailImplicit
				());
			authorName = localConfig.Get(UserConfig.KEY).GetCommitterName();
			authorEmail = localConfig.Get(UserConfig.KEY).GetCommitterEmail();
			NUnit.Framework.Assert.AreEqual("local username", authorName);
			NUnit.Framework.Assert.AreEqual("author@localemail", authorEmail);
			NUnit.Framework.Assert.IsFalse(localConfig.Get(UserConfig.KEY).IsCommitterNameImplicit
				());
			NUnit.Framework.Assert.IsFalse(localConfig.Get(UserConfig.KEY).IsCommitterEmailImplicit
				());
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadBoolean_TrueFalse1()
		{
			Config c = Parse("[s]\na = true\nb = false\n");
			NUnit.Framework.Assert.AreEqual("true", c.GetString("s", null, "a"));
			NUnit.Framework.Assert.AreEqual("false", c.GetString("s", null, "b"));
			NUnit.Framework.Assert.IsTrue(c.GetBoolean("s", "a", false));
			NUnit.Framework.Assert.IsFalse(c.GetBoolean("s", "b", true));
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadBoolean_TrueFalse2()
		{
			Config c = Parse("[s]\na = TrUe\nb = fAlSe\n");
			NUnit.Framework.Assert.AreEqual("TrUe", c.GetString("s", null, "a"));
			NUnit.Framework.Assert.AreEqual("fAlSe", c.GetString("s", null, "b"));
			NUnit.Framework.Assert.IsTrue(c.GetBoolean("s", "a", false));
			NUnit.Framework.Assert.IsFalse(c.GetBoolean("s", "b", true));
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadBoolean_YesNo1()
		{
			Config c = Parse("[s]\na = yes\nb = no\n");
			NUnit.Framework.Assert.AreEqual("yes", c.GetString("s", null, "a"));
			NUnit.Framework.Assert.AreEqual("no", c.GetString("s", null, "b"));
			NUnit.Framework.Assert.IsTrue(c.GetBoolean("s", "a", false));
			NUnit.Framework.Assert.IsFalse(c.GetBoolean("s", "b", true));
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadBoolean_YesNo2()
		{
			Config c = Parse("[s]\na = yEs\nb = NO\n");
			NUnit.Framework.Assert.AreEqual("yEs", c.GetString("s", null, "a"));
			NUnit.Framework.Assert.AreEqual("NO", c.GetString("s", null, "b"));
			NUnit.Framework.Assert.IsTrue(c.GetBoolean("s", "a", false));
			NUnit.Framework.Assert.IsFalse(c.GetBoolean("s", "b", true));
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadBoolean_OnOff1()
		{
			Config c = Parse("[s]\na = on\nb = off\n");
			NUnit.Framework.Assert.AreEqual("on", c.GetString("s", null, "a"));
			NUnit.Framework.Assert.AreEqual("off", c.GetString("s", null, "b"));
			NUnit.Framework.Assert.IsTrue(c.GetBoolean("s", "a", false));
			NUnit.Framework.Assert.IsFalse(c.GetBoolean("s", "b", true));
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadBoolean_OnOff2()
		{
			Config c = Parse("[s]\na = ON\nb = OFF\n");
			NUnit.Framework.Assert.AreEqual("ON", c.GetString("s", null, "a"));
			NUnit.Framework.Assert.AreEqual("OFF", c.GetString("s", null, "b"));
			NUnit.Framework.Assert.IsTrue(c.GetBoolean("s", "a", false));
			NUnit.Framework.Assert.IsFalse(c.GetBoolean("s", "b", true));
		}

		internal enum TestEnum
		{
			ONE_TWO
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestGetEnum()
		{
			Config c = Parse("[s]\na = ON\nb = input\nc = true\nd = off\n");
			NUnit.Framework.Assert.AreEqual(CoreConfig.AutoCRLF.TRUE, c.GetEnum("s", null, "a"
				, CoreConfig.AutoCRLF.FALSE));
			NUnit.Framework.Assert.AreEqual(CoreConfig.AutoCRLF.INPUT, c.GetEnum("s", null, "b"
				, CoreConfig.AutoCRLF.FALSE));
			NUnit.Framework.Assert.AreEqual(CoreConfig.AutoCRLF.TRUE, c.GetEnum("s", null, "c"
				, CoreConfig.AutoCRLF.FALSE));
			NUnit.Framework.Assert.AreEqual(CoreConfig.AutoCRLF.FALSE, c.GetEnum("s", null, "d"
				, CoreConfig.AutoCRLF.TRUE));
			c = new Config();
			NUnit.Framework.Assert.AreEqual(CoreConfig.AutoCRLF.FALSE, c.GetEnum("s", null, "d"
				, CoreConfig.AutoCRLF.FALSE));
			c = Parse("[s \"b\"]\n\tc = one two\n");
			NUnit.Framework.Assert.AreEqual(ConfigTest.TestEnum.ONE_TWO, c.GetEnum("s", "b", 
				"c", ConfigTest.TestEnum.ONE_TWO));
		}

		[NUnit.Framework.Test]
		public virtual void TestSetEnum()
		{
			Config c = new Config();
			c.SetEnum("s", "b", "c", ConfigTest.TestEnum.ONE_TWO);
			NUnit.Framework.Assert.AreEqual("[s \"b\"]\n\tc = one two\n", c.ToText());
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestReadLong()
		{
			AssertReadLong(1L);
			AssertReadLong(-1L);
			AssertReadLong(long.MinValue);
			AssertReadLong(long.MaxValue);
			AssertReadLong(4L * 1024 * 1024 * 1024, "4g");
			AssertReadLong(3L * 1024 * 1024, "3 m");
			AssertReadLong(8L * 1024, "8 k");
			try
			{
				AssertReadLong(-1, "1.5g");
				NUnit.Framework.Assert.Fail("incorrectly accepted 1.5g");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("Invalid integer value: s.a=1.5g", e.Message);
			}
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBooleanWithNoValue()
		{
			Config c = Parse("[my]\n\tempty\n");
			NUnit.Framework.Assert.AreEqual(string.Empty, c.GetString("my", null, "empty"));
			NUnit.Framework.Assert.AreEqual(1, c.GetStringList("my", null, "empty").Length);
			NUnit.Framework.Assert.AreEqual(string.Empty, c.GetStringList("my", null, "empty"
				)[0]);
			NUnit.Framework.Assert.IsTrue(c.GetBoolean("my", "empty", false));
			NUnit.Framework.Assert.AreEqual("[my]\n\tempty\n", c.ToText());
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyString()
		{
			Config c = Parse("[my]\n\tempty =\n");
			NUnit.Framework.Assert.IsNull(c.GetString("my", null, "empty"));
			string[] values = c.GetStringList("my", null, "empty");
			NUnit.Framework.Assert.IsNotNull(values);
			NUnit.Framework.Assert.AreEqual(1, values.Length);
			NUnit.Framework.Assert.IsNull(values[0]);
			// always matches the default, because its non-boolean
			NUnit.Framework.Assert.IsTrue(c.GetBoolean("my", "empty", true));
			NUnit.Framework.Assert.IsFalse(c.GetBoolean("my", "empty", false));
			NUnit.Framework.Assert.AreEqual("[my]\n\tempty =\n", c.ToText());
			c = new Config();
			c.SetStringList("my", null, "empty", Arrays.AsList(values));
			NUnit.Framework.Assert.AreEqual("[my]\n\tempty =\n", c.ToText());
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUnsetBranchSection()
		{
			Config c = Parse(string.Empty + "[branch \"keep\"]\n" + "  merge = master.branch.to.keep.in.the.file\n"
				 + "\n" + "[branch \"remove\"]\n" + "  merge = this.will.get.deleted\n" + "  remote = origin-for-some-long-gone-place\n"
				 + "\n" + "[core-section-not-to-remove-in-test]\n" + "  packedGitLimit = 14\n");
			//
			c.UnsetSection("branch", "does.not.exist");
			c.UnsetSection("branch", "remove");
			NUnit.Framework.Assert.AreEqual(string.Empty + "[branch \"keep\"]\n" + "  merge = master.branch.to.keep.in.the.file\n"
				 + "\n" + "[core-section-not-to-remove-in-test]\n" + "  packedGitLimit = 14\n", 
				c.ToText());
		}

		//
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUnsetSingleSection()
		{
			Config c = Parse(string.Empty + "[branch \"keep\"]\n" + "  merge = master.branch.to.keep.in.the.file\n"
				 + "\n" + "[single]\n" + "  merge = this.will.get.deleted\n" + "  remote = origin-for-some-long-gone-place\n"
				 + "\n" + "[core-section-not-to-remove-in-test]\n" + "  packedGitLimit = 14\n");
			//
			c.UnsetSection("single", null);
			NUnit.Framework.Assert.AreEqual(string.Empty + "[branch \"keep\"]\n" + "  merge = master.branch.to.keep.in.the.file\n"
				 + "\n" + "[core-section-not-to-remove-in-test]\n" + "  packedGitLimit = 14\n", 
				c.ToText());
		}

		//
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test008_readSectionNames()
		{
			Config c = Parse("[a]\n [B]\n");
			ICollection<string> sections = c.GetSections();
			NUnit.Framework.Assert.IsTrue(sections.Contains("a"), "Sections should contain \"a\""
				);
			NUnit.Framework.Assert.IsTrue(sections.Contains("b"), "Sections should contain \"b\""
				);
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test009_readNamesInSection()
		{
			string configString = "[core]\n" + "repositoryFormatVersion = 0\n" + "filemode = false\n"
				 + "logAllRefUpdates = true\n";
			Config c = Parse(configString);
			ICollection<string> names = c.GetNames("core");
			NUnit.Framework.Assert.AreEqual(3, names.Count, "Core section size");
			NUnit.Framework.Assert.IsTrue(names.Contains("filemode"), "Core section should contain \"filemode\""
				);
			NUnit.Framework.Assert.IsTrue(names.Contains("repositoryFormatVersion"), "Core section should contain \"repositoryFormatVersion\""
				);
			NUnit.Framework.Assert.IsTrue(names.Contains("repositoryformatversion"), "Core section should contain \"repositoryformatversion\""
				);
			Iterator<string> itr = names.Iterator();
			NUnit.Framework.Assert.AreEqual("repositoryFormatVersion", itr.Next());
			NUnit.Framework.Assert.AreEqual("filemode", itr.Next());
			NUnit.Framework.Assert.AreEqual("logAllRefUpdates", itr.Next());
			NUnit.Framework.Assert.IsFalse(itr.HasNext());
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test010_readNamesInSubSection()
		{
			string configString = "[a \"sub1\"]\n" + "x = 0\n" + "y = false\n" + "z = true\n"
				 + "[a \"sub2\"]\n" + "a=0\n" + "b=1\n";
			//
			//
			//
			//
			//
			//
			Config c = Parse(configString);
			ICollection<string> names = c.GetNames("a", "sub1");
			NUnit.Framework.Assert.AreEqual(3, names.Count, "Subsection size");
			NUnit.Framework.Assert.IsTrue(names.Contains("x"), "Subsection should contain \"x\""
				);
			NUnit.Framework.Assert.IsTrue(names.Contains("y"), "Subsection should contain \"y\""
				);
			NUnit.Framework.Assert.IsTrue(names.Contains("z"), "Subsection should contain \"z\""
				);
			names = c.GetNames("a", "sub2");
			NUnit.Framework.Assert.AreEqual(2, names.Count, "Subsection size");
			NUnit.Framework.Assert.IsTrue(names.Contains("a"), "Subsection should contain \"a\""
				);
			NUnit.Framework.Assert.IsTrue(names.Contains("b"), "Subsection should contain \"b\""
				);
		}

		[NUnit.Framework.Test]
		public virtual void TestQuotingForSubSectionNames()
		{
			string resultPattern = "[testsection \"{0}\"]\n\ttestname = testvalue\n";
			string result;
			Config config = new Config();
			config.SetString("testsection", "testsubsection", "testname", "testvalue");
			result = MessageFormat.Format(resultPattern, "testsubsection");
			NUnit.Framework.Assert.AreEqual(result, config.ToText());
			config.Clear();
			config.SetString("testsection", "#quotable", "testname", "testvalue");
			result = MessageFormat.Format(resultPattern, "#quotable");
			NUnit.Framework.Assert.AreEqual(result, config.ToText());
			config.Clear();
			config.SetString("testsection", "with\"quote", "testname", "testvalue");
			result = MessageFormat.Format(resultPattern, "with\\\"quote");
			NUnit.Framework.Assert.AreEqual(result, config.ToText());
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		private void AssertReadLong(long exp)
		{
			AssertReadLong(exp, exp.ToString());
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		private void AssertReadLong(long exp, string act)
		{
			Config c = Parse("[s]\na = " + act + "\n");
			NUnit.Framework.Assert.AreEqual(exp, c.GetLong("s", null, "a", 0L));
		}

		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		private Config Parse(string content)
		{
			Config c = new Config(null);
			c.FromText(content);
			return c;
		}
	}
}
