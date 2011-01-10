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
using NGit.Junit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>Tests for setting up the working directory when creating a Repository</summary>
	[NUnit.Framework.TestFixture]
	public class RepositorySetupWorkDirTest : LocalDiskRepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIsBare_CreateRepositoryFromArbitraryGitDir()
		{
			FilePath gitDir = GetFile("workdir");
			NUnit.Framework.Assert.IsTrue(new FileRepository(gitDir).IsBare);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNotBare_CreateRepositoryFromDotGitGitDir()
		{
			FilePath gitDir = GetFile("workdir", Constants.DOT_GIT);
			Repository repo = new FileRepository(gitDir);
			NUnit.Framework.Assert.IsFalse(repo.IsBare);
			AssertWorkdirPath(repo, "workdir");
			AssertGitdirPath(repo, "workdir", Constants.DOT_GIT);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWorkdirIsParentDir_CreateRepositoryFromDotGitGitDir()
		{
			FilePath gitDir = GetFile("workdir", Constants.DOT_GIT);
			Repository repo = new FileRepository(gitDir);
			string workdir = repo.WorkTree.GetName();
			NUnit.Framework.Assert.AreEqual(workdir, "workdir");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNotBare_CreateRepositoryFromWorkDirOnly()
		{
			FilePath workdir = GetFile("workdir", "repo");
			FileRepository repo = new FileRepositoryBuilder().SetWorkTree(workdir).Build();
			NUnit.Framework.Assert.IsFalse(repo.IsBare);
			AssertWorkdirPath(repo, "workdir", "repo");
			AssertGitdirPath(repo, "workdir", "repo", Constants.DOT_GIT);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWorkdirIsDotGit_CreateRepositoryFromWorkDirOnly()
		{
			FilePath workdir = GetFile("workdir", "repo");
			FileRepository repo = new FileRepositoryBuilder().SetWorkTree(workdir).Build();
			AssertGitdirPath(repo, "workdir", "repo", Constants.DOT_GIT);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNotBare_CreateRepositoryFromGitDirOnlyWithWorktreeConfig(
			)
		{
			FilePath gitDir = GetFile("workdir", "repoWithConfig");
			FilePath workTree = GetFile("workdir", "treeRoot");
			SetWorkTree(gitDir, workTree);
			FileRepository repo = new FileRepositoryBuilder().SetGitDir(gitDir).Build();
			NUnit.Framework.Assert.IsFalse(repo.IsBare);
			AssertWorkdirPath(repo, "workdir", "treeRoot");
			AssertGitdirPath(repo, "workdir", "repoWithConfig");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestBare_CreateRepositoryFromGitDirOnlyWithBareConfigTrue()
		{
			FilePath gitDir = GetFile("workdir", "repoWithConfig");
			SetBare(gitDir, true);
			FileRepository repo = new FileRepositoryBuilder().SetGitDir(gitDir).Build();
			NUnit.Framework.Assert.IsTrue(repo.IsBare);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWorkdirIsParent_CreateRepositoryFromGitDirOnlyWithBareConfigFalse
			()
		{
			FilePath gitDir = GetFile("workdir", "repoWithBareConfigTrue", "child");
			SetBare(gitDir, false);
			FileRepository repo = new FileRepositoryBuilder().SetGitDir(gitDir).Build();
			AssertWorkdirPath(repo, "workdir", "repoWithBareConfigTrue");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNotBare_CreateRepositoryFromGitDirOnlyWithBareConfigFalse
			()
		{
			FilePath gitDir = GetFile("workdir", "repoWithBareConfigFalse", "child");
			SetBare(gitDir, false);
			FileRepository repo = new FileRepositoryBuilder().SetGitDir(gitDir).Build();
			NUnit.Framework.Assert.IsFalse(repo.IsBare);
			AssertWorkdirPath(repo, "workdir", "repoWithBareConfigFalse");
			AssertGitdirPath(repo, "workdir", "repoWithBareConfigFalse", "child");
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestExceptionThrown_BareRepoGetWorkDir()
		{
			FilePath gitDir = GetFile("workdir");
			try
			{
				string s = new FileRepository(gitDir).WorkTree;
				NUnit.Framework.Assert.Fail("Expected NoWorkTreeException missing");
			}
			catch (NoWorkTreeException)
			{
			}
		}

		// expected
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestExceptionThrown_BareRepoGetIndex()
		{
			FilePath gitDir = GetFile("workdir");
			try
			{
				new FileRepository(gitDir).GetIndex();
				NUnit.Framework.Assert.Fail("Expected NoWorkTreeException missing");
			}
			catch (NoWorkTreeException)
			{
			}
		}

		// expected
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestExceptionThrown_BareRepoGetIndexFile()
		{
			FilePath gitDir = GetFile("workdir");
			try
			{
				new FileRepository(gitDir).GetIndexFile();
				NUnit.Framework.Assert.Fail("Expected NoWorkTreeException missing");
			}
			catch (NoWorkTreeException)
			{
			}
		}

		// expected
		private FilePath GetFile(params string[] pathComponents)
		{
			string rootPath = new FilePath(new FilePath("target"), "trash").GetPath();
			foreach (string pathComponent in pathComponents)
			{
				rootPath = rootPath + FilePath.separatorChar + pathComponent;
			}
			FilePath result = new FilePath(rootPath);
			result.Mkdir();
			return result;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		private void SetBare(FilePath gitDir, bool bare)
		{
			FileBasedConfig cfg = ConfigFor(gitDir);
			cfg.SetBoolean(ConfigConstants.CONFIG_CORE_SECTION, null, ConfigConstants.CONFIG_KEY_BARE
				, bare);
			cfg.Save();
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		private void SetWorkTree(FilePath gitDir, FilePath workTree)
		{
			string path = workTree.GetAbsolutePath();
			FileBasedConfig cfg = ConfigFor(gitDir);
			cfg.SetString(ConfigConstants.CONFIG_CORE_SECTION, null, ConfigConstants.CONFIG_KEY_WORKTREE
				, path);
			cfg.Save();
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		private FileBasedConfig ConfigFor(FilePath gitDir)
		{
			FilePath configPath = new FilePath(gitDir, "config");
			FileBasedConfig cfg = new FileBasedConfig(configPath, FS.DETECTED);
			cfg.Load();
			return cfg;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertGitdirPath(Repository repo, params string[] expected)
		{
			FilePath exp = GetFile(expected).GetCanonicalFile();
			FilePath act = repo.Directory.GetCanonicalFile();
			NUnit.Framework.Assert.AreEqual(exp, act, "Wrong Git Directory");
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertWorkdirPath(Repository repo, params string[] expected)
		{
			FilePath exp = GetFile(expected).GetCanonicalFile();
			FilePath act = repo.WorkTree.GetCanonicalFile();
			NUnit.Framework.Assert.AreEqual(exp, act, "Wrong working Directory");
		}
	}
}
