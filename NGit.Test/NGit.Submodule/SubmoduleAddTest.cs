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
using NGit;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Dircache;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Submodule;
using Sharpen;

namespace NGit.Submodule
{
	/// <summary>
	/// Unit tests of
	/// <see cref="NGit.Api.SubmoduleAddCommand">NGit.Api.SubmoduleAddCommand</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class SubmoduleAddTest : RepositoryTestCase
	{
		[NUnit.Framework.Test]
		public virtual void CommandWithNullPath()
		{
			try
			{
				new SubmoduleAddCommand(db).SetURI("uri").Call();
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual(JGitText.Get().pathNotConfigured, e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void CommandWithEmptyPath()
		{
			try
			{
				new SubmoduleAddCommand(db).SetPath(string.Empty).SetURI("uri").Call();
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual(JGitText.Get().pathNotConfigured, e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void CommandWithNullUri()
		{
			try
			{
				new SubmoduleAddCommand(db).SetPath("sub").Call();
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual(JGitText.Get().uriNotConfigured, e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void CommandWithEmptyUri()
		{
			try
			{
				new SubmoduleAddCommand(db).SetPath("sub").SetURI(string.Empty).Call();
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual(JGitText.Get().uriNotConfigured, e.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void AddSubmodule()
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
			Status status = Git.Wrap(db).Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetAdded().Contains(Constants.DOT_GIT_MODULES
				));
			NUnit.Framework.Assert.IsTrue(status.GetAdded().Contains(path));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void AddExistentSubmodule()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_154(id, path));
			editor.Commit();
			SubmoduleAddCommand command = new SubmoduleAddCommand(db);
			command.SetPath(path);
			command.SetURI("git://server/repo.git");
			try
			{
				command.Call();
				NUnit.Framework.Assert.Fail("Exception not thrown");
			}
			catch (JGitInternalException e)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().submoduleExists
					, path), e.Message);
			}
		}

		private sealed class _PathEdit_154 : DirCacheEditor.PathEdit
		{
			public _PathEdit_154(ObjectId id, string baseArg1) : base(baseArg1)
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

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void AddSubmoduleWithRelativeUri()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit commit = git.Commit().SetMessage("create file").Call();
			SubmoduleAddCommand command = new SubmoduleAddCommand(db);
			string path = "sub";
			string uri = "./.git";
			command.SetPath(path);
			command.SetURI(uri);
			Repository repo = command.Call();
			NUnit.Framework.Assert.IsNotNull(repo);
			SubmoduleWalk generator = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(generator.Next());
			NUnit.Framework.Assert.AreEqual(path, generator.GetPath());
			NUnit.Framework.Assert.AreEqual(commit, generator.GetObjectId());
			NUnit.Framework.Assert.AreEqual(uri, generator.GetModulesUrl());
			NUnit.Framework.Assert.AreEqual(path, generator.GetModulesPath());
			string fullUri = db.Directory.GetAbsolutePath();
			if (FilePath.separatorChar == '\\')
			{
				fullUri = fullUri.Replace('\\', '/');
			}
			NUnit.Framework.Assert.AreEqual(fullUri, generator.GetConfigUrl());
			NUnit.Framework.Assert.IsNotNull(generator.GetRepository());
			NUnit.Framework.Assert.AreEqual(fullUri, generator.GetRepository().GetConfig().GetString
				(ConfigConstants.CONFIG_REMOTE_SECTION, Constants.DEFAULT_REMOTE_NAME, ConfigConstants
				.CONFIG_KEY_URL));
			NUnit.Framework.Assert.AreEqual(commit, repo.Resolve(Constants.HEAD));
			Status status = Git.Wrap(db).Status().Call();
			NUnit.Framework.Assert.IsTrue(status.GetAdded().Contains(Constants.DOT_GIT_MODULES
				));
			NUnit.Framework.Assert.IsTrue(status.GetAdded().Contains(path));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void AddSubmoduleWithExistingSubmoduleDefined()
		{
			string path1 = "sub1";
			string url1 = "git://server/repo1.git";
			string path2 = "sub2";
			FileBasedConfig modulesConfig = new FileBasedConfig(new FilePath(db.WorkTree, Constants
				.DOT_GIT_MODULES), db.FileSystem);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path1, ConfigConstants
				.CONFIG_KEY_PATH, path1);
			modulesConfig.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path1, ConfigConstants
				.CONFIG_KEY_URL, url1);
			modulesConfig.Save();
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			NUnit.Framework.Assert.IsNotNull(git.Commit().SetMessage("create file").Call());
			SubmoduleAddCommand command = new SubmoduleAddCommand(db);
			command.SetPath(path2);
			string url2 = db.Directory.ToURI().ToString();
			command.SetURI(url2);
			NUnit.Framework.Assert.IsNotNull(command.Call());
			modulesConfig.Load();
			NUnit.Framework.Assert.AreEqual(path1, modulesConfig.GetString(ConfigConstants.CONFIG_SUBMODULE_SECTION
				, path1, ConfigConstants.CONFIG_KEY_PATH));
			NUnit.Framework.Assert.AreEqual(url1, modulesConfig.GetString(ConfigConstants.CONFIG_SUBMODULE_SECTION
				, path1, ConfigConstants.CONFIG_KEY_URL));
			NUnit.Framework.Assert.AreEqual(path2, modulesConfig.GetString(ConfigConstants.CONFIG_SUBMODULE_SECTION
				, path2, ConfigConstants.CONFIG_KEY_PATH));
			NUnit.Framework.Assert.AreEqual(url2, modulesConfig.GetString(ConfigConstants.CONFIG_SUBMODULE_SECTION
				, path2, ConfigConstants.CONFIG_KEY_URL));
		}
	}
}
