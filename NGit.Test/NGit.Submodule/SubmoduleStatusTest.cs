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
using NGit.Storage.File;
using NGit.Submodule;
using Sharpen;

namespace NGit.Submodule
{
	/// <summary>
	/// Unit tests of
	/// <see cref="NGit.Api.SubmoduleStatusCommand">NGit.Api.SubmoduleStatusCommand</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class SubmoduleStatusTest : RepositoryTestCase
	{
		[NUnit.Framework.Test]
		public virtual void RepositoryWithNoSubmodules()
		{
			SubmoduleStatusCommand command = new SubmoduleStatusCommand(db);
			IDictionary<string, SubmoduleStatus> statuses = command.Call();
			NUnit.Framework.Assert.IsNotNull(statuses);
			NUnit.Framework.Assert.IsTrue(statuses.IsEmpty());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void RepositoryWithMissingSubmodule()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_91(id, path));
			editor.Commit();
			SubmoduleStatusCommand command = new SubmoduleStatusCommand(db);
			IDictionary<string, SubmoduleStatus> statuses = command.Call();
			NUnit.Framework.Assert.IsNotNull(statuses);
			NUnit.Framework.Assert.AreEqual(1, statuses.Count);
			KeyValuePair<string, SubmoduleStatus> module = statuses.EntrySet().Iterator().Next
				();
			NUnit.Framework.Assert.IsNotNull(module);
			NUnit.Framework.Assert.AreEqual(path, module.Key);
			SubmoduleStatus status = module.Value;
			NUnit.Framework.Assert.IsNotNull(status);
			NUnit.Framework.Assert.AreEqual(path, status.GetPath());
			NUnit.Framework.Assert.AreEqual(id, status.GetIndexId());
			NUnit.Framework.Assert.AreEqual(SubmoduleStatusType.MISSING, status.GetType());
		}

		private sealed class _PathEdit_91 : DirCacheEditor.PathEdit
		{
			public _PathEdit_91(ObjectId id, string baseArg1) : base(baseArg1)
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
		[NUnit.Framework.Test]
		public virtual void RepositoryWithUninitializedSubmodule()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_122(id, path));
			editor.Commit();
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, path);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, "git://server/repo.git");
			modulesConfig.Save();
			SubmoduleStatusCommand command = new SubmoduleStatusCommand(db);
			IDictionary<string, SubmoduleStatus> statuses = command.Call();
			NUnit.Framework.Assert.IsNotNull(statuses);
			NUnit.Framework.Assert.AreEqual(1, statuses.Count);
			KeyValuePair<string, SubmoduleStatus> module = statuses.EntrySet().Iterator().Next
				();
			NUnit.Framework.Assert.IsNotNull(module);
			NUnit.Framework.Assert.AreEqual(path, module.Key);
			SubmoduleStatus status = module.Value;
			NUnit.Framework.Assert.IsNotNull(status);
			NUnit.Framework.Assert.AreEqual(path, status.GetPath());
			NUnit.Framework.Assert.AreEqual(id, status.GetIndexId());
			NUnit.Framework.Assert.AreEqual(SubmoduleStatusType.UNINITIALIZED, status.GetType
				());
		}

		private sealed class _PathEdit_122 : DirCacheEditor.PathEdit
		{
			public _PathEdit_122(ObjectId id, string baseArg1) : base(baseArg1)
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
		[NUnit.Framework.Test]
		public virtual void RepositoryWithNoHeadInSubmodule()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_161(id, path));
			editor.Commit();
			string url = "git://server/repo.git";
			StoredConfig config = ((FileBasedConfig)db.GetConfig());
			config.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants.
				CONFIG_KEY_URL, url);
			config.Save();
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, path);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, url);
			modulesConfig.Save();
			Repository subRepo = Git.Init().SetBare(false).SetDirectory(new FilePath(db.WorkTree
				, path)).Call().GetRepository();
			NUnit.Framework.Assert.IsNotNull(subRepo);
			SubmoduleStatusCommand command = new SubmoduleStatusCommand(db);
			IDictionary<string, SubmoduleStatus> statuses = command.Call();
			NUnit.Framework.Assert.IsNotNull(statuses);
			NUnit.Framework.Assert.AreEqual(1, statuses.Count);
			KeyValuePair<string, SubmoduleStatus> module = statuses.EntrySet().Iterator().Next
				();
			NUnit.Framework.Assert.IsNotNull(module);
			NUnit.Framework.Assert.AreEqual(path, module.Key);
			SubmoduleStatus status = module.Value;
			NUnit.Framework.Assert.IsNotNull(status);
			NUnit.Framework.Assert.AreEqual(path, status.GetPath());
			NUnit.Framework.Assert.AreEqual(id, status.GetIndexId());
			NUnit.Framework.Assert.AreEqual(SubmoduleStatusType.UNINITIALIZED, status.GetType
				());
		}

		private sealed class _PathEdit_161 : DirCacheEditor.PathEdit
		{
			public _PathEdit_161(ObjectId id, string baseArg1) : base(baseArg1)
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
		[NUnit.Framework.Test]
		public virtual void RepositoryWithNoSubmoduleRepository()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_211(id, path));
			editor.Commit();
			string url = "git://server/repo.git";
			StoredConfig config = ((FileBasedConfig)db.GetConfig());
			config.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants.
				CONFIG_KEY_URL, url);
			config.Save();
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, path);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, url);
			modulesConfig.Save();
			SubmoduleStatusCommand command = new SubmoduleStatusCommand(db);
			IDictionary<string, SubmoduleStatus> statuses = command.Call();
			NUnit.Framework.Assert.IsNotNull(statuses);
			NUnit.Framework.Assert.AreEqual(1, statuses.Count);
			KeyValuePair<string, SubmoduleStatus> module = statuses.EntrySet().Iterator().Next
				();
			NUnit.Framework.Assert.IsNotNull(module);
			NUnit.Framework.Assert.AreEqual(path, module.Key);
			SubmoduleStatus status = module.Value;
			NUnit.Framework.Assert.IsNotNull(status);
			NUnit.Framework.Assert.AreEqual(path, status.GetPath());
			NUnit.Framework.Assert.AreEqual(id, status.GetIndexId());
			NUnit.Framework.Assert.AreEqual(SubmoduleStatusType.UNINITIALIZED, status.GetType
				());
		}

		private sealed class _PathEdit_211 : DirCacheEditor.PathEdit
		{
			public _PathEdit_211(ObjectId id, string baseArg1) : base(baseArg1)
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
		[NUnit.Framework.Test]
		public virtual void RepositoryWithInitializedSubmodule()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_256(id, path));
			editor.Commit();
			string url = "git://server/repo.git";
			StoredConfig config = ((FileBasedConfig)db.GetConfig());
			config.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants.
				CONFIG_KEY_URL, url);
			config.Save();
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, path);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, url);
			modulesConfig.Save();
			Repository subRepo = Git.Init().SetBare(false).SetDirectory(new FilePath(db.WorkTree
				, path)).Call().GetRepository();
			NUnit.Framework.Assert.IsNotNull(subRepo);
			RefUpdate update = subRepo.UpdateRef(Constants.HEAD, true);
			update.SetNewObjectId(id);
			update.ForceUpdate();
			SubmoduleStatusCommand command = new SubmoduleStatusCommand(db);
			IDictionary<string, SubmoduleStatus> statuses = command.Call();
			NUnit.Framework.Assert.IsNotNull(statuses);
			NUnit.Framework.Assert.AreEqual(1, statuses.Count);
			KeyValuePair<string, SubmoduleStatus> module = statuses.EntrySet().Iterator().Next
				();
			NUnit.Framework.Assert.IsNotNull(module);
			NUnit.Framework.Assert.AreEqual(path, module.Key);
			SubmoduleStatus status = module.Value;
			NUnit.Framework.Assert.IsNotNull(status);
			NUnit.Framework.Assert.AreEqual(path, status.GetPath());
			NUnit.Framework.Assert.AreEqual(id, status.GetIndexId());
			NUnit.Framework.Assert.AreEqual(SubmoduleStatusType.INITIALIZED, status.GetType()
				);
		}

		private sealed class _PathEdit_256 : DirCacheEditor.PathEdit
		{
			public _PathEdit_256(ObjectId id, string baseArg1) : base(baseArg1)
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
		[NUnit.Framework.Test]
		public virtual void RepositoryWithDifferentRevCheckedOutSubmodule()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_311(id, path));
			editor.Commit();
			string url = "git://server/repo.git";
			StoredConfig config = ((FileBasedConfig)db.GetConfig());
			config.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants.
				CONFIG_KEY_URL, url);
			config.Save();
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, path);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, url);
			modulesConfig.Save();
			Repository subRepo = Git.Init().SetBare(false).SetDirectory(new FilePath(db.WorkTree
				, path)).Call().GetRepository();
			NUnit.Framework.Assert.IsNotNull(subRepo);
			RefUpdate update = subRepo.UpdateRef(Constants.HEAD, true);
			update.SetNewObjectId(ObjectId.FromString("aaaa0000aaaa0000aaaa0000aaaa0000aaaa0000"
				));
			update.ForceUpdate();
			SubmoduleStatusCommand command = new SubmoduleStatusCommand(db);
			IDictionary<string, SubmoduleStatus> statuses = command.Call();
			NUnit.Framework.Assert.IsNotNull(statuses);
			NUnit.Framework.Assert.AreEqual(1, statuses.Count);
			KeyValuePair<string, SubmoduleStatus> module = statuses.EntrySet().Iterator().Next
				();
			NUnit.Framework.Assert.IsNotNull(module);
			NUnit.Framework.Assert.AreEqual(path, module.Key);
			SubmoduleStatus status = module.Value;
			NUnit.Framework.Assert.IsNotNull(status);
			NUnit.Framework.Assert.AreEqual(path, status.GetPath());
			NUnit.Framework.Assert.AreEqual(id, status.GetIndexId());
			NUnit.Framework.Assert.AreEqual(update.GetNewObjectId(), status.GetHeadId());
			NUnit.Framework.Assert.AreEqual(SubmoduleStatusType.REV_CHECKED_OUT, status.GetType
				());
		}

		private sealed class _PathEdit_311 : DirCacheEditor.PathEdit
		{
			public _PathEdit_311(ObjectId id, string baseArg1) : base(baseArg1)
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
