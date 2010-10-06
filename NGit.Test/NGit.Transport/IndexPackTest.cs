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

using ICSharpCode.SharpZipLib.Zip.Compression;
using NGit;
using NGit.Junit;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Transport;
using NGit.Util;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Test indexing of git packs.</summary>
	/// <remarks>
	/// Test indexing of git packs. A pack is read from a stream, copied
	/// to a new pack and an index is created. Then the packs are tested
	/// to make sure they contain the expected objects (well we don't test
	/// for all of them unless the packs are very small).
	/// </remarks>
	public class IndexPackTest : RepositoryTestCase
	{
		/// <summary>Test indexing one of the test packs in the egit repo.</summary>
		/// <remarks>Test indexing one of the test packs in the egit repo. It has deltas.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void Test1()
		{
			FilePath packFile = JGitTestUtil.GetTestResourceFile("pack-34be9032ac282b11fa9babdc2b2a93ca996c9c2f.pack"
				);
			InputStream @is = new FileInputStream(packFile);
			try
			{
				IndexPack pack = new IndexPack(db, @is, new FilePath(trash, "tmp_pack1"));
				pack.Index(new TextProgressMonitor());
				PackFile file = new PackFile(new FilePath(trash, "tmp_pack1.idx"), new FilePath(trash
					, "tmp_pack1.pack"));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("4b825dc642cb6eb9a060e54bf8d69288fbee4904"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("540a36d136cf413e4b064c2b0e0a4db60f77feab"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("5b6e7c66c276e7610d4a73c70ec1a1f7c1003259"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("6ff87c4664981e4397625791c8ea3bbb5f2279a3"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("82c6b885ff600be425b4ea96dee75dca255b69e7"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("902d5476fa249b7abc9d84c611577a81381f0327"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("aabf2ffaec9b497f0950352b3e582d73035c2035"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("c59759f143fb1fe21c197981df75a7ee00290799"
					)));
			}
			finally
			{
				@is.Close();
			}
		}

		/// <summary>This is just another pack.</summary>
		/// <remarks>
		/// This is just another pack. It so happens that we have two convenient pack to
		/// test with in the repository.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		[NUnit.Framework.Test]
		public virtual void Test2()
		{
			FilePath packFile = JGitTestUtil.GetTestResourceFile("pack-df2982f284bbabb6bdb59ee3fcc6eb0983e20371.pack"
				);
			InputStream @is = new FileInputStream(packFile);
			try
			{
				IndexPack pack = new IndexPack(db, @is, new FilePath(trash, "tmp_pack2"));
				pack.Index(new TextProgressMonitor());
				PackFile file = new PackFile(new FilePath(trash, "tmp_pack2.idx"), new FilePath(trash
					, "tmp_pack2.pack"));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("02ba32d3649e510002c21651936b7077aa75ffa9"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("0966a434eb1a025db6b71485ab63a3bfbea520b6"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("09efc7e59a839528ac7bda9fa020dc9101278680"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("0a3d7772488b6b106fb62813c4d6d627918d9181"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("1004d0d7ac26fbf63050a234c9b88a46075719d3"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("10da5895682013006950e7da534b705252b03be6"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("1203b03dc816ccbb67773f28b3c19318654b0bc8"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("15fae9e651043de0fd1deef588aa3fbf5a7a41c6"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("16f9ec009e5568c435f473ba3a1df732d49ce8c3"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("1fd7d579fb6ae3fe942dc09c2c783443d04cf21e"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("20a8ade77639491ea0bd667bf95de8abf3a434c8"
					)));
				NUnit.Framework.Assert.IsTrue(file.HasObject(ObjectId.FromString("2675188fd86978d5bc4d7211698b2118ae3bf658"
					)));
			}
			finally
			{
				// and lots more...
				@is.Close();
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestTinyThinPack()
		{
			TestRepository d = new TestRepository(db);
			RevBlob a = d.Blob("a");
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 1);
			pack.Write((Constants.OBJ_REF_DELTA) << 4 | 4);
			a.CopyRawTo(pack);
			Deflate(pack, new byte[] { unchecked((int)(0x1)), unchecked((int)(0x1)), unchecked(
				(int)(0x1)), (byte)('b') });
			Digest(pack);
			byte[] raw = pack.ToByteArray();
			IndexPack ip = IndexPack.Create(db, new ByteArrayInputStream(raw));
			ip.SetFixThin(true);
			ip.Index(NullProgressMonitor.INSTANCE);
			ip.RenameAndOpenPack();
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void PackHeader(TemporaryBuffer.Heap tinyPack, int cnt)
		{
			byte[] hdr = new byte[8];
			NB.EncodeInt32(hdr, 0, 2);
			NB.EncodeInt32(hdr, 4, cnt);
			tinyPack.Write(Constants.PACK_SIGNATURE);
			tinyPack.Write(hdr, 0, 8);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Deflate(TemporaryBuffer.Heap tinyPack, byte[] content)
		{
			Deflater deflater = new Deflater();
			byte[] buf = new byte[128];
			deflater.SetInput(content, 0, content.Length);
			deflater.Finish();
			do
			{
				int n = deflater.Deflate(buf, 0, buf.Length);
				if (n > 0)
				{
					tinyPack.Write(buf, 0, n);
				}
			}
			while (!deflater.IsFinished);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Digest(TemporaryBuffer.Heap buf)
		{
			MessageDigest md = Constants.NewMessageDigest();
			md.Update(buf.ToByteArray());
			buf.Write(md.Digest());
		}
	}
}
