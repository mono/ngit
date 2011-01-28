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
using NGit.Storage.File;
using NGit.Treewalk;
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

		private static string NameOf(AbstractTreeIterator i)
		{
			return RawParseUtils.Decode(Constants.CHARSET, i.path, 0, i.pathLen);
		}
	}
}
