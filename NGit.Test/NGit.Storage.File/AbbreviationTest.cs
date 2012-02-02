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
using NGit.Errors;
using NGit.Junit;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Transport;
using NGit.Util;
using NGit.Util.IO;
using Sharpen;

namespace NGit.Storage.File
{
	[NUnit.Framework.TestFixture]
	public class AbbreviationTest : LocalDiskRepositoryTestCase
	{
		private FileRepository db;

		private ObjectReader reader;

		private TestRepository<FileRepository> test;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			db = CreateBareRepository();
			reader = db.NewObjectReader();
			test = new TestRepository<FileRepository>(db);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.TearDown]
		public override void TearDown()
		{
			if (reader != null)
			{
				reader.Release();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAbbreviateOnEmptyRepository()
		{
			ObjectId id = Id("9d5b926ed164e8ee88d3b8b1e525d699adda01ba");
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(2), reader.Abbreviate(id, 2));
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(7), reader.Abbreviate(id, 7));
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(8), reader.Abbreviate(id, 8));
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(10), reader.Abbreviate(id, 10));
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(16), reader.Abbreviate(id, 16));
			NUnit.Framework.Assert.AreEqual(AbbreviatedObjectId.FromObjectId(id), reader.Abbreviate
				(id, Constants.OBJECT_ID_STRING_LENGTH));
			//
			ICollection<ObjectId> matches;
			matches = reader.Resolve(reader.Abbreviate(id, 8));
			NUnit.Framework.Assert.IsNotNull(matches);
			NUnit.Framework.Assert.AreEqual(0, matches.Count);
			matches = reader.Resolve(AbbreviatedObjectId.FromObjectId(id));
			NUnit.Framework.Assert.IsNotNull(matches);
			NUnit.Framework.Assert.AreEqual(1, matches.Count);
			NUnit.Framework.Assert.AreEqual(id, matches.Iterator().Next());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAbbreviateLooseBlob()
		{
			ObjectId id = test.Blob("test");
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(2), reader.Abbreviate(id, 2));
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(7), reader.Abbreviate(id, 7));
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(8), reader.Abbreviate(id, 8));
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(10), reader.Abbreviate(id, 10));
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(16), reader.Abbreviate(id, 16));
			ICollection<ObjectId> matches = reader.Resolve(reader.Abbreviate(id, 8));
			NUnit.Framework.Assert.IsNotNull(matches);
			NUnit.Framework.Assert.AreEqual(1, matches.Count);
			NUnit.Framework.Assert.AreEqual(id, matches.Iterator().Next());
			NUnit.Framework.Assert.AreEqual(id, db.Resolve(reader.Abbreviate(id, 8).Name));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAbbreviatePackedBlob()
		{
			RevBlob id = test.Blob("test");
			test.Branch("master").Commit().Add("test", id).Child();
			test.PackAndPrune();
			NUnit.Framework.Assert.IsTrue(reader.Has(id));
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(7), reader.Abbreviate(id, 7));
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(8), reader.Abbreviate(id, 8));
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(10), reader.Abbreviate(id, 10));
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(16), reader.Abbreviate(id, 16));
			ICollection<ObjectId> matches = reader.Resolve(reader.Abbreviate(id, 8));
			NUnit.Framework.Assert.IsNotNull(matches);
			NUnit.Framework.Assert.AreEqual(1, matches.Count);
			NUnit.Framework.Assert.AreEqual(id, matches.Iterator().Next());
			NUnit.Framework.Assert.AreEqual(id, db.Resolve(reader.Abbreviate(id, 8).Name));
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestAbbreviateIsActuallyUnique()
		{
			// This test is far more difficult. We have to manually craft
			// an input that contains collisions at a particular prefix,
			// but this is computationally difficult. Instead we force an
			// index file to have what we want.
			//
			ObjectId id = Id("9d5b926ed164e8ee88d3b8b1e525d699adda01ba");
			byte[] idBuf = ToByteArray(id);
			IList<PackedObjectInfo> objects = new AList<PackedObjectInfo>();
			for (int i = 0; i < 256; i++)
			{
				idBuf[9] = unchecked((byte)i);
				objects.AddItem(new PackedObjectInfo(ObjectId.FromRaw(idBuf)));
			}
			string packName = "pack-" + id.Name;
			FilePath packDir = new FilePath(((ObjectDirectory)db.ObjectDatabase).GetDirectory
				(), "pack");
			FilePath idxFile = new FilePath(packDir, packName + ".idx");
			FilePath packFile = new FilePath(packDir, packName + ".pack");
			FileUtils.Mkdir(packDir, true);
			OutputStream dst = new SafeBufferedOutputStream(new FileOutputStream(idxFile));
			try
			{
				PackIndexWriter writer = new PackIndexWriterV2(dst);
				writer.Write(objects, new byte[Constants.OBJECT_ID_LENGTH]);
			}
			finally
			{
				dst.Close();
			}
			new FileOutputStream(packFile).Close();
			NUnit.Framework.Assert.AreEqual(id.Abbreviate(20), reader.Abbreviate(id, 2));
			AbbreviatedObjectId abbrev8 = id.Abbreviate(8);
			ICollection<ObjectId> matches = reader.Resolve(abbrev8);
			NUnit.Framework.Assert.IsNotNull(matches);
			NUnit.Framework.Assert.AreEqual(objects.Count, matches.Count);
			foreach (PackedObjectInfo info in objects)
			{
				NUnit.Framework.Assert.IsTrue(matches.Contains(info), "contains " + info.Name);
			}
			try
			{
				db.Resolve(abbrev8.Name);
				NUnit.Framework.Assert.Fail("did not throw AmbiguousObjectException");
			}
			catch (AmbiguousObjectException err)
			{
				NUnit.Framework.Assert.AreEqual(abbrev8, err.GetAbbreviatedObjectId());
				matches = err.GetCandidates();
				NUnit.Framework.Assert.IsNotNull(matches);
				NUnit.Framework.Assert.AreEqual(objects.Count, matches.Count);
				foreach (PackedObjectInfo info_1 in objects)
				{
					NUnit.Framework.Assert.IsTrue(matches.Contains(info_1), "contains " + info_1.Name
						);
				}
			}
			NUnit.Framework.Assert.AreEqual(id, db.Resolve(id.Abbreviate(20).Name));
		}

		private static ObjectId Id(string name)
		{
			return ObjectId.FromString(name);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private static byte[] ToByteArray(ObjectId id)
		{
			ByteArrayOutputStream buf = new ByteArrayOutputStream(Constants.OBJECT_ID_LENGTH);
			id.CopyRawTo(buf);
			return buf.ToByteArray();
		}
	}
}
