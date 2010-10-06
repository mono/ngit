using NGit;
using NGit.Errors;
using NGit.Junit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>Tests for setting up the working directory when creating a Repository</summary>
	public class RepositorySetupWorkDirTest : LocalDiskRepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestIsBare_CreateRepositoryFromArbitraryGitDir()
		{
			FilePath gitDir = GetFile("workdir");
			NUnit.Framework.Assert.IsTrue(new FileRepository(gitDir).IsBare);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNotBare_CreateRepositoryFromDotGitGitDir()
		{
			FilePath gitDir = GetFile("workdir", Constants.DOT_GIT);
			Repository repo = new FileRepository(gitDir);
			NUnit.Framework.Assert.IsFalse(repo.IsBare);
			AssertWorkdirPath(repo, "workdir");
			AssertGitdirPath(repo, "workdir", Constants.DOT_GIT);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWorkdirIsParentDir_CreateRepositoryFromDotGitGitDir()
		{
			FilePath gitDir = GetFile("workdir", Constants.DOT_GIT);
			Repository repo = new FileRepository(gitDir);
			string workdir = repo.WorkTree.GetName();
			NUnit.Framework.Assert.AreEqual(workdir, "workdir");
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestNotBare_CreateRepositoryFromWorkDirOnly()
		{
			FilePath workdir = GetFile("workdir", "repo");
			FileRepository repo = new FileRepositoryBuilder().SetWorkTree(workdir).Build();
			NUnit.Framework.Assert.IsFalse(repo.IsBare);
			AssertWorkdirPath(repo, "workdir", "repo");
			AssertGitdirPath(repo, "workdir", "repo", Constants.DOT_GIT);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWorkdirIsDotGit_CreateRepositoryFromWorkDirOnly()
		{
			FilePath workdir = GetFile("workdir", "repo");
			FileRepository repo = new FileRepositoryBuilder().SetWorkTree(workdir).Build();
			AssertGitdirPath(repo, "workdir", "repo", Constants.DOT_GIT);
		}

		/// <exception cref="System.Exception"></exception>
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
		public virtual void TestBare_CreateRepositoryFromGitDirOnlyWithBareConfigTrue()
		{
			FilePath gitDir = GetFile("workdir", "repoWithConfig");
			SetBare(gitDir, true);
			FileRepository repo = new FileRepositoryBuilder().SetGitDir(gitDir).Build();
			NUnit.Framework.Assert.IsTrue(repo.IsBare);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWorkdirIsParent_CreateRepositoryFromGitDirOnlyWithBareConfigFalse
			()
		{
			FilePath gitDir = GetFile("workdir", "repoWithBareConfigTrue", "child");
			SetBare(gitDir, false);
			FileRepository repo = new FileRepositoryBuilder().SetGitDir(gitDir).Build();
			AssertWorkdirPath(repo, "workdir", "repoWithBareConfigTrue");
		}

		/// <exception cref="System.Exception"></exception>
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
		public virtual void TestExceptionThrown_BareRepoGetWorkDir()
		{
			FilePath gitDir = GetFile("workdir");
			try
			{
				new FileRepository(gitDir).WorkTree;
				NUnit.Framework.Assert.Fail("Expected NoWorkTreeException missing");
			}
			catch (NoWorkTreeException)
			{
			}
		}

		// expected
		/// <exception cref="System.Exception"></exception>
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
			NUnit.Framework.Assert.AreEqual("Wrong Git Directory", exp, act);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertWorkdirPath(Repository repo, params string[] expected)
		{
			FilePath exp = GetFile(expected).GetCanonicalFile();
			FilePath act = repo.WorkTree.GetCanonicalFile();
			NUnit.Framework.Assert.AreEqual("Wrong working Directory", exp, act);
		}
	}
}
