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
using System.Diagnostics;
using NGit;
using NGit.Api;
using NGit.Diff;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Submodule;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using NGit.Util;
using Sharpen;

namespace NGit.Api
{
	/// <summary>
	/// Unit tests of
	/// <see cref="CommitCommand">CommitCommand</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class CommitCommandTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestExecutableRetention()
		{
			StoredConfig config = ((FileBasedConfig)db.GetConfig());
			config.SetBoolean(ConfigConstants.CONFIG_CORE_SECTION, null, ConfigConstants.CONFIG_KEY_FILEMODE
				, true);
			config.Save();
			FS executableFs = new _FS_81();
			Git git = Git.Open(db.Directory, executableFs);
			string path = "a.txt";
			WriteTrashFile(path, "content");
			git.Add().AddFilepattern(path).Call();
			RevCommit commit1 = git.Commit().SetMessage("commit").Call();
			TreeWalk walk = TreeWalk.ForPath(db, path, commit1.Tree);
			NUnit.Framework.Assert.IsNotNull(walk);
			NUnit.Framework.Assert.AreEqual(FileMode.EXECUTABLE_FILE, walk.GetFileMode(0));
			FS nonExecutableFs = new _FS_121();
			config = ((FileBasedConfig)db.GetConfig());
			config.SetBoolean(ConfigConstants.CONFIG_CORE_SECTION, null, ConfigConstants.CONFIG_KEY_FILEMODE
				, false);
			config.Save();
			Git git2 = Git.Open(db.Directory, nonExecutableFs);
			WriteTrashFile(path, "content2");
			RevCommit commit2 = git2.Commit().SetOnly(path).SetMessage("commit2").Call();
			walk = TreeWalk.ForPath(db, path, commit2.Tree);
			NUnit.Framework.Assert.IsNotNull(walk);
			NUnit.Framework.Assert.AreEqual(FileMode.EXECUTABLE_FILE, walk.GetFileMode(0));
		}

		private sealed class _FS_81 : FS
		{
			public _FS_81()
			{
			}

			public override bool SupportsExecute()
			{
				return true;
			}

			public override bool SetExecute(FilePath f, bool canExec)
			{
				return true;
			}

			public override ProcessStartInfo RunInShell(string cmd, string[] args)
			{
				return null;
			}

			public override bool RetryFailedLockFileCommit()
			{
				return false;
			}

			public override FS NewInstance()
			{
				return this;
			}

			protected internal override FilePath DiscoverGitPrefix()
			{
				return null;
			}

			public override bool CanExecute(FilePath f)
			{
				return true;
			}
		}

		private sealed class _FS_121 : FS
		{
			public _FS_121()
			{
			}

			public override bool SupportsExecute()
			{
				return false;
			}

			public override bool SetExecute(FilePath f, bool canExec)
			{
				return false;
			}

			public override ProcessStartInfo RunInShell(string cmd, string[] args)
			{
				return null;
			}

			public override bool RetryFailedLockFileCommit()
			{
				return false;
			}

			public override FS NewInstance()
			{
				return this;
			}

			protected internal override FilePath DiscoverGitPrefix()
			{
				return null;
			}

			public override bool CanExecute(FilePath f)
			{
				return false;
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void CommitNewSubmodule()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit = git.Commit().SetMessage("create file").Call();
			SubmoduleAddCommand command = new SubmoduleAddCommand(db);
			string path = "sub";
			command.SetPath(path);
			string uri = db.Directory.ToURI().ToString();
			command.SetURI(uri);
			Repository repo = command.Call();
			NUnit.Framework.Assert.IsNotNull(repo);
			SubmoduleWalk generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.AreEqual(path, generator.GetPath());
			NUnit.Framework.Assert.AreEqual(commit, generator.GetObjectId());
			NUnit.Framework.Assert.AreEqual(uri, generator.GetModulesUrl());
			NUnit.Framework.Assert.AreEqual(path, generator.GetModulesPath());
			NUnit.Framework.Assert.AreEqual(uri, generator.GetConfigUrl());
			NUnit.Framework.Assert.IsNotNull(generator.GetRepository());
			NUnit.Framework.Assert.AreEqual(commit, repo.Resolve(Constants.HEAD));
			RevCommit submoduleCommit = git.Commit().SetMessage("submodule add").SetOnly(path
				).Call();
			NUnit.Framework.Assert.IsNotNull(submoduleCommit);
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(commit.Tree);
			walk.AddTree(submoduleCommit.Tree);
			walk.Filter = TreeFilter.ANY_DIFF;
			IList<DiffEntry> diffs = DiffEntry.Scan(walk);
			NUnit.Framework.Assert.AreEqual(1, diffs.Count);
			DiffEntry subDiff = diffs[0];
			NUnit.Framework.Assert.AreEqual(FileMode.MISSING, subDiff.GetOldMode());
			NUnit.Framework.Assert.AreEqual(FileMode.GITLINK, subDiff.GetNewMode());
			NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, subDiff.GetOldId().ToObjectId());
			NUnit.Framework.Assert.AreEqual(commit, subDiff.GetNewId().ToObjectId());
			NUnit.Framework.Assert.AreEqual(path, subDiff.GetNewPath());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void CommitSubmoduleUpdate()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit = git.Commit().SetMessage("create file").Call();
			WriteTrashFile("file.txt", "content2");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit2 = git.Commit().SetMessage("edit file").Call();
			SubmoduleAddCommand command = new SubmoduleAddCommand(db);
			string path = "sub";
			command.SetPath(path);
			string uri = db.Directory.ToURI().ToString();
			command.SetURI(uri);
			Repository repo = command.Call();
			NUnit.Framework.Assert.IsNotNull(repo);
			SubmoduleWalk generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.AreEqual(path, generator.GetPath());
			NUnit.Framework.Assert.AreEqual(commit2, generator.GetObjectId());
			NUnit.Framework.Assert.AreEqual(uri, generator.GetModulesUrl());
			NUnit.Framework.Assert.AreEqual(path, generator.GetModulesPath());
			NUnit.Framework.Assert.AreEqual(uri, generator.GetConfigUrl());
			NUnit.Framework.Assert.IsNotNull(generator.GetRepository());
			NUnit.Framework.Assert.AreEqual(commit2, repo.Resolve(Constants.HEAD));
			RevCommit submoduleAddCommit = git.Commit().SetMessage("submodule add").SetOnly(path
				).Call();
			NUnit.Framework.Assert.IsNotNull(submoduleAddCommit);
			RefUpdate update = repo.UpdateRef(Constants.HEAD);
			update.SetNewObjectId(commit);
			NUnit.Framework.Assert.AreEqual(RefUpdate.Result.FORCED, update.ForceUpdate());
			RevCommit submoduleEditCommit = git.Commit().SetMessage("submodule add").SetOnly(
				path).Call();
			NUnit.Framework.Assert.IsNotNull(submoduleEditCommit);
			TreeWalk walk = new TreeWalk(db);
			walk.AddTree(submoduleAddCommit.Tree);
			walk.AddTree(submoduleEditCommit.Tree);
			walk.Filter = TreeFilter.ANY_DIFF;
			IList<DiffEntry> diffs = DiffEntry.Scan(walk);
			NUnit.Framework.Assert.AreEqual(1, diffs.Count);
			DiffEntry subDiff = diffs[0];
			NUnit.Framework.Assert.AreEqual(FileMode.GITLINK, subDiff.GetOldMode());
			NUnit.Framework.Assert.AreEqual(FileMode.GITLINK, subDiff.GetNewMode());
			NUnit.Framework.Assert.AreEqual(commit2, subDiff.GetOldId().ToObjectId());
			NUnit.Framework.Assert.AreEqual(commit, subDiff.GetNewId().ToObjectId());
			NUnit.Framework.Assert.AreEqual(path, subDiff.GetNewPath());
			NUnit.Framework.Assert.AreEqual(path, subDiff.GetOldPath());
		}
	}
}
