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
using System.IO;
using NGit;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Dircache;
using NGit.Storage.File;
using NGit.Submodule;
using Sharpen;

namespace NGit.Submodule
{
	/// <summary>
	/// Unit tests of
	/// <see cref="NGit.Api.SubmoduleInitCommand">NGit.Api.SubmoduleInitCommand</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class SubmoduleInitTest : RepositoryTestCase
	{
		[NUnit.Framework.Test]
		public virtual void RepositoryWithNoSubmodules()
		{
			SubmoduleInitCommand command = new SubmoduleInitCommand(db);
			ICollection<string> modules = command.Call();
			NUnit.Framework.Assert.IsNotNull(modules);
			NUnit.Framework.Assert.IsTrue(modules.IsEmpty());
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void RepositoryWithUninitializedModule()
		{
			string path = AddSubmoduleToIndex();
			SubmoduleWalk generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.IsNull(generator.GetConfigUrl());
			NUnit.Framework.Assert.IsNull(generator.GetConfigUpdate());
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
			SubmoduleInitCommand command = new SubmoduleInitCommand(db);
			ICollection<string> modules = command.Call();
			NUnit.Framework.Assert.IsNotNull(modules);
			NUnit.Framework.Assert.AreEqual(1, modules.Count);
			NUnit.Framework.Assert.AreEqual(path, modules.Iterator().Next());
			generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.AreEqual(url, generator.GetConfigUrl());
			NUnit.Framework.Assert.AreEqual(update, generator.GetConfigUpdate());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void ResolveSameLevelRelativeUrl()
		{
			string path = AddSubmoduleToIndex();
			string @base = "git://server/repo.git";
			FileBasedConfig config = ((FileBasedConfig)db.GetConfig());
			config.SetString(ConfigConstants.CONFIG_REMOTE_SECTION, Constants.DEFAULT_REMOTE_NAME
				, ConfigConstants.CONFIG_KEY_URL, @base);
			config.Save();
			SubmoduleWalk generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.IsNull(generator.GetConfigUrl());
			NUnit.Framework.Assert.IsNull(generator.GetConfigUpdate());
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, path);
			string url = "./sub.git";
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, url);
			string update = "rebase";
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_UPDATE, update);
			modulesConfig.Save();
			SubmoduleInitCommand command = new SubmoduleInitCommand(db);
			ICollection<string> modules = command.Call();
			NUnit.Framework.Assert.IsNotNull(modules);
			NUnit.Framework.Assert.AreEqual(1, modules.Count);
			NUnit.Framework.Assert.AreEqual(path, modules.Iterator().Next());
			generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.AreEqual("git://server/repo.git/sub.git", generator.GetConfigUrl
				());
			NUnit.Framework.Assert.AreEqual(update, generator.GetConfigUpdate());
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void ResolveOneLevelHigherRelativeUrl()
		{
			string path = AddSubmoduleToIndex();
			string @base = "git://server/repo.git";
			FileBasedConfig config = ((FileBasedConfig)db.GetConfig());
			config.SetString(ConfigConstants.CONFIG_REMOTE_SECTION, Constants.DEFAULT_REMOTE_NAME
				, ConfigConstants.CONFIG_KEY_URL, @base);
			config.Save();
			SubmoduleWalk generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.IsNull(generator.GetConfigUrl());
			NUnit.Framework.Assert.IsNull(generator.GetConfigUpdate());
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, path);
			string url = "../sub.git";
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, url);
			string update = "rebase";
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_UPDATE, update);
			modulesConfig.Save();
			SubmoduleInitCommand command = new SubmoduleInitCommand(db);
			ICollection<string> modules = command.Call();
			NUnit.Framework.Assert.IsNotNull(modules);
			NUnit.Framework.Assert.AreEqual(1, modules.Count);
			NUnit.Framework.Assert.AreEqual(path, modules.Iterator().Next());
			generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.AreEqual("git://server/sub.git", generator.GetConfigUrl());
			NUnit.Framework.Assert.AreEqual(update, generator.GetConfigUpdate());
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void ResolveTwoLevelHigherRelativeUrl()
		{
			string path = AddSubmoduleToIndex();
			string @base = "git://server/repo.git";
			FileBasedConfig config = ((FileBasedConfig)db.GetConfig());
			config.SetString(ConfigConstants.CONFIG_REMOTE_SECTION, Constants.DEFAULT_REMOTE_NAME
				, ConfigConstants.CONFIG_KEY_URL, @base);
			config.Save();
			SubmoduleWalk generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.IsNull(generator.GetConfigUrl());
			NUnit.Framework.Assert.IsNull(generator.GetConfigUpdate());
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, path);
			string url = "../../server2/sub.git";
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, url);
			string update = "rebase";
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_UPDATE, update);
			modulesConfig.Save();
			SubmoduleInitCommand command = new SubmoduleInitCommand(db);
			ICollection<string> modules = command.Call();
			NUnit.Framework.Assert.IsNotNull(modules);
			NUnit.Framework.Assert.AreEqual(1, modules.Count);
			NUnit.Framework.Assert.AreEqual(path, modules.Iterator().Next());
			generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.AreEqual("git://server2/sub.git", generator.GetConfigUrl()
				);
			NUnit.Framework.Assert.AreEqual(update, generator.GetConfigUpdate());
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void ResolveWorkingDirectoryRelativeUrl()
		{
			string path = AddSubmoduleToIndex();
			string @base = db.WorkTree.GetAbsolutePath();
			if (FilePath.separatorChar == '\\')
			{
				@base = @base.Replace('\\', '/');
			}
			FileBasedConfig config = ((FileBasedConfig)db.GetConfig());
			config.SetString(ConfigConstants.CONFIG_REMOTE_SECTION, Constants.DEFAULT_REMOTE_NAME
				, ConfigConstants.CONFIG_KEY_URL, null);
			config.Save();
			SubmoduleWalk generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.IsNull(generator.GetConfigUrl());
			NUnit.Framework.Assert.IsNull(generator.GetConfigUpdate());
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, path);
			string url = "./sub.git";
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, url);
			string update = "rebase";
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_UPDATE, update);
			modulesConfig.Save();
			SubmoduleInitCommand command = new SubmoduleInitCommand(db);
			ICollection<string> modules = command.Call();
			NUnit.Framework.Assert.IsNotNull(modules);
			NUnit.Framework.Assert.AreEqual(1, modules.Count);
			NUnit.Framework.Assert.AreEqual(path, modules.Iterator().Next());
			generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.AreEqual(@base + "/sub.git", generator.GetConfigUrl());
			NUnit.Framework.Assert.AreEqual(update, generator.GetConfigUpdate());
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void ResolveInvalidParentUrl()
		{
			string path = AddSubmoduleToIndex();
			string @base = "no_slash";
			FileBasedConfig config = ((FileBasedConfig)db.GetConfig());
			config.SetString(ConfigConstants.CONFIG_REMOTE_SECTION, Constants.DEFAULT_REMOTE_NAME
				, ConfigConstants.CONFIG_KEY_URL, @base);
			config.Save();
			SubmoduleWalk generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.IsNull(generator.GetConfigUrl());
			NUnit.Framework.Assert.IsNull(generator.GetConfigUpdate());
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, path);
			string url = "../sub.git";
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, url);
			modulesConfig.Save();
			try
			{
				new SubmoduleInitCommand(db).Call();
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (JGitInternalException e)
			{
				NUnit.Framework.Assert.IsTrue(e.InnerException is IOException);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private string AddSubmoduleToIndex()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_322(id, path));
			editor.Commit();
			return path;
		}

		private sealed class _PathEdit_322 : DirCacheEditor.PathEdit
		{
			public _PathEdit_322(ObjectId id, string baseArg1) : base(baseArg1)
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
