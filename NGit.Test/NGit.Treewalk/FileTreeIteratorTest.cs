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
using NGit.Dircache;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using NGit.Util;
using Sharpen;

namespace NGit.Treewalk
{
	[NUnit.Framework.TestFixture]
	public class FileTreeIteratorTest : RepositoryTestCase
	{
		private readonly string[] paths = new string[] { "a,", "a,b", "a/b", "a0b" };

		private long[] mtime;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			// We build the entries backwards so that on POSIX systems we
			// are likely to get the entries in the trash directory in the
			// opposite order of what they should be in for the iteration.
			// This should stress the sorting code better than doing it in
			// the correct order.
			//
			mtime = new long[paths.Length];
			for (int i = paths.Length - 1; i >= 0; i--)
			{
				string s = paths[i];
				WriteTrashFile(s, s);
				mtime[i] = new FilePath(trash, s).LastModified();
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyIfRootIsFile()
		{
			FilePath r = new FilePath(trash, paths[0]);
			NUnit.Framework.Assert.IsTrue(r.IsFile());
			FileTreeIterator fti = new FileTreeIterator(r, db.FileSystem, ((FileBasedConfig)db
				.GetConfig()).Get(WorkingTreeOptions.KEY));
			NUnit.Framework.Assert.IsTrue(fti.First);
			NUnit.Framework.Assert.IsTrue(fti.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyIfRootDoesNotExist()
		{
			FilePath r = new FilePath(trash, "not-existing-file");
			NUnit.Framework.Assert.IsFalse(r.Exists());
			FileTreeIterator fti = new FileTreeIterator(r, db.FileSystem, ((FileBasedConfig)db
				.GetConfig()).Get(WorkingTreeOptions.KEY));
			NUnit.Framework.Assert.IsTrue(fti.First);
			NUnit.Framework.Assert.IsTrue(fti.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEmptyIfRootIsEmpty()
		{
			FilePath r = new FilePath(trash, "not-existing-file");
			NUnit.Framework.Assert.IsFalse(r.Exists());
			FileUtils.Mkdir(r);
			FileTreeIterator fti = new FileTreeIterator(r, db.FileSystem, ((FileBasedConfig)db
				.GetConfig()).Get(WorkingTreeOptions.KEY));
			NUnit.Framework.Assert.IsTrue(fti.First);
			NUnit.Framework.Assert.IsTrue(fti.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSimpleIterate()
		{
			FileTreeIterator top = new FileTreeIterator(trash, db.FileSystem, ((FileBasedConfig
				)db.GetConfig()).Get(WorkingTreeOptions.KEY));
			NUnit.Framework.Assert.IsTrue(top.First);
			NUnit.Framework.Assert.IsFalse(top.Eof);
			NUnit.Framework.Assert.AreEqual(FileMode.REGULAR_FILE.GetBits(), top.mode);
			NUnit.Framework.Assert.AreEqual(paths[0], NameOf(top));
			NUnit.Framework.Assert.AreEqual(paths[0].Length, top.GetEntryLength());
			NUnit.Framework.Assert.AreEqual(mtime[0], top.GetEntryLastModified());
			top.Next(1);
			NUnit.Framework.Assert.IsFalse(top.First);
			NUnit.Framework.Assert.IsFalse(top.Eof);
			NUnit.Framework.Assert.AreEqual(FileMode.REGULAR_FILE.GetBits(), top.mode);
			NUnit.Framework.Assert.AreEqual(paths[1], NameOf(top));
			NUnit.Framework.Assert.AreEqual(paths[1].Length, top.GetEntryLength());
			NUnit.Framework.Assert.AreEqual(mtime[1], top.GetEntryLastModified());
			top.Next(1);
			NUnit.Framework.Assert.IsFalse(top.First);
			NUnit.Framework.Assert.IsFalse(top.Eof);
			NUnit.Framework.Assert.AreEqual(FileMode.TREE.GetBits(), top.mode);
			ObjectReader reader = db.NewObjectReader();
			AbstractTreeIterator sub = top.CreateSubtreeIterator(reader);
			NUnit.Framework.Assert.IsTrue(sub is FileTreeIterator);
			FileTreeIterator subfti = (FileTreeIterator)sub;
			NUnit.Framework.Assert.IsTrue(sub.First);
			NUnit.Framework.Assert.IsFalse(sub.Eof);
			NUnit.Framework.Assert.AreEqual(paths[2], NameOf(sub));
			NUnit.Framework.Assert.AreEqual(paths[2].Length, subfti.GetEntryLength());
			NUnit.Framework.Assert.AreEqual(mtime[2], subfti.GetEntryLastModified());
			sub.Next(1);
			NUnit.Framework.Assert.IsTrue(sub.Eof);
			top.Next(1);
			NUnit.Framework.Assert.IsFalse(top.First);
			NUnit.Framework.Assert.IsFalse(top.Eof);
			NUnit.Framework.Assert.AreEqual(FileMode.REGULAR_FILE.GetBits(), top.mode);
			NUnit.Framework.Assert.AreEqual(paths[3], NameOf(top));
			NUnit.Framework.Assert.AreEqual(paths[3].Length, top.GetEntryLength());
			NUnit.Framework.Assert.AreEqual(mtime[3], top.GetEntryLastModified());
			top.Next(1);
			NUnit.Framework.Assert.IsTrue(top.Eof);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestComputeFileObjectId()
		{
			FileTreeIterator top = new FileTreeIterator(trash, db.FileSystem, ((FileBasedConfig
				)db.GetConfig()).Get(WorkingTreeOptions.KEY));
			MessageDigest md = Constants.NewMessageDigest();
			md.Update(Constants.EncodeASCII(Constants.TYPE_BLOB));
			md.Update(unchecked((byte)' '));
			md.Update(Constants.EncodeASCII(paths[0].Length));
			md.Update(unchecked((byte)0));
			md.Update(Constants.Encode(paths[0]));
			ObjectId expect = ObjectId.FromRaw(md.Digest());
			NUnit.Framework.Assert.AreEqual(expect, top.EntryObjectId);
			// Verify it was cached by removing the file and getting it again.
			//
			FileUtils.Delete(new FilePath(trash, paths[0]));
			NUnit.Framework.Assert.AreEqual(expect, top.EntryObjectId);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDirCacheMatchingId()
		{
			FilePath f = WriteTrashFile("file", "content");
			Git git = new Git(db);
			WriteTrashFile("file", "content");
			FsTick(f);
			git.Add().AddFilepattern("file").Call();
			DirCacheEntry dce = db.ReadDirCache().GetEntry("file");
			TreeWalk tw = new TreeWalk(db);
			FileTreeIterator fti = new FileTreeIterator(trash, db.FileSystem, ((FileBasedConfig
				)db.GetConfig()).Get(WorkingTreeOptions.KEY));
			tw.AddTree(fti);
			DirCacheIterator dci = new DirCacheIterator(db.ReadDirCache());
			tw.AddTree(dci);
			fti.SetDirCacheIterator(tw, 1);
			while (tw.Next() && !tw.PathString.Equals("file"))
			{
			}
			//
			NUnit.Framework.Assert.AreEqual(WorkingTreeIterator.MetadataDiff.EQUAL, fti.CompareMetadata
				(dce));
			ObjectId fromRaw = ObjectId.FromRaw(fti.IdBuffer, fti.IdOffset);
			NUnit.Framework.Assert.AreEqual("6b584e8ece562ebffc15d38808cd6b98fc3d97ea", fromRaw
				.GetName());
			NUnit.Framework.Assert.IsFalse(fti.IsModified(dce, false));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIsModifiedSymlink()
		{
			FilePath f = WriteTrashFile("symlink", "content");
			Git git = new Git(db);
			git.Add().AddFilepattern("symlink").Call();
			git.Commit().SetMessage("commit").Call();
			// Modify previously committed DirCacheEntry and write it back to disk
			DirCacheEntry dce = db.ReadDirCache().GetEntry("symlink");
			dce.FileMode = FileMode.SYMLINK;
			DirCacheCheckout.CheckoutEntry(db, f, dce);
			FileTreeIterator fti = new FileTreeIterator(trash, db.FileSystem, ((FileBasedConfig
				)db.GetConfig()).Get(WorkingTreeOptions.KEY));
			while (!fti.EntryPathString.Equals("symlink"))
			{
				fti.Next(1);
			}
			NUnit.Framework.Assert.IsFalse(fti.IsModified(dce, false));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIsModifiedFileSmudged()
		{
			FilePath f = WriteTrashFile("file", "content");
			Git git = new Git(db);
			// The idea of this test is to check the smudged handling
			// Hopefully fsTick will make sure our entry gets smudged
			FsTick(f);
			WriteTrashFile("file", "content");
			git.Add().AddFilepattern("file").Call();
			WriteTrashFile("file", "conten2");
			DirCacheEntry dce = db.ReadDirCache().GetEntry("file");
			FileTreeIterator fti = new FileTreeIterator(trash, db.FileSystem, ((FileBasedConfig
				)db.GetConfig()).Get(WorkingTreeOptions.KEY));
			while (!fti.EntryPathString.Equals("file"))
			{
				fti.Next(1);
			}
			// If the fsTick trick does not work we could skip the compareMetaData
			// test and hope that we are usually testing the intended code path.
			NUnit.Framework.Assert.AreEqual(WorkingTreeIterator.MetadataDiff.SMUDGED, fti.CompareMetadata
				(dce));
			NUnit.Framework.Assert.IsTrue(fti.IsModified(dce, false));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void SubmoduleHeadMatchesIndex()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit id = git.Commit().SetMessage("create file").Call();
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_278(id, path));
			editor.Commit();
			Git.CloneRepository().SetURI(db.Directory.ToURI().ToString()).SetDirectory(new FilePath
				(db.WorkTree, path)).Call().GetRepository().Close();
			TreeWalk walk = new TreeWalk(db);
			DirCacheIterator indexIter = new DirCacheIterator(db.ReadDirCache());
			FileTreeIterator workTreeIter = new FileTreeIterator(db);
			walk.AddTree(indexIter);
			walk.AddTree(workTreeIter);
			walk.Filter = PathFilter.Create(path);
			NUnit.Framework.Assert.IsTrue(walk.Next());
			NUnit.Framework.Assert.IsTrue(indexIter.IdEqual(workTreeIter));
		}

		private sealed class _PathEdit_278 : DirCacheEditor.PathEdit
		{
			public _PathEdit_278(RevCommit id, string baseArg1) : base(baseArg1)
			{
				this.id = id;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(id);
			}

			private readonly RevCommit id;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void SubmoduleWithNoGitDirectory()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit id = git.Commit().SetMessage("create file").Call();
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_311(id, path));
			editor.Commit();
			FilePath submoduleRoot = new FilePath(db.WorkTree, path);
			NUnit.Framework.Assert.IsTrue(submoduleRoot.Mkdir());
			NUnit.Framework.Assert.IsTrue(new FilePath(submoduleRoot, Constants.DOT_GIT).Mkdir
				());
			TreeWalk walk = new TreeWalk(db);
			DirCacheIterator indexIter = new DirCacheIterator(db.ReadDirCache());
			FileTreeIterator workTreeIter = new FileTreeIterator(db);
			walk.AddTree(indexIter);
			walk.AddTree(workTreeIter);
			walk.Filter = PathFilter.Create(path);
			NUnit.Framework.Assert.IsTrue(walk.Next());
			NUnit.Framework.Assert.IsFalse(indexIter.IdEqual(workTreeIter));
			NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, workTreeIter.EntryObjectId);
		}

		private sealed class _PathEdit_311 : DirCacheEditor.PathEdit
		{
			public _PathEdit_311(RevCommit id, string baseArg1) : base(baseArg1)
			{
				this.id = id;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(id);
			}

			private readonly RevCommit id;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void SubmoduleWithNoHead()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit id = git.Commit().SetMessage("create file").Call();
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_345(id, path));
			editor.Commit();
			NUnit.Framework.Assert.IsNotNull(Git.Init().SetDirectory(new FilePath(db.WorkTree
				, path)).Call().GetRepository());
			TreeWalk walk = new TreeWalk(db);
			DirCacheIterator indexIter = new DirCacheIterator(db.ReadDirCache());
			FileTreeIterator workTreeIter = new FileTreeIterator(db);
			walk.AddTree(indexIter);
			walk.AddTree(workTreeIter);
			walk.Filter = PathFilter.Create(path);
			NUnit.Framework.Assert.IsTrue(walk.Next());
			NUnit.Framework.Assert.IsFalse(indexIter.IdEqual(workTreeIter));
			NUnit.Framework.Assert.AreEqual(ObjectId.ZeroId, workTreeIter.EntryObjectId);
		}

		private sealed class _PathEdit_345 : DirCacheEditor.PathEdit
		{
			public _PathEdit_345(RevCommit id, string baseArg1) : base(baseArg1)
			{
				this.id = id;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(id);
			}

			private readonly RevCommit id;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void SubmoduleDirectoryIterator()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit id = git.Commit().SetMessage("create file").Call();
			string path = "sub";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_378(id, path));
			editor.Commit();
			Git.CloneRepository().SetURI(db.Directory.ToURI().ToString()).SetDirectory(new FilePath
				(db.WorkTree, path)).Call().GetRepository().Close();
			TreeWalk walk = new TreeWalk(db);
			DirCacheIterator indexIter = new DirCacheIterator(db.ReadDirCache());
			FileTreeIterator workTreeIter = new FileTreeIterator(db.WorkTree, db.FileSystem, 
				((FileBasedConfig)db.GetConfig()).Get(WorkingTreeOptions.KEY));
			walk.AddTree(indexIter);
			walk.AddTree(workTreeIter);
			walk.Filter = PathFilter.Create(path);
			NUnit.Framework.Assert.IsTrue(walk.Next());
			NUnit.Framework.Assert.IsTrue(indexIter.IdEqual(workTreeIter));
		}

		private sealed class _PathEdit_378 : DirCacheEditor.PathEdit
		{
			public _PathEdit_378(RevCommit id, string baseArg1) : base(baseArg1)
			{
				this.id = id;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(id);
			}

			private readonly RevCommit id;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void SubmoduleNestedWithHeadMatchingIndex()
		{
			Git git = new Git(db);
			WriteTrashFile("file.txt", "content");
			git.Add().AddFilepattern("file.txt").Call();
			RevCommit id = git.Commit().SetMessage("create file").Call();
			string path = "sub/dir1/dir2";
			DirCache cache = db.LockDirCache();
			DirCacheEditor editor = cache.Editor();
			editor.Add(new _PathEdit_412(id, path));
			editor.Commit();
			Git.CloneRepository().SetURI(db.Directory.ToURI().ToString()).SetDirectory(new FilePath
				(db.WorkTree, path)).Call().GetRepository().Close();
			TreeWalk walk = new TreeWalk(db);
			DirCacheIterator indexIter = new DirCacheIterator(db.ReadDirCache());
			FileTreeIterator workTreeIter = new FileTreeIterator(db);
			walk.AddTree(indexIter);
			walk.AddTree(workTreeIter);
			walk.Filter = PathFilter.Create(path);
			NUnit.Framework.Assert.IsTrue(walk.Next());
			NUnit.Framework.Assert.IsTrue(indexIter.IdEqual(workTreeIter));
		}

		private sealed class _PathEdit_412 : DirCacheEditor.PathEdit
		{
			public _PathEdit_412(RevCommit id, string baseArg1) : base(baseArg1)
			{
				this.id = id;
			}

			public override void Apply(DirCacheEntry ent)
			{
				ent.FileMode = FileMode.GITLINK;
				ent.SetObjectId(id);
			}

			private readonly RevCommit id;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void IdOffset()
		{
			Git git = new Git(db);
			WriteTrashFile("fileAinfsonly", "A");
			FilePath fileBinindex = WriteTrashFile("fileBinindex", "B");
			FsTick(fileBinindex);
			git.Add().AddFilepattern("fileBinindex").Call();
			WriteTrashFile("fileCinfsonly", "C");
			TreeWalk tw = new TreeWalk(db);
			DirCacheIterator indexIter = new DirCacheIterator(db.ReadDirCache());
			FileTreeIterator workTreeIter = new FileTreeIterator(db);
			tw.AddTree(indexIter);
			tw.AddTree(workTreeIter);
			workTreeIter.SetDirCacheIterator(tw, 0);
			AssertEntry("d46c305e85b630558ee19cc47e73d2e5c8c64cdc", "a,", tw);
			AssertEntry("58ee403f98538ec02409538b3f80adf610accdec", "a,b", tw);
			AssertEntry("0000000000000000000000000000000000000000", "a", tw);
			AssertEntry("b8d30ff397626f0f1d3538d66067edf865e201d6", "a0b", tw);
			// The reason for adding this test. Check that the id is correct for
			// mixed
			AssertEntry("8c7e5a667f1b771847fe88c01c3de34413a1b220", "fileAinfsonly", tw);
			AssertEntry("7371f47a6f8bd23a8fa1a8b2a9479cdd76380e54", "fileBinindex", tw);
			AssertEntry("96d80cd6c4e7158dbebd0849f4fb7ce513e5828c", "fileCinfsonly", tw);
			NUnit.Framework.Assert.IsFalse(tw.Next());
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private void AssertEntry(string sha1string, string path, TreeWalk tw)
		{
			NUnit.Framework.Assert.IsTrue(tw.Next());
			NUnit.Framework.Assert.AreEqual(path, tw.PathString);
			NUnit.Framework.Assert.AreEqual(sha1string, tw.GetObjectId(1).GetName());
		}

		private static string NameOf(AbstractTreeIterator i)
		{
			return RawParseUtils.Decode(Constants.CHARSET, i.path, 0, i.pathLen);
		}
	}
}
