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
using NUnit.Framework;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Unit tests of
	/// <see cref="RenameBranchCommand">RenameBranchCommand</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class RenameBranchCommandTest : RepositoryTestCase
	{
		private static readonly string PATH = "file.txt";

		private RevCommit head;

		private Git git;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			git = Git.Wrap(db);
			WriteTrashFile(PATH, "content");
			git.Add().AddFilepattern(PATH).Call();
			head = git.Commit().SetMessage("add file").Call();
			NUnit.Framework.Assert.IsNotNull(head);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void RenameBranchNoConfigValues()
		{
			StoredConfig config = git.GetRepository().GetConfig();
			config.UnsetSection(ConfigConstants.CONFIG_BRANCH_SECTION, Constants.MASTER);
			config.Save();
			string branch = "b1";
			NUnit.Framework.Assert.IsTrue(config.GetNames(ConfigConstants.CONFIG_BRANCH_SECTION
				, Constants.MASTER).IsEmpty());
			NUnit.Framework.Assert.IsNotNull(git.BranchRename().SetNewName(branch).Call());
			config = git.GetRepository().GetConfig();
			NUnit.Framework.Assert.IsTrue(config.GetNames(ConfigConstants.CONFIG_BRANCH_SECTION
				, Constants.MASTER).IsEmpty());
			NUnit.Framework.Assert.IsTrue(config.GetNames(ConfigConstants.CONFIG_BRANCH_SECTION
				, branch).IsEmpty());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void RenameBranchSingleConfigValue()
		{
			StoredConfig config = git.GetRepository().GetConfig();
			config.SetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION, Constants.MASTER, ConfigConstants
				.CONFIG_KEY_REBASE, true);
			config.Save();
			string branch = "b1";
			NUnit.Framework.Assert.IsTrue(config.GetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION
				, Constants.MASTER, ConfigConstants.CONFIG_KEY_REBASE, true));
			NUnit.Framework.Assert.IsFalse(config.GetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION
				, branch, ConfigConstants.CONFIG_KEY_REBASE, false));
			NUnit.Framework.Assert.IsNotNull(git.BranchRename().SetNewName(branch).Call());
			config = git.GetRepository().GetConfig();
			NUnit.Framework.Assert.IsFalse(config.GetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION
				, Constants.MASTER, ConfigConstants.CONFIG_KEY_REBASE, false));
			NUnit.Framework.Assert.IsTrue(config.GetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION
				, branch, ConfigConstants.CONFIG_KEY_REBASE, false));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void RenameBranchExistingSection()
		{
			string branch = "b1";
			StoredConfig config = git.GetRepository().GetConfig();
			config.SetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION, Constants.MASTER, ConfigConstants
				.CONFIG_KEY_REBASE, true);
			config.SetString(ConfigConstants.CONFIG_BRANCH_SECTION, Constants.MASTER, "a", "a"
				);
			config.SetString(ConfigConstants.CONFIG_BRANCH_SECTION, branch, "a", "b");
			config.Save();
			NUnit.Framework.Assert.IsNotNull(git.BranchRename().SetNewName(branch).Call());
			config = git.GetRepository().GetConfig();
			Assert.AssertArrayEquals(new string[] { "b", "a" }, config.GetStringList(ConfigConstants
				.CONFIG_BRANCH_SECTION, branch, "a"));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void RenameBranchMultipleConfigValues()
		{
			StoredConfig config = git.GetRepository().GetConfig();
			config.SetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION, Constants.MASTER, ConfigConstants
				.CONFIG_KEY_REBASE, true);
			config.SetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION, Constants.MASTER, ConfigConstants
				.CONFIG_KEY_MERGE, true);
			config.Save();
			string branch = "b1";
			NUnit.Framework.Assert.IsTrue(config.GetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION
				, Constants.MASTER, ConfigConstants.CONFIG_KEY_REBASE, true));
			NUnit.Framework.Assert.IsFalse(config.GetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION
				, branch, ConfigConstants.CONFIG_KEY_REBASE, false));
			NUnit.Framework.Assert.IsTrue(config.GetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION
				, Constants.MASTER, ConfigConstants.CONFIG_KEY_MERGE, true));
			NUnit.Framework.Assert.IsFalse(config.GetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION
				, branch, ConfigConstants.CONFIG_KEY_MERGE, false));
			NUnit.Framework.Assert.IsNotNull(git.BranchRename().SetNewName(branch).Call());
			config = git.GetRepository().GetConfig();
			NUnit.Framework.Assert.IsFalse(config.GetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION
				, Constants.MASTER, ConfigConstants.CONFIG_KEY_REBASE, false));
			NUnit.Framework.Assert.IsTrue(config.GetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION
				, branch, ConfigConstants.CONFIG_KEY_REBASE, false));
			NUnit.Framework.Assert.IsFalse(config.GetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION
				, Constants.MASTER, ConfigConstants.CONFIG_KEY_MERGE, false));
			NUnit.Framework.Assert.IsTrue(config.GetBoolean(ConfigConstants.CONFIG_BRANCH_SECTION
				, branch, ConfigConstants.CONFIG_KEY_MERGE, false));
		}
	}
}
