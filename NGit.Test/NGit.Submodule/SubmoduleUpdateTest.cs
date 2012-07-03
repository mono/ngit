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
using NGit.Api;
using NGit.Dircache;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Submodule;
using Sharpen;

namespace NGit.Submodule
{
	/// <summary>
	/// Unit tests of
	/// <see cref="NGit.Api.SubmoduleUpdateCommand">NGit.Api.SubmoduleUpdateCommand</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class SubmoduleUpdateTest : RepositoryTestCase
	{
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void RepositoryWithNoSubmodules()
		{
			SubmoduleUpdateCommand command = new SubmoduleUpdateCommand(db);
			ICollection<string> modules = command.Call();
			NUnit.Framework.Assert.IsNotNull(modules);
			NUnit.Framework.Assert.IsTrue(modules.IsEmpty());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void RepositoryWithSubmodule()
		{
			WriteTrashFile("file.txt", "content");
			Git git = Git.Wrap(db);
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit = git.Commit().SetMessage("create file").Call();
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_94(commit, path));
			editor.Commit();
			StoredConfig config = ((FileBasedConfig)db.GetConfig());
			config.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants.
				CONFIG_KEY_URL, db.Directory.ToURI().ToString());
			config.Save();
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, path);
			modulesConfig.Save();
			SubmoduleUpdateCommand command = new SubmoduleUpdateCommand(db);
			ICollection<string> updated = command.Call();
			NUnit.Framework.Assert.IsNotNull(updated);
			NUnit.Framework.Assert.AreEqual(1, updated.Count);
			NUnit.Framework.Assert.AreEqual(path, updated.Iterator().Next());
			SubmoduleWalk generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			Repository subRepo = generator.GetRepository();
			AddRepoToClose(subRepo);
			NUnit.Framework.Assert.IsNotNull(subRepo);
			NUnit.Framework.Assert.AreEqual(commit, subRepo.Resolve(Constants.HEAD));
		}

		private sealed class _PathEdit_94 : DirCacheEditor.PathEdit
		{
			public _PathEdit_94(RevCommit commit, string baseArg1) : base(baseArg1)
			{
				this.commit = commit;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(commit);
			}

			private readonly RevCommit commit;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void RepositoryWithUnconfiguredSubmodule()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_137(id, path));
			editor.Commit();
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, path);
			string url = "git://server/repo.git";
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, url);
			string update = "rebase";
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_UPDATE, update);
			modulesConfig.Save();
			SubmoduleUpdateCommand command = new SubmoduleUpdateCommand(db);
			ICollection<string> updated = command.Call();
			NUnit.Framework.Assert.IsNotNull(updated);
			NUnit.Framework.Assert.IsTrue(updated.IsEmpty());
		}

		private sealed class _PathEdit_137 : DirCacheEditor.PathEdit
		{
			public _PathEdit_137(ObjectId id, string baseArg1) : base(baseArg1)
			{
				this.id = id;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(id);
			}

			private readonly ObjectId id;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Api.Errors.GitAPIException"></exception>
		[NUnit.Framework.Test]
		public virtual void RepositoryWithInitializedSubmodule()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_172(id, path));
			editor.Commit();
			Repository subRepo = Git.Init().SetBare(false).SetDirectory(new FilePath(db.WorkTree
				, path)).Call().GetRepository();
			NUnit.Framework.Assert.IsNotNull(subRepo);
			SubmoduleUpdateCommand command = new SubmoduleUpdateCommand(db);
			ICollection<string> updated = command.Call();
			NUnit.Framework.Assert.IsNotNull(updated);
			NUnit.Framework.Assert.IsTrue(updated.IsEmpty());
		}

		private sealed class _PathEdit_172 : DirCacheEditor.PathEdit
		{
			public _PathEdit_172(ObjectId id, string baseArg1) : base(baseArg1)
			{
				this.id = id;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(id);
			}

			private readonly ObjectId id;
		}
	}
}
