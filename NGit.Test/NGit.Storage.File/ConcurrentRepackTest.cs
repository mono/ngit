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
using NGit.Errors;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Storage.Pack;
using NGit.Util;
using NGit.Util.IO;
using NUnit.Framework;
using Sharpen;

namespace NGit.Storage.File
{
	[NUnit.Framework.TestFixture]
	public class ConcurrentRepackTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			WindowCacheConfig windowCacheConfig = new WindowCacheConfig();
			windowCacheConfig.SetPackedGitOpenFiles(1);
			WindowCache.Reconfigure(windowCacheConfig);
			base.SetUp();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.TearDown]
		public override void TearDown()
		{
			base.TearDown();
			WindowCacheConfig windowCacheConfig = new WindowCacheConfig();
			WindowCache.Reconfigure(windowCacheConfig);
		}

		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestObjectInNewPack()
		{
			// Create a new object in a new pack, and test that it is present.
			//
			Repository eden = CreateBareRepository();
			RevObject o1 = WriteBlob(eden, "o1");
			Pack(eden, o1);
			NUnit.Framework.Assert.AreEqual(o1.Name, Parse(o1).Name);
		}

		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestObjectMovedToNewPack1()
		{
			// Create an object and pack it. Then remove that pack and put the
			// object into a different pack file, with some other object. We
			// still should be able to access the objects.
			//
			Repository eden = CreateBareRepository();
			RevObject o1 = WriteBlob(eden, "o1");
			FilePath[] out1 = Pack(eden, o1);
			NUnit.Framework.Assert.AreEqual(o1.Name, Parse(o1).Name);
			RevObject o2 = WriteBlob(eden, "o2");
			Pack(eden, o2, o1);
			// Force close, and then delete, the old pack.
			//
			WhackCache();
			Delete(out1);
			// Now here is the interesting thing. Will git figure the new
			// object exists in the new pack, and not the old one.
			//
			NUnit.Framework.Assert.AreEqual(o2.Name, Parse(o2).Name);
			NUnit.Framework.Assert.AreEqual(o1.Name, Parse(o1).Name);
		}

		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestObjectMovedWithinPack()
		{
			// Create an object and pack it.
			//
			Repository eden = CreateBareRepository();
			RevObject o1 = WriteBlob(eden, "o1");
			FilePath[] out1 = Pack(eden, o1);
			NUnit.Framework.Assert.AreEqual(o1.Name, Parse(o1).Name);
			// Force close the old pack.
			//
			WhackCache();
			// Now overwrite the old pack in place. This method of creating a
			// different pack under the same file name is partially broken. We
			// should also have a different file name because the list of objects
			// within the pack has been modified.
			//
			RevObject o2 = WriteBlob(eden, "o2");
			PackWriter pw = new PackWriter(eden);
			pw.AddObject(o2);
			pw.AddObject(o1);
			Write(out1, pw);
			pw.Release();
			// Try the old name, then the new name. The old name should cause the
			// pack to reload when it opens and the index and pack mismatch.
			//
			NUnit.Framework.Assert.AreEqual(o1.Name, Parse(o1).Name);
			NUnit.Framework.Assert.AreEqual(o2.Name, Parse(o2).Name);
		}

		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestObjectMovedToNewPack2()
		{
			// Create an object and pack it. Then remove that pack and put the
			// object into a different pack file, with some other object. We
			// still should be able to access the objects.
			//
			Repository eden = CreateBareRepository();
			RevObject o1 = WriteBlob(eden, "o1");
			FilePath[] out1 = Pack(eden, o1);
			NUnit.Framework.Assert.AreEqual(o1.Name, Parse(o1).Name);
			ObjectLoader load1 = db.Open(o1, Constants.OBJ_BLOB);
			NUnit.Framework.Assert.IsNotNull(load1);
			RevObject o2 = WriteBlob(eden, "o2");
			Pack(eden, o2, o1);
			// Force close, and then delete, the old pack.
			//
			WhackCache();
			Delete(out1);
			// Now here is the interesting thing... can the loader we made
			// earlier still resolve the object, even though its underlying
			// pack is gone, but the object still exists.
			//
			ObjectLoader load2 = db.Open(o1, Constants.OBJ_BLOB);
			NUnit.Framework.Assert.IsNotNull(load2);
			NUnit.Framework.Assert.AreNotSame(load1, load2);
			byte[] data2 = load2.GetCachedBytes();
			byte[] data1 = load1.GetCachedBytes();
			NUnit.Framework.Assert.IsNotNull(data2);
			NUnit.Framework.Assert.IsNotNull(data1);
			NUnit.Framework.Assert.AreNotSame(data1, data2);
			// cache should be per-pack, not per object
			CollectionAssert.AreEquivalent(data1, data2);
			NUnit.Framework.Assert.AreEqual(load2.GetType(), load1.GetType());
		}

		private static void WhackCache()
		{
			WindowCacheConfig config = new WindowCacheConfig();
			config.SetPackedGitOpenFiles(1);
			WindowCache.Reconfigure(config);
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private RevObject Parse(AnyObjectId id)
		{
			return new RevWalk(db).ParseAny(id);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private FilePath[] Pack(Repository src, params RevObject[] list)
		{
			PackWriter pw = new PackWriter(src);
			foreach (RevObject o in list)
			{
				pw.AddObject(o);
			}
			ObjectId name = pw.ComputeName();
			FilePath packFile = FullPackFileName(name, ".pack");
			FilePath idxFile = FullPackFileName(name, ".idx");
			FilePath[] files = new FilePath[] { packFile, idxFile };
			Write(files, pw);
			pw.Release();
			return files;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private static void Write(FilePath[] files, PackWriter pw)
		{
			long begin = files[0].GetParentFile().LastModified();
			NullProgressMonitor m = NullProgressMonitor.INSTANCE;
			OutputStream @out;
			@out = new SafeBufferedOutputStream(new FileOutputStream(files[0]));
			try
			{
				pw.WritePack(m, m, @out);
			}
			finally
			{
				@out.Close();
			}
			@out = new SafeBufferedOutputStream(new FileOutputStream(files[1]));
			try
			{
				pw.WriteIndex(@out);
			}
			finally
			{
				@out.Close();
			}
			Touch(begin, files[0].GetParentFile());
		}

		/// <exception cref="System.IO.IOException"></exception>
		private static void Delete(FilePath[] list)
		{
			long begin = list[0].GetParentFile().LastModified();
			foreach (FilePath f in list)
			{
				FileUtils.Delete(f);
				NUnit.Framework.Assert.IsFalse(f.Exists(), f + " was removed");
			}
			Touch(begin, list[0].GetParentFile());
		}

		private static void Touch(long begin, FilePath dir)
		{
			while (begin >= dir.LastModified())
			{
				try
				{
					Sharpen.Thread.Sleep(25);
				}
				catch (Exception)
				{
				}
				//
				dir.SetLastModified(Runtime.CurrentTimeMillis());
			}
		}

		private FilePath FullPackFileName(ObjectId name, string suffix)
		{
			FilePath packdir = new FilePath(((ObjectDirectory)db.ObjectDatabase).GetDirectory
				(), "pack");
			return new FilePath(packdir, "pack-" + name.Name + suffix);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private RevObject WriteBlob(Repository repo, string data)
		{
			RevWalk revWalk = new RevWalk(repo);
			byte[] bytes = Constants.Encode(data);
			ObjectInserter inserter = repo.NewObjectInserter();
			ObjectId id;
			try
			{
				id = inserter.Insert(Constants.OBJ_BLOB, bytes);
				inserter.Flush();
			}
			finally
			{
				inserter.Release();
			}
			try
			{
				Parse(id);
				NUnit.Framework.Assert.Fail("Object " + id.Name + " should not exist in test repository"
					);
			}
			catch (MissingObjectException)
			{
			}
			// Ok
			return revWalk.LookupBlob(id);
		}
	}
}
