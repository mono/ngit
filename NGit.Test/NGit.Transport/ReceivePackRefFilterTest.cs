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
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip.Compression;
using NGit;
using NGit.Errors;
using NGit.Junit;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Storage.Pack;
using NGit.Transport;
using NGit.Util;
using Sharpen;

namespace NGit.Transport
{
	[NUnit.Framework.TestFixture]
	public class ReceivePackRefFilterTest : LocalDiskRepositoryTestCase
	{
		private static readonly NullProgressMonitor PM = NullProgressMonitor.INSTANCE;

		private static readonly string R_MASTER = Constants.R_HEADS + Constants.MASTER;

		private static readonly string R_PRIVATE = Constants.R_HEADS + "private";

		private Repository src;

		private Repository dst;

		private RevCommit A;

		private RevCommit B;

		private RevCommit P;

		private RevBlob a;

		private RevBlob b;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public override void SetUp()
		{
			base.SetUp();
			src = CreateBareRepository();
			dst = CreateBareRepository();
			// Fill dst with a some common history.
			//
			TestRepository d = new TestRepository(dst);
			a = d.Blob("a");
			A = d.Commit(d.Tree(d.File("a", a)));
			B = d.Commit().Parent(A).Create();
			d.Update(R_MASTER, B);
			// Clone from dst into src
			//
			NGit.Transport.Transport t = NGit.Transport.Transport.Open(src, UriOf(dst));
			try
			{
				t.Fetch(PM, Collections.Singleton(new RefSpec("+refs/*:refs/*")));
				NUnit.Framework.Assert.AreEqual(B, src.Resolve(R_MASTER));
			}
			finally
			{
				t.Close();
			}
			// Now put private stuff into dst.
			//
			b = d.Blob("b");
			P = d.Commit(d.Tree(d.File("b", b)), A);
			d.Update(R_PRIVATE, P);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.TearDown]
		public override void TearDown()
		{
			if (src != null)
			{
				src.Close();
			}
			if (dst != null)
			{
				dst.Close();
			}
			base.TearDown();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFilterHidesPrivate()
		{
			IDictionary<string, Ref> refs;
			TransportLocal t = new _TransportLocal_146(this, src, UriOf(dst));
			try
			{
				PushConnection c = t.OpenPush();
				try
				{
					refs = c.GetRefsMap();
				}
				finally
				{
					c.Close();
				}
			}
			finally
			{
				t.Close();
			}
			NUnit.Framework.Assert.IsNotNull(refs);
			NUnit.Framework.Assert.IsNull(refs.Get(R_PRIVATE), "no private");
			NUnit.Framework.Assert.IsNull(refs.Get(Constants.HEAD), "no HEAD");
			NUnit.Framework.Assert.AreEqual(1, refs.Count);
			Ref master = refs.Get(R_MASTER);
			NUnit.Framework.Assert.IsNotNull(master, "has master");
			NUnit.Framework.Assert.AreEqual(B, master.GetObjectId());
		}

		private sealed class _TransportLocal_146 : TransportLocal
		{
			public _TransportLocal_146(ReceivePackRefFilterTest _enclosing, Repository baseArg1
				, URIish baseArg2) : base(baseArg1, baseArg2)
			{
				this._enclosing = _enclosing;
			}

			internal override ReceivePack CreateReceivePack(Repository db)
			{
				db.Close();
				this._enclosing.dst.IncrementOpen();
				ReceivePack rp = base.CreateReceivePack(this._enclosing.dst);
				rp.SetRefFilter(new ReceivePackRefFilterTest.HidePrivateFilter());
				return rp;
			}

			private readonly ReceivePackRefFilterTest _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSuccess()
		{
			// Manually force a delta of an object so we reuse it later.
			//
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 2);
			pack.Write((Constants.OBJ_BLOB) << 4 | 1);
			Deflate(pack, new byte[] { (byte)('a') });
			pack.Write((Constants.OBJ_REF_DELTA) << 4 | 4);
			a.CopyRawTo(pack);
			Deflate(pack, new byte[] { unchecked((int)(0x1)), unchecked((int)(0x1)), unchecked(
				(int)(0x1)), (byte)('b') });
			Digest(pack);
			OpenPack(pack);
			// Verify the only storage of b is our packed delta above.
			//
			ObjectDirectory od = (ObjectDirectory)src.ObjectDatabase;
			NUnit.Framework.Assert.IsTrue(src.HasObject(b), "has b");
			NUnit.Framework.Assert.IsFalse(od.FileFor(b).Exists(), "b not loose");
			// Now use b but in a different commit than what is hidden.
			//
			TestRepository s = new TestRepository(src);
			RevCommit N = s.Commit().Parent(B).Add("q", b).Create();
			s.Update(R_MASTER, N);
			// Push this new content to the remote, doing strict validation.
			//
			TransportLocal t = new _TransportLocal_209(this, src, UriOf(dst));
			RemoteRefUpdate u = new RemoteRefUpdate(src, R_MASTER, R_MASTER, false, null, null
				);
			//
			//
			// src name
			// dst name
			// do not force update
			// local tracking branch
			// expected id
			PushResult r;
			try
			{
				t.SetPushThin(true);
				r = t.Push(PM, Sharpen.Collections.Singleton(u));
			}
			finally
			{
				t.Close();
			}
			NUnit.Framework.Assert.IsNotNull(r, "have result");
			NUnit.Framework.Assert.IsNull(r.GetAdvertisedRef(R_PRIVATE), "private not advertised"
				);
			NUnit.Framework.Assert.AreEqual(RemoteRefUpdate.Status.OK, u.GetStatus(), "master updated"
				);
			NUnit.Framework.Assert.AreEqual(N, dst.Resolve(R_MASTER));
		}

		private sealed class _TransportLocal_209 : TransportLocal
		{
			public _TransportLocal_209(ReceivePackRefFilterTest _enclosing, Repository baseArg1
				, URIish baseArg2) : base(baseArg1, baseArg2)
			{
				this._enclosing = _enclosing;
			}

			internal override ReceivePack CreateReceivePack(Repository db)
			{
				db.Close();
				this._enclosing.dst.IncrementOpen();
				ReceivePack rp = base.CreateReceivePack(this._enclosing.dst);
				rp.SetCheckReceivedObjects(true);
				rp.SetCheckReferencedObjectsAreReachable(true);
				rp.SetRefFilter(new ReceivePackRefFilterTest.HidePrivateFilter());
				return rp;
			}

			private readonly ReceivePackRefFilterTest _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCreateBranchAtHiddenCommitFails()
		{
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(64);
			PackHeader(pack, 0);
			Digest(pack);
			TemporaryBuffer.Heap inBuf = new TemporaryBuffer.Heap(256);
			PacketLineOut inPckLine = new PacketLineOut(inBuf);
			inPckLine.WriteString(ObjectId.ZeroId.Name + ' ' + P.Name + ' ' + "refs/heads/s" 
				+ '\0' + BasePackPushConnection.CAPABILITY_REPORT_STATUS);
			inPckLine.End();
			pack.WriteTo(inBuf, PM);
			TemporaryBuffer.Heap outBuf = new TemporaryBuffer.Heap(1024);
			ReceivePack rp = new ReceivePack(dst);
			rp.SetCheckReceivedObjects(true);
			rp.SetCheckReferencedObjectsAreReachable(true);
			rp.SetRefFilter(new ReceivePackRefFilterTest.HidePrivateFilter());
			try
			{
				Receive(rp, inBuf, outBuf);
				NUnit.Framework.Assert.Fail("Expected UnpackException");
			}
			catch (UnpackException failed)
			{
				Exception err = failed.InnerException;
				NUnit.Framework.Assert.IsTrue(err is MissingObjectException);
				MissingObjectException moe = (MissingObjectException)err;
				NUnit.Framework.Assert.AreEqual(P, moe.GetObjectId());
			}
			PacketLineIn r = AsPacketLineIn(outBuf);
			string master = r.ReadString();
			int nul = master.IndexOf('\0');
			NUnit.Framework.Assert.IsTrue(nul > 0, "has capability list");
			NUnit.Framework.Assert.AreEqual(B.Name + ' ' + R_MASTER, Sharpen.Runtime.Substring
				(master, 0, nul));
			NUnit.Framework.Assert.AreSame(PacketLineIn.END, r.ReadString());
			NUnit.Framework.Assert.AreEqual("unpack error Missing commit " + P.Name, r.ReadString
				());
			NUnit.Framework.Assert.AreEqual("ng refs/heads/s n/a (unpacker error)", r.ReadString
				());
			NUnit.Framework.Assert.AreSame(PacketLineIn.END, r.ReadString());
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Receive(ReceivePack rp, TemporaryBuffer.Heap inBuf, TemporaryBuffer.Heap
			 outBuf)
		{
			rp.Receive(new ByteArrayInputStream(inBuf.ToByteArray()), outBuf, null);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUsingHiddenDeltaBaseFails()
		{
			byte[] delta = new byte[] { unchecked((int)(0x1)), unchecked((int)(0x1)), unchecked(
				(int)(0x1)), (byte)('c') };
			TestRepository<Repository> s = new TestRepository<Repository>(src);
			RevCommit N = s.Commit().Parent(B).Add("q", s.Blob(BinaryDelta.Apply(dst.Open(b).
				GetCachedBytes(), delta))).Create();
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 3);
			Copy(pack, src.Open(N));
			Copy(pack, src.Open(s.ParseBody(N).Tree));
			pack.Write((Constants.OBJ_REF_DELTA) << 4 | 4);
			b.CopyRawTo(pack);
			Deflate(pack, delta);
			Digest(pack);
			TemporaryBuffer.Heap inBuf = new TemporaryBuffer.Heap(1024);
			PacketLineOut inPckLine = new PacketLineOut(inBuf);
			inPckLine.WriteString(ObjectId.ZeroId.Name + ' ' + N.Name + ' ' + "refs/heads/s" 
				+ '\0' + BasePackPushConnection.CAPABILITY_REPORT_STATUS);
			inPckLine.End();
			pack.WriteTo(inBuf, PM);
			TemporaryBuffer.Heap outBuf = new TemporaryBuffer.Heap(1024);
			ReceivePack rp = new ReceivePack(dst);
			rp.SetCheckReceivedObjects(true);
			rp.SetCheckReferencedObjectsAreReachable(true);
			rp.SetRefFilter(new ReceivePackRefFilterTest.HidePrivateFilter());
			try
			{
				Receive(rp, inBuf, outBuf);
				NUnit.Framework.Assert.Fail("Expected UnpackException");
			}
			catch (UnpackException failed)
			{
				Exception err = failed.InnerException;
				NUnit.Framework.Assert.IsTrue(err is MissingObjectException);
				MissingObjectException moe = (MissingObjectException)err;
				NUnit.Framework.Assert.AreEqual(b, moe.GetObjectId());
			}
			PacketLineIn r = AsPacketLineIn(outBuf);
			string master = r.ReadString();
			int nul = master.IndexOf('\0');
			NUnit.Framework.Assert.IsTrue(nul > 0, "has capability list");
			NUnit.Framework.Assert.AreEqual(B.Name + ' ' + R_MASTER, Sharpen.Runtime.Substring
				(master, 0, nul));
			NUnit.Framework.Assert.AreSame(PacketLineIn.END, r.ReadString());
			NUnit.Framework.Assert.AreEqual("unpack error Missing blob " + b.Name, r.ReadString
				());
			NUnit.Framework.Assert.AreEqual("ng refs/heads/s n/a (unpacker error)", r.ReadString
				());
			NUnit.Framework.Assert.AreSame(PacketLineIn.END, r.ReadString());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUsingHiddenCommonBlobFails()
		{
			// Try to use the 'b' blob that is hidden.
			//
			TestRepository<Repository> s = new TestRepository<Repository>(src);
			RevCommit N = s.Commit().Parent(B).Add("q", s.Blob("b")).Create();
			// But don't include it in the pack.
			//
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 2);
			Copy(pack, src.Open(N));
			Copy(pack, src.Open(s.ParseBody(N).Tree));
			Digest(pack);
			TemporaryBuffer.Heap inBuf = new TemporaryBuffer.Heap(1024);
			PacketLineOut inPckLine = new PacketLineOut(inBuf);
			inPckLine.WriteString(ObjectId.ZeroId.Name + ' ' + N.Name + ' ' + "refs/heads/s" 
				+ '\0' + BasePackPushConnection.CAPABILITY_REPORT_STATUS);
			inPckLine.End();
			pack.WriteTo(inBuf, PM);
			TemporaryBuffer.Heap outBuf = new TemporaryBuffer.Heap(1024);
			ReceivePack rp = new ReceivePack(dst);
			rp.SetCheckReceivedObjects(true);
			rp.SetCheckReferencedObjectsAreReachable(true);
			rp.SetRefFilter(new ReceivePackRefFilterTest.HidePrivateFilter());
			try
			{
				Receive(rp, inBuf, outBuf);
				NUnit.Framework.Assert.Fail("Expected UnpackException");
			}
			catch (UnpackException failed)
			{
				Exception err = failed.InnerException;
				NUnit.Framework.Assert.IsTrue(err is MissingObjectException);
				MissingObjectException moe = (MissingObjectException)err;
				NUnit.Framework.Assert.AreEqual(b, moe.GetObjectId());
			}
			PacketLineIn r = AsPacketLineIn(outBuf);
			string master = r.ReadString();
			int nul = master.IndexOf('\0');
			NUnit.Framework.Assert.IsTrue(nul > 0, "has capability list");
			NUnit.Framework.Assert.AreEqual(B.Name + ' ' + R_MASTER, Sharpen.Runtime.Substring
				(master, 0, nul));
			NUnit.Framework.Assert.AreSame(PacketLineIn.END, r.ReadString());
			NUnit.Framework.Assert.AreEqual("unpack error Missing blob " + b.Name, r.ReadString
				());
			NUnit.Framework.Assert.AreEqual("ng refs/heads/s n/a (unpacker error)", r.ReadString
				());
			NUnit.Framework.Assert.AreSame(PacketLineIn.END, r.ReadString());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUsingUnknownBlobFails()
		{
			// Try to use the 'n' blob that is not on the server.
			//
			TestRepository<Repository> s = new TestRepository<Repository>(src);
			RevBlob n = s.Blob("n");
			RevCommit N = s.Commit().Parent(B).Add("q", n).Create();
			// But don't include it in the pack.
			//
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 2);
			Copy(pack, src.Open(N));
			Copy(pack, src.Open(s.ParseBody(N).Tree));
			Digest(pack);
			TemporaryBuffer.Heap inBuf = new TemporaryBuffer.Heap(1024);
			PacketLineOut inPckLine = new PacketLineOut(inBuf);
			inPckLine.WriteString(ObjectId.ZeroId.Name + ' ' + N.Name + ' ' + "refs/heads/s" 
				+ '\0' + BasePackPushConnection.CAPABILITY_REPORT_STATUS);
			inPckLine.End();
			pack.WriteTo(inBuf, PM);
			TemporaryBuffer.Heap outBuf = new TemporaryBuffer.Heap(1024);
			ReceivePack rp = new ReceivePack(dst);
			rp.SetCheckReceivedObjects(true);
			rp.SetCheckReferencedObjectsAreReachable(true);
			rp.SetRefFilter(new ReceivePackRefFilterTest.HidePrivateFilter());
			try
			{
				Receive(rp, inBuf, outBuf);
				NUnit.Framework.Assert.Fail("Expected UnpackException");
			}
			catch (UnpackException failed)
			{
				Exception err = failed.InnerException;
				NUnit.Framework.Assert.IsTrue(err is MissingObjectException);
				MissingObjectException moe = (MissingObjectException)err;
				NUnit.Framework.Assert.AreEqual(n, moe.GetObjectId());
			}
			PacketLineIn r = AsPacketLineIn(outBuf);
			string master = r.ReadString();
			int nul = master.IndexOf('\0');
			NUnit.Framework.Assert.IsTrue(nul > 0, "has capability list");
			NUnit.Framework.Assert.AreEqual(B.Name + ' ' + R_MASTER, Sharpen.Runtime.Substring
				(master, 0, nul));
			NUnit.Framework.Assert.AreSame(PacketLineIn.END, r.ReadString());
			NUnit.Framework.Assert.AreEqual("unpack error Missing blob " + n.Name, r.ReadString
				());
			NUnit.Framework.Assert.AreEqual("ng refs/heads/s n/a (unpacker error)", r.ReadString
				());
			NUnit.Framework.Assert.AreSame(PacketLineIn.END, r.ReadString());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestUsingUnknownTreeFails()
		{
			TestRepository<Repository> s = new TestRepository<Repository>(src);
			RevCommit N = s.Commit().Parent(B).Add("q", s.Blob("a")).Create();
			RevTree t = s.ParseBody(N).Tree;
			// Don't include the tree in the pack.
			//
			TemporaryBuffer.Heap pack = new TemporaryBuffer.Heap(1024);
			PackHeader(pack, 1);
			Copy(pack, src.Open(N));
			Digest(pack);
			TemporaryBuffer.Heap inBuf = new TemporaryBuffer.Heap(1024);
			PacketLineOut inPckLine = new PacketLineOut(inBuf);
			inPckLine.WriteString(ObjectId.ZeroId.Name + ' ' + N.Name + ' ' + "refs/heads/s" 
				+ '\0' + BasePackPushConnection.CAPABILITY_REPORT_STATUS);
			inPckLine.End();
			pack.WriteTo(inBuf, PM);
			TemporaryBuffer.Heap outBuf = new TemporaryBuffer.Heap(1024);
			ReceivePack rp = new ReceivePack(dst);
			rp.SetCheckReceivedObjects(true);
			rp.SetCheckReferencedObjectsAreReachable(true);
			rp.SetRefFilter(new ReceivePackRefFilterTest.HidePrivateFilter());
			try
			{
				Receive(rp, inBuf, outBuf);
				NUnit.Framework.Assert.Fail("Expected UnpackException");
			}
			catch (UnpackException failed)
			{
				Exception err = failed.InnerException;
				NUnit.Framework.Assert.IsTrue(err is MissingObjectException);
				MissingObjectException moe = (MissingObjectException)err;
				NUnit.Framework.Assert.AreEqual(t, moe.GetObjectId());
			}
			PacketLineIn r = AsPacketLineIn(outBuf);
			string master = r.ReadString();
			int nul = master.IndexOf('\0');
			NUnit.Framework.Assert.IsTrue(nul > 0, "has capability list");
			NUnit.Framework.Assert.AreEqual(B.Name + ' ' + R_MASTER, Sharpen.Runtime.Substring
				(master, 0, nul));
			NUnit.Framework.Assert.AreSame(PacketLineIn.END, r.ReadString());
			NUnit.Framework.Assert.AreEqual("unpack error Missing tree " + t.Name, r.ReadString
				());
			NUnit.Framework.Assert.AreEqual("ng refs/heads/s n/a (unpacker error)", r.ReadString
				());
			NUnit.Framework.Assert.AreSame(PacketLineIn.END, r.ReadString());
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
		private void Copy(TemporaryBuffer.Heap tinyPack, ObjectLoader ldr)
		{
			byte[] buf = new byte[64];
			byte[] content = ldr.GetCachedBytes();
			int dataLength = content.Length;
			int nextLength = (int)(((uint)dataLength) >> 4);
			int size = 0;
			buf[size++] = unchecked((byte)((nextLength > 0 ? unchecked((int)(0x80)) : unchecked(
				(int)(0x00))) | (ldr.GetType() << 4) | (dataLength & unchecked((int)(0x0F)))));
			dataLength = nextLength;
			while (dataLength > 0)
			{
				nextLength = (int)(((uint)nextLength) >> 7);
				buf[size++] = unchecked((byte)((nextLength > 0 ? unchecked((int)(0x80)) : unchecked(
					(int)(0x00))) | (dataLength & unchecked((int)(0x7F)))));
				dataLength = nextLength;
			}
			tinyPack.Write(buf, 0, size);
			Deflate(tinyPack, content);
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

		[NUnit.Framework.TearDown]
		public virtual void Release()
		{
			if (inserter != null)
			{
				inserter.Release();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void OpenPack(TemporaryBuffer.Heap buf)
		{
			if (inserter == null)
			{
				inserter = src.NewObjectInserter();
			}
			byte[] raw = buf.ToByteArray();
			PackParser p = inserter.NewPackParser(new ByteArrayInputStream(raw));
			p.SetAllowThin(true);
			p.Parse(PM);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private static PacketLineIn AsPacketLineIn(TemporaryBuffer.Heap buf)
		{
			return new PacketLineIn(new ByteArrayInputStream(buf.ToByteArray()));
		}

		private sealed class HidePrivateFilter : RefFilter
		{
			public override IDictionary<string, Ref> Filter(IDictionary<string, Ref> refs)
			{
				IDictionary<string, Ref> r = new Dictionary<string, Ref>(refs);
				NUnit.Framework.Assert.IsNotNull(Sharpen.Collections.Remove(r, R_PRIVATE));
				return r;
			}
		}

		/// <exception cref="Sharpen.URISyntaxException"></exception>
		private static URIish UriOf(Repository r)
		{
			return new URIish(r.Directory.GetAbsolutePath());
		}
	}
}
