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

using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;
using NGit;
using NGit.Errors;
using NGit.Internal;
using NGit.Junit;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Transport;
using NGit.Util;
using NGit.Util.IO;
using NUnit.Framework;
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
	[NUnit.Framework.TestFixture]
	public class PackParserTest : RepositoryTestCase
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
				ObjectDirectoryPackParser p = (ObjectDirectoryPackParser)Index(@is);
				p.Parse(NullProgressMonitor.INSTANCE);
				PackFile file = p.GetPackFile();
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
				ObjectDirectoryPackParser p = (ObjectDirectoryPackParser)Index(@is);
				p.Parse(NullProgressMonitor.INSTANCE);
				PackFile file = p.GetPackFile();
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
			TestRepository d = new TestRepository<FileRepository>(db);
			RevBlob a = d.Blob("a");
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 1);
			pack.Write((Constants.OBJ_REF_DELTA) << 4 | 4);
			a.CopyRawTo(pack);
			Deflate(pack, new byte[] { unchecked((int)(0x1)), unchecked((int)(0x1)), unchecked(
				(int)(0x1)), (byte)('b') });
			Digest(pack);
			PackParser p = Index(new ByteArrayInputStream(pack.ToByteArray()));
			p.SetAllowThin(true);
			p.Parse(NullProgressMonitor.INSTANCE);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPackWithDuplicateBlob()
		{
			byte[] data = Constants.Encode("0123456789abcdefg");
			TestRepository<Repository> d = new TestRepository<Repository>(db);
			NUnit.Framework.Assert.IsTrue(db.HasObject(d.Blob(data)));
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 1);
			pack.Write((Constants.OBJ_BLOB) << 4 | unchecked((int)(0x80)) | 1);
			pack.Write(1);
			Deflate(pack, data);
			Digest(pack);
			PackParser p = Index(new ByteArrayInputStream(pack.ToByteArray()));
			p.SetAllowThin(false);
			p.Parse(NullProgressMonitor.INSTANCE);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPackWithTrailingGarbage()
		{
			TestRepository d = new TestRepository<FileRepository>(db);
			RevBlob a = d.Blob("a");
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 1);
			pack.Write((Constants.OBJ_REF_DELTA) << 4 | 4);
			a.CopyRawTo(pack);
			Deflate(pack, new byte[] { unchecked((int)(0x1)), unchecked((int)(0x1)), unchecked(
				(int)(0x1)), (byte)('b') });
			Digest(pack);
			PackParser p = Index(new UnionInputStream(new ByteArrayInputStream(pack.ToByteArray
				()), new ByteArrayInputStream(new byte[] { unchecked((int)(0x7e)) })));
			p.SetAllowThin(true);
			p.SetCheckEofAfterPackFooter(true);
			try
			{
				p.Parse(NullProgressMonitor.INSTANCE);
				NUnit.Framework.Assert.Fail("Pack with trailing garbage was accepted");
			}
			catch (IOException err)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().expectedEOFReceived
					, "\\x7e"), err.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMaxObjectSizeFullBlob()
		{
			TestRepository d = new TestRepository<FileRepository>(db);
			byte[] data = Constants.Encode("0123456789");
			d.Blob(data);
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 1);
			pack.Write((Constants.OBJ_BLOB) << 4 | 10);
			Deflate(pack, data);
			Digest(pack);
			PackParser p = Index(new ByteArrayInputStream(pack.ToByteArray()));
			p.SetMaxObjectSizeLimit(11);
			p.Parse(NullProgressMonitor.INSTANCE);
			p = Index(new ByteArrayInputStream(pack.ToByteArray()));
			p.SetMaxObjectSizeLimit(10);
			p.Parse(NullProgressMonitor.INSTANCE);
			p = Index(new ByteArrayInputStream(pack.ToByteArray()));
			p.SetMaxObjectSizeLimit(9);
			try
			{
				p.Parse(NullProgressMonitor.INSTANCE);
				NUnit.Framework.Assert.Fail("PackParser should have failed");
			}
			catch (TooLargeObjectInPackException e)
			{
				NUnit.Framework.Assert.IsTrue(e.Message.Contains("10"));
				// obj size
				NUnit.Framework.Assert.IsTrue(e.Message.Contains("9"));
			}
		}

		// max obj size
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMaxObjectSizeDeltaBlock()
		{
			TestRepository d = new TestRepository<FileRepository>(db);
			RevBlob a = d.Blob("a");
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 1);
			pack.Write((Constants.OBJ_REF_DELTA) << 4 | 14);
			a.CopyRawTo(pack);
			Deflate(pack, new byte[] { 1, 11, 11, (byte)('a'), (byte)('0'), (byte)('1'), (byte
				)('2'), (byte)('3'), (byte)('4'), (byte)('5'), (byte)('6'), (byte)('7'), (byte)(
				'8'), (byte)('9') });
			Digest(pack);
			PackParser p = Index(new ByteArrayInputStream(pack.ToByteArray()));
			p.SetAllowThin(true);
			p.SetMaxObjectSizeLimit(14);
			p.Parse(NullProgressMonitor.INSTANCE);
			p = Index(new ByteArrayInputStream(pack.ToByteArray()));
			p.SetAllowThin(true);
			p.SetMaxObjectSizeLimit(13);
			try
			{
				p.Parse(NullProgressMonitor.INSTANCE);
				NUnit.Framework.Assert.Fail("PackParser should have failed");
			}
			catch (TooLargeObjectInPackException e)
			{
				NUnit.Framework.Assert.IsTrue(e.Message.Contains("13"));
				// max obj size
				NUnit.Framework.Assert.IsFalse(e.Message.Contains("14"));
			}
		}

		// no delta size
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestMaxObjectSizeDeltaResultSize()
		{
			TestRepository d = new TestRepository<FileRepository>(db);
			RevBlob a = d.Blob("0123456789");
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 1);
			pack.Write((Constants.OBJ_REF_DELTA) << 4 | 4);
			a.CopyRawTo(pack);
			Deflate(pack, new byte[] { 10, 11, 1, (byte)('a') });
			Digest(pack);
			PackParser p = Index(new ByteArrayInputStream(pack.ToByteArray()));
			p.SetAllowThin(true);
			p.SetMaxObjectSizeLimit(11);
			p.Parse(NullProgressMonitor.INSTANCE);
			p = Index(new ByteArrayInputStream(pack.ToByteArray()));
			p.SetAllowThin(true);
			p.SetMaxObjectSizeLimit(10);
			try
			{
				p.Parse(NullProgressMonitor.INSTANCE);
				NUnit.Framework.Assert.Fail("PackParser should have failed");
			}
			catch (TooLargeObjectInPackException e)
			{
				NUnit.Framework.Assert.IsTrue(e.Message.Contains("11"));
				// result obj size
				NUnit.Framework.Assert.IsTrue(e.Message.Contains("10"));
			}
		}

		// max obj size
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestNonMarkingInputStream()
		{
			TestRepository d = new TestRepository<FileRepository>(db);
			RevBlob a = d.Blob("a");
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 1);
			pack.Write((Constants.OBJ_REF_DELTA) << 4 | 4);
			a.CopyRawTo(pack);
			Deflate(pack, new byte[] { unchecked((int)(0x1)), unchecked((int)(0x1)), unchecked(
				(int)(0x1)), (byte)('b') });
			Digest(pack);
			InputStream @in = new _ByteArrayInputStream_319(pack.ToByteArray());
			PackParser p = Index(@in);
			p.SetAllowThin(true);
			p.SetCheckEofAfterPackFooter(false);
			p.SetExpectDataAfterPackFooter(true);
			try
			{
				p.Parse(NullProgressMonitor.INSTANCE);
				NUnit.Framework.Assert.Fail("PackParser should have failed");
			}
			catch (IOException e)
			{
				NUnit.Framework.Assert.AreEqual(e.Message, JGitText.Get().inputStreamMustSupportMark
					);
			}
		}

		private sealed class _ByteArrayInputStream_319 : ByteArrayInputStream
		{
			public _ByteArrayInputStream_319(byte[] baseArg1) : base(baseArg1)
			{
			}

			public override bool MarkSupported()
			{
				return false;
			}

			public override void Mark(int maxlength)
			{
				NUnit.Framework.Assert.Fail("Mark should not be called");
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDataAfterPackFooterSingleRead()
		{
			TestRepository d = new TestRepository<FileRepository>(db);
			RevBlob a = d.Blob("a");
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(32 * 1024);
			PackHeader(pack, 1);
			pack.Write((Constants.OBJ_REF_DELTA) << 4 | 4);
			a.CopyRawTo(pack);
			Deflate(pack, new byte[] { unchecked((int)(0x1)), unchecked((int)(0x1)), unchecked(
				(int)(0x1)), (byte)('b') });
			Digest(pack);
			byte[] packData = pack.ToByteArray();
			byte[] streamData = new byte[packData.Length + 1];
			System.Array.Copy(packData, 0, streamData, 0, packData.Length);
			streamData[packData.Length] = unchecked((int)(0x7e));
			InputStream @in = new ByteArrayInputStream(streamData);
			PackParser p = Index(@in);
			p.SetAllowThin(true);
			p.SetCheckEofAfterPackFooter(false);
			p.SetExpectDataAfterPackFooter(true);
			p.Parse(NullProgressMonitor.INSTANCE);
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x7e)), @in.Read());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDataAfterPackFooterSplitObjectRead()
		{
			byte[] data = Constants.Encode("0123456789");
			// Build a pack ~17k
			int objects = 900;
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(32 * 1024);
			PackHeader(pack, objects);
			for (int i = 0; i < objects; i++)
			{
				pack.Write((Constants.OBJ_BLOB) << 4 | 10);
				Deflate(pack, data);
			}
			Digest(pack);
			byte[] packData = pack.ToByteArray();
			byte[] streamData = new byte[packData.Length + 1];
			System.Array.Copy(packData, 0, streamData, 0, packData.Length);
			streamData[packData.Length] = unchecked((int)(0x7e));
			InputStream @in = new ByteArrayInputStream(streamData);
			PackParser p = Index(@in);
			p.SetAllowThin(true);
			p.SetCheckEofAfterPackFooter(false);
			p.SetExpectDataAfterPackFooter(true);
			p.Parse(NullProgressMonitor.INSTANCE);
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x7e)), @in.Read());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestDataAfterPackFooterSplitHeaderRead()
		{
			TestRepository d = new TestRepository<FileRepository>(db);
			byte[] data = Constants.Encode("a");
			RevBlob b = d.Blob(data);
			int objects = 248;
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(32 * 1024);
			PackHeader(pack, objects + 1);
			int offset = 13;
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < offset; i++)
			{
				sb.Append(i);
			}
			offset = sb.ToString().Length;
			int lenByte = (Constants.OBJ_BLOB) << 4 | (offset & unchecked((int)(0x0F)));
			offset >>= 4;
			if (offset > 0)
			{
				lenByte |= 1 << 7;
			}
			pack.Write(lenByte);
			while (offset > 0)
			{
				lenByte = offset & unchecked((int)(0x7F));
				offset >>= 6;
				if (offset > 0)
				{
					lenByte |= 1 << 7;
				}
				pack.Write(lenByte);
			}
			Deflate(pack, Constants.Encode(sb.ToString()));
			for (int i_1 = 0; i_1 < objects; i_1++)
			{
				// The last pack header written falls across the 8192 byte boundary
				// between [8189:8210]
				pack.Write((Constants.OBJ_REF_DELTA) << 4 | 4);
				b.CopyRawTo(pack);
				Deflate(pack, new byte[] { unchecked((int)(0x1)), unchecked((int)(0x1)), unchecked(
					(int)(0x1)), (byte)('b') });
			}
			Digest(pack);
			byte[] packData = pack.ToByteArray();
			byte[] streamData = new byte[packData.Length + 1];
			System.Array.Copy(packData, 0, streamData, 0, packData.Length);
			streamData[packData.Length] = unchecked((int)(0x7e));
			InputStream @in = new ByteArrayInputStream(streamData);
			PackParser p = Index(@in);
			p.SetAllowThin(true);
			p.SetCheckEofAfterPackFooter(false);
			p.SetExpectDataAfterPackFooter(true);
			p.Parse(NullProgressMonitor.INSTANCE);
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x7e)), @in.Read());
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

		private ObjectInserter inserter;

		[TearDown]
		public virtual void Release()
		{
			if (inserter != null)
			{
				inserter.Release();
				inserter = null;
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private PackParser Index(InputStream @in)
		{
			if (inserter == null)
			{
				inserter = db.NewObjectInserter();
			}
			return inserter.NewPackParser(@in);
		}
	}
}
