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
		protected override void SetUp()
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
				AssertEquals(B, src.Resolve(R_MASTER));
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
		protected override void TearDown()
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
		public virtual void TestFilterHidesPrivate()
		{
			IDictionary<string, Ref> refs;
			TransportLocal t = new _TransportLocal_131(this, src, UriOf(dst));
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
			NUnit.Framework.Assert.IsNull("no private", refs.Get(R_PRIVATE));
			NUnit.Framework.Assert.IsNull("no HEAD", refs.Get(Constants.HEAD));
			NUnit.Framework.Assert.AreEqual(1, refs.Count);
			Ref master = refs.Get(R_MASTER);
			NUnit.Framework.Assert.IsNotNull("has master", master);
			AssertEquals(B, master.GetObjectId());
		}

		private sealed class _TransportLocal_131 : TransportLocal
		{
			public _TransportLocal_131(ReceivePackRefFilterTest _enclosing, Repository baseArg1
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
			NUnit.Framework.Assert.IsTrue("has b", src.HasObject(b));
			NUnit.Framework.Assert.IsFalse("b not loose", od.FileFor(b).Exists());
			// Now use b but in a different commit than what is hidden.
			//
			TestRepository s = new TestRepository(src);
			RevCommit N = s.Commit().Parent(B).Add("q", b).Create();
			s.Update(R_MASTER, N);
			// Push this new content to the remote, doing strict validation.
			//
			TransportLocal t = new _TransportLocal_193(this, src, UriOf(dst));
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
			NUnit.Framework.Assert.IsNotNull("have result", r);
			NUnit.Framework.Assert.IsNull("private not advertised", r.GetAdvertisedRef(R_PRIVATE
				));
			NUnit.Framework.Assert.AreSame("master updated", RemoteRefUpdate.Status.OK, u.GetStatus
				());
			AssertEquals(N, dst.Resolve(R_MASTER));
		}

		private sealed class _TransportLocal_193 : TransportLocal
		{
			public _TransportLocal_193(ReceivePackRefFilterTest _enclosing, Repository baseArg1
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
				AssertEquals(P, moe.GetObjectId());
			}
			PacketLineIn r = AsPacketLineIn(outBuf);
			string master = r.ReadString();
			int nul = master.IndexOf('\0');
			NUnit.Framework.Assert.IsTrue("has capability list", nul > 0);
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
				AssertEquals(b, moe.GetObjectId());
			}
			PacketLineIn r = AsPacketLineIn(outBuf);
			string master = r.ReadString();
			int nul = master.IndexOf('\0');
			NUnit.Framework.Assert.IsTrue("has capability list", nul > 0);
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
				AssertEquals(b, moe.GetObjectId());
			}
			PacketLineIn r = AsPacketLineIn(outBuf);
			string master = r.ReadString();
			int nul = master.IndexOf('\0');
			NUnit.Framework.Assert.IsTrue("has capability list", nul > 0);
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
				AssertEquals(n, moe.GetObjectId());
			}
			PacketLineIn r = AsPacketLineIn(outBuf);
			string master = r.ReadString();
			int nul = master.IndexOf('\0');
			NUnit.Framework.Assert.IsTrue("has capability list", nul > 0);
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
				AssertEquals(t, moe.GetObjectId());
			}
			PacketLineIn r = AsPacketLineIn(outBuf);
			string master = r.ReadString();
			int nul = master.IndexOf('\0');
			NUnit.Framework.Assert.IsTrue("has capability list", nul > 0);
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

		/// <exception cref="System.IO.IOException"></exception>
		private void OpenPack(TemporaryBuffer.Heap buf)
		{
			byte[] raw = buf.ToByteArray();
			IndexPack ip = IndexPack.Create(src, new ByteArrayInputStream(raw));
			ip.SetFixThin(true);
			ip.Index(PM);
			ip.RenameAndOpenPack();
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
