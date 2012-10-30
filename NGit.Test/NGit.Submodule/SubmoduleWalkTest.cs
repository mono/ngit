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
using NGit.Dircache;
using NGit.Junit;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Submodule;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Submodule
{
	/// <summary>
	/// Unit tests of
	/// <see cref="SubmoduleWalk">SubmoduleWalk</see>
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class SubmoduleWalkTest : RepositoryTestCase
	{
		private TestRepository<FileRepository> testDb;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			testDb = new TestRepository<FileRepository>(db);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void RepositoryWithNoSubmodules()
		{
			SubmoduleWalk gen = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsFalse(gen.Next());
			NUnit.Framework.Assert.IsNull(gen.GetPath());
			NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, gen.GetObjectId());
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void RepositoryWithRootLevelSubmodule()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_108(id, path));
			editor.Commit();
			SubmoduleWalk gen = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(gen.Next());
			NUnit.Framework.Assert.AreEqual(path, gen.GetPath());
			NUnit.Framework.Assert.AreEqual(id, gen.GetObjectId());
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, path), gen.GetDirectory
				());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUpdate());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUrl());
			NUnit.Framework.Assert.IsNull(gen.GetModulesPath());
			NUnit.Framework.Assert.IsNull(gen.GetModulesUpdate());
			NUnit.Framework.Assert.IsNull(gen.GetModulesUrl());
			NUnit.Framework.Assert.IsNull(gen.GetRepository());
			NUnit.Framework.Assert.IsFalse(gen.Next());
		}

		private sealed class _PathEdit_108 : DirCacheEditor.PathEdit
		{
			public _PathEdit_108(ObjectId id, string baseArg1) : base(baseArg1)
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
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void RepositoryWithRootLevelSubmoduleAbsoluteRef()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			FilePath dotGit = new FilePath(db.WorkTree, path + FilePath.separatorChar + Constants
				.DOT_GIT);
			if (!dotGit.GetParentFile().Exists())
			{
				dotGit.GetParentFile().Mkdirs();
			}
			FilePath modulesGitDir = new FilePath(db.Directory, "modules" + FilePath.separatorChar
				 + path);
			new FileWriter(dotGit).Append("gitdir: " + modulesGitDir.GetAbsolutePath()).Close
				();
			FileRepositoryBuilder builder = new FileRepositoryBuilder();
			builder.SetWorkTree(new FilePath(db.WorkTree, path));
			builder.Build().Create();
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_153(id, path));
			editor.Commit();
			SubmoduleWalk gen = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(gen.Next());
			NUnit.Framework.Assert.AreEqual(path, gen.GetPath());
			NUnit.Framework.Assert.AreEqual(id, gen.GetObjectId());
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, path), gen.GetDirectory
				());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUpdate());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUrl());
			NUnit.Framework.Assert.IsNull(gen.GetModulesPath());
			NUnit.Framework.Assert.IsNull(gen.GetModulesUpdate());
			NUnit.Framework.Assert.IsNull(gen.GetModulesUrl());
			Repository subRepo = gen.GetRepository();
			AddRepoToClose(subRepo);
			NUnit.Framework.Assert.IsNotNull(subRepo);
			NUnit.Framework.Assert.AreEqual(modulesGitDir, subRepo.Directory);
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, path), subRepo.WorkTree
				);
			NUnit.Framework.Assert.IsFalse(gen.Next());
		}

		private sealed class _PathEdit_153 : DirCacheEditor.PathEdit
		{
			public _PathEdit_153(ObjectId id, string baseArg1) : base(baseArg1)
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
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void RepositoryWithRootLevelSubmoduleRelativeRef()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			FilePath dotGit = new FilePath(db.WorkTree, path + FilePath.separatorChar + Constants
				.DOT_GIT);
			if (!dotGit.GetParentFile().Exists())
			{
				dotGit.GetParentFile().Mkdirs();
			}
			FilePath modulesGitDir = new FilePath(db.Directory, "modules" + FilePath.separatorChar
				 + path);
			new FileWriter(dotGit).Append("gitdir: " + "../" + Constants.DOT_GIT + "/modules/"
				 + path).Close();
			FileRepositoryBuilder builder = new FileRepositoryBuilder();
			builder.SetWorkTree(new FilePath(db.WorkTree, path));
			builder.Build().Create();
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_203(id, path));
			editor.Commit();
			SubmoduleWalk gen = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(gen.Next());
			NUnit.Framework.Assert.AreEqual(path, gen.GetPath());
			NUnit.Framework.Assert.AreEqual(id, gen.GetObjectId());
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, path), gen.GetDirectory
				());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUpdate());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUrl());
			NUnit.Framework.Assert.IsNull(gen.GetModulesPath());
			NUnit.Framework.Assert.IsNull(gen.GetModulesUpdate());
			NUnit.Framework.Assert.IsNull(gen.GetModulesUrl());
			Repository subRepo = gen.GetRepository();
			AddRepoToClose(subRepo);
			NUnit.Framework.Assert.IsNotNull(subRepo);
			NUnit.Framework.Assert.AreEqual(modulesGitDir, subRepo.Directory);
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, path), subRepo.WorkTree
				);
			NUnit.Framework.Assert.IsFalse(gen.Next());
		}

		private sealed class _PathEdit_203 : DirCacheEditor.PathEdit
		{
			public _PathEdit_203(ObjectId id, string baseArg1) : base(baseArg1)
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
		/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
		[NUnit.Framework.Test]
		public virtual void RepositoryWithNestedSubmodule()
		{
			ObjectId id = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub/dir/final";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_238(id, path));
			editor.Commit();
			SubmoduleWalk gen = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(gen.Next());
			NUnit.Framework.Assert.AreEqual(path, gen.GetPath());
			NUnit.Framework.Assert.AreEqual(id, gen.GetObjectId());
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, path), gen.GetDirectory
				());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUpdate());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUrl());
			NUnit.Framework.Assert.IsNull(gen.GetModulesPath());
			NUnit.Framework.Assert.IsNull(gen.GetModulesUpdate());
			NUnit.Framework.Assert.IsNull(gen.GetModulesUrl());
			NUnit.Framework.Assert.IsNull(gen.GetRepository());
			NUnit.Framework.Assert.IsFalse(gen.Next());
		}

		private sealed class _PathEdit_238 : DirCacheEditor.PathEdit
		{
			public _PathEdit_238(ObjectId id, string baseArg1) : base(baseArg1)
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
		public virtual void GeneratorFilteredToOneOfTwoSubmodules()
		{
			ObjectId id1 = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path1 = "sub1";
			ObjectId id2 = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1235");
			string path2 = "sub2";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_271(id1, path1));
			editor.Add(new _PathEdit_278(id2, path2));
			editor.Commit();
			SubmoduleWalk gen = SubmoduleWalk.ForIndex(db);
			gen.SetFilter(PathFilter.Create(path1));
			NUnit.Framework.Assert.IsTrue(gen.Next());
			NUnit.Framework.Assert.AreEqual(path1, gen.GetPath());
			NUnit.Framework.Assert.AreEqual(id1, gen.GetObjectId());
			NUnit.Framework.Assert.IsFalse(gen.Next());
		}

		private sealed class _PathEdit_271 : DirCacheEditor.PathEdit
		{
			public _PathEdit_271(ObjectId id1, string baseArg1) : base(baseArg1)
			{
				this.id1 = id1;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(id1);
			}

			private readonly ObjectId id1;
		}

		private sealed class _PathEdit_278 : DirCacheEditor.PathEdit
		{
			public _PathEdit_278(ObjectId id2, string baseArg1) : base(baseArg1)
			{
				this.id2 = id2;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(id2);
			}

			private readonly ObjectId id2;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void IndexWithGitmodules()
		{
			ObjectId subId = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			Config gitmodules = new Config();
			gitmodules.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, "sub");
			// Different config in the index should be overridden by the working tree.
			gitmodules.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, "git://example.com/bad");
			RevBlob gitmodulesBlob = testDb.Blob(gitmodules.ToText());
			gitmodules.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, "git://example.com/sub");
			WriteTrashFile(Constants.DOT_GIT_MODULES, gitmodules.ToText());
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_315(subId, path));
			editor.Add(new _PathEdit_322(gitmodulesBlob, Constants.DOT_GIT_MODULES));
			editor.Commit();
			SubmoduleWalk gen = SubmoduleWalk.ForIndex(db);
			NUnit.Framework.Assert.IsTrue(gen.Next());
			NUnit.Framework.Assert.AreEqual(path, gen.GetPath());
			NUnit.Framework.Assert.AreEqual(subId, gen.GetObjectId());
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, path), gen.GetDirectory
				());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUpdate());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUrl());
			NUnit.Framework.Assert.AreEqual("sub", gen.GetModulesPath());
			NUnit.Framework.Assert.IsNull(gen.GetModulesUpdate());
			NUnit.Framework.Assert.AreEqual("git://example.com/sub", gen.GetModulesUrl());
			NUnit.Framework.Assert.IsNull(gen.GetRepository());
			NUnit.Framework.Assert.IsFalse(gen.Next());
		}

		private sealed class _PathEdit_315 : DirCacheEditor.PathEdit
		{
			public _PathEdit_315(ObjectId subId, string baseArg1) : base(baseArg1)
			{
				this.subId = subId;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(subId);
			}

			private readonly ObjectId subId;
		}

		private sealed class _PathEdit_322 : DirCacheEditor.PathEdit
		{
			public _PathEdit_322(RevBlob gitmodulesBlob, string baseArg1) : base(baseArg1)
			{
				this.gitmodulesBlob = gitmodulesBlob;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.REGULAR_FILE;
				ent.SetObjectId(gitmodulesBlob);
			}

			private readonly RevBlob gitmodulesBlob;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TreeIdWithGitmodules()
		{
			ObjectId subId = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			Config gitmodules = new Config();
			gitmodules.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, "sub");
			gitmodules.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, "git://example.com/sub");
			RevCommit commit = testDb.GetRevWalk().ParseCommit(testDb.Commit().NoParents().Add
				(Constants.DOT_GIT_MODULES, gitmodules.ToText()).Edit(new _PathEdit_360(subId, path
				)).Create());
			SubmoduleWalk gen = SubmoduleWalk.ForPath(db, commit.Tree, "sub");
			NUnit.Framework.Assert.AreEqual(path, gen.GetPath());
			NUnit.Framework.Assert.AreEqual(subId, gen.GetObjectId());
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, path), gen.GetDirectory
				());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUpdate());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUrl());
			NUnit.Framework.Assert.AreEqual("sub", gen.GetModulesPath());
			NUnit.Framework.Assert.IsNull(gen.GetModulesUpdate());
			NUnit.Framework.Assert.AreEqual("git://example.com/sub", gen.GetModulesUrl());
			NUnit.Framework.Assert.IsNull(gen.GetRepository());
			NUnit.Framework.Assert.IsFalse(gen.Next());
		}

		private sealed class _PathEdit_360 : DirCacheEditor.PathEdit
		{
			public _PathEdit_360(ObjectId subId, string baseArg1) : base(baseArg1)
			{
				this.subId = subId;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(subId);
			}

			private readonly ObjectId subId;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTreeIteratorWithGitmodules()
		{
			ObjectId subId = ObjectId.FromString("abcd1234abcd1234abcd1234abcd1234abcd1234");
			string path = "sub";
			Config gitmodules = new Config();
			gitmodules.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_PATH, "sub");
			gitmodules.SetString(ConfigConstants.CONFIG_SUBMODULE_SECTION, path, ConfigConstants
				.CONFIG_KEY_URL, "git://example.com/sub");
			RevCommit commit = testDb.GetRevWalk().ParseCommit(testDb.Commit().NoParents().Add
				(Constants.DOT_GIT_MODULES, gitmodules.ToText()).Edit(new _PathEdit_397(subId, path
				)).Create());
			CanonicalTreeParser p = new CanonicalTreeParser();
			p.Reset(testDb.GetRevWalk().GetObjectReader(), commit.Tree);
			SubmoduleWalk gen = SubmoduleWalk.ForPath(db, p, "sub");
			NUnit.Framework.Assert.AreEqual(path, gen.GetPath());
			NUnit.Framework.Assert.AreEqual(subId, gen.GetObjectId());
			NUnit.Framework.Assert.AreEqual(new FilePath(db.WorkTree, path), gen.GetDirectory
				());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUpdate());
			NUnit.Framework.Assert.IsNull(gen.GetConfigUrl());
			NUnit.Framework.Assert.AreEqual("sub", gen.GetModulesPath());
			NUnit.Framework.Assert.IsNull(gen.GetModulesUpdate());
			NUnit.Framework.Assert.AreEqual("git://example.com/sub", gen.GetModulesUrl());
			NUnit.Framework.Assert.IsNull(gen.GetRepository());
			NUnit.Framework.Assert.IsFalse(gen.Next());
		}

		private sealed class _PathEdit_397 : DirCacheEditor.PathEdit
		{
			public _PathEdit_397(ObjectId subId, string baseArg1) : base(baseArg1)
			{
				this.subId = subId;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(subId);
			}

			private readonly ObjectId subId;
		}
	}
}
