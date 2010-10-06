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
using NGit.Errors;
using NGit.Junit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	public class UnpackedObjectTest : LocalDiskRepositoryTestCase
	{
		private int streamThreshold = 16 * 1024;

		private TestRng rng;

		private FileRepository repo;

		private WindowCursor wc;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			WindowCacheConfig cfg = new WindowCacheConfig();
			cfg.SetStreamFileThreshold(streamThreshold);
			WindowCache.Reconfigure(cfg);
			rng = new TestRng(Sharpen.Extensions.GetTestName(this));
			repo = CreateBareRepository();
			wc = (WindowCursor)repo.NewObjectReader();
		}

		/// <exception cref="System.Exception"></exception>
		protected override void TearDown()
		{
			if (wc != null)
			{
				wc.Release();
			}
			WindowCache.Reconfigure(new WindowCacheConfig());
			base.TearDown();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStandardFormat_SmallObject()
		{
			int type = Constants.OBJ_BLOB;
			byte[] data = rng.NextBytes(300);
			byte[] gz = CompressStandardFormat(type, data);
			ObjectId id = ObjectId.ZeroId;
			ObjectLoader ol = UnpackedObject.Open(new ByteArrayInputStream(gz), Path(id), id, 
				wc);
			NUnit.Framework.Assert.IsNotNull(ol, "created loader");
			NUnit.Framework.Assert.AreEqual(type, ol.GetType());
			NUnit.Framework.Assert.AreEqual(data.Length, ol.GetSize());
			NUnit.Framework.Assert.IsFalse(ol.IsLarge(), "is not large");
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(data, ol.GetCachedBytes()), "same content"
				);
			ObjectStream @in = ol.OpenStream();
			NUnit.Framework.Assert.IsNotNull(@in, "have stream");
			NUnit.Framework.Assert.AreEqual(type, @in.GetType());
			NUnit.Framework.Assert.AreEqual(data.Length, @in.GetSize());
			byte[] data2 = new byte[data.Length];
			IOUtil.ReadFully(@in, data2, 0, data.Length);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(data2, data), "same content");
			NUnit.Framework.Assert.AreEqual(-1, @in.Read(), "stream at EOF");
			@in.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStandardFormat_LargeObject()
		{
			int type = Constants.OBJ_BLOB;
			byte[] data = rng.NextBytes(streamThreshold + 5);
			ObjectId id = new ObjectInserter.Formatter().IdFor(type, data);
			Write(id, CompressStandardFormat(type, data));
			ObjectLoader ol;
			{
				FileInputStream fs = new FileInputStream(Path(id));
				try
				{
					ol = UnpackedObject.Open(fs, Path(id), id, wc);
				}
				finally
				{
					fs.Close();
				}
			}
			NUnit.Framework.Assert.IsNotNull(ol, "created loader");
			NUnit.Framework.Assert.AreEqual(type, ol.GetType());
			NUnit.Framework.Assert.AreEqual(data.Length, ol.GetSize());
			NUnit.Framework.Assert.IsTrue(ol.IsLarge(), "is large");
			try
			{
				ol.GetCachedBytes();
				NUnit.Framework.Assert.Fail("Should have thrown LargeObjectException");
			}
			catch (LargeObjectException tooBig)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().largeObjectException
					, id.Name), tooBig.Message);
			}
			ObjectStream @in = ol.OpenStream();
			NUnit.Framework.Assert.IsNotNull(@in, "have stream");
			NUnit.Framework.Assert.AreEqual(type, @in.GetType());
			NUnit.Framework.Assert.AreEqual(data.Length, @in.GetSize());
			byte[] data2 = new byte[data.Length];
			IOUtil.ReadFully(@in, data2, 0, data.Length);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(data2, data), "same content");
			NUnit.Framework.Assert.AreEqual(-1, @in.Read(), "stream at EOF");
			@in.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStandardFormat_NegativeSize()
		{
			ObjectId id = ObjectId.ZeroId;
			byte[] data = rng.NextBytes(300);
			try
			{
				byte[] gz = CompressStandardFormat("blob", "-1", data);
				UnpackedObject.Open(new ByteArrayInputStream(gz), Path(id), id, wc);
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectNegativeSize), coe.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStandardFormat_InvalidType()
		{
			ObjectId id = ObjectId.ZeroId;
			byte[] data = rng.NextBytes(300);
			try
			{
				byte[] gz = CompressStandardFormat("not.a.type", "1", data);
				UnpackedObject.Open(new ByteArrayInputStream(gz), Path(id), id, wc);
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectInvalidType), coe.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStandardFormat_NoHeader()
		{
			ObjectId id = ObjectId.ZeroId;
			byte[] data = new byte[] {  };
			try
			{
				byte[] gz = CompressStandardFormat(string.Empty, string.Empty, data);
				UnpackedObject.Open(new ByteArrayInputStream(gz), Path(id), id, wc);
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectNoHeader), coe.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStandardFormat_GarbageAfterSize()
		{
			ObjectId id = ObjectId.ZeroId;
			byte[] data = rng.NextBytes(300);
			try
			{
				byte[] gz = CompressStandardFormat("blob", "1foo", data);
				UnpackedObject.Open(new ByteArrayInputStream(gz), Path(id), id, wc);
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectGarbageAfterSize), coe.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStandardFormat_SmallObject_CorruptZLibStream()
		{
			ObjectId id = ObjectId.ZeroId;
			byte[] data = rng.NextBytes(300);
			try
			{
				byte[] gz = CompressStandardFormat(Constants.OBJ_BLOB, data);
				for (int i = 5; i < gz.Length; i++)
				{
					gz[i] = 0;
				}
				UnpackedObject.Open(new ByteArrayInputStream(gz), Path(id), id, wc);
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectBadStream), coe.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStandardFormat_SmallObject_TruncatedZLibStream()
		{
			ObjectId id = ObjectId.ZeroId;
			byte[] data = rng.NextBytes(300);
			try
			{
				byte[] gz = CompressStandardFormat(Constants.OBJ_BLOB, data);
				byte[] tr = new byte[gz.Length - 1];
				System.Array.Copy(gz, 0, tr, 0, tr.Length);
				UnpackedObject.Open(new ByteArrayInputStream(tr), Path(id), id, wc);
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectBadStream), coe.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStandardFormat_SmallObject_TrailingGarbage()
		{
			ObjectId id = ObjectId.ZeroId;
			byte[] data = rng.NextBytes(300);
			try
			{
				byte[] gz = CompressStandardFormat(Constants.OBJ_BLOB, data);
				byte[] tr = new byte[gz.Length + 1];
				System.Array.Copy(gz, 0, tr, 0, gz.Length);
				UnpackedObject.Open(new ByteArrayInputStream(tr), Path(id), id, wc);
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectBadStream), coe.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStandardFormat_LargeObject_CorruptZLibStream()
		{
			int type = Constants.OBJ_BLOB;
			byte[] data = rng.NextBytes(streamThreshold + 5);
			ObjectId id = new ObjectInserter.Formatter().IdFor(type, data);
			byte[] gz = CompressStandardFormat(type, data);
			gz[gz.Length - 1] = 0;
			gz[gz.Length - 2] = 0;
			Write(id, gz);
			ObjectLoader ol;
			{
				FileInputStream fs = new FileInputStream(Path(id));
				try
				{
					ol = UnpackedObject.Open(fs, Path(id), id, wc);
				}
				finally
				{
					fs.Close();
				}
			}
			try
			{
				byte[] tmp = new byte[data.Length];
				InputStream @in = ol.OpenStream();
				try
				{
					IOUtil.ReadFully(@in, tmp, 0, tmp.Length);
				}
				finally
				{
					@in.Close();
				}
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectBadStream), coe.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStandardFormat_LargeObject_TruncatedZLibStream()
		{
			int type = Constants.OBJ_BLOB;
			byte[] data = rng.NextBytes(streamThreshold + 5);
			ObjectId id = new ObjectInserter.Formatter().IdFor(type, data);
			byte[] gz = CompressStandardFormat(type, data);
			byte[] tr = new byte[gz.Length - 1];
			System.Array.Copy(gz, 0, tr, 0, tr.Length);
			Write(id, tr);
			ObjectLoader ol;
			{
				FileInputStream fs = new FileInputStream(Path(id));
				try
				{
					ol = UnpackedObject.Open(fs, Path(id), id, wc);
				}
				finally
				{
					fs.Close();
				}
			}
			byte[] tmp = new byte[data.Length];
			InputStream @in = ol.OpenStream();
			IOUtil.ReadFully(@in, tmp, 0, tmp.Length);
			try
			{
				@in.Close();
				NUnit.Framework.Assert.Fail("close did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectBadStream), coe.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestStandardFormat_LargeObject_TrailingGarbage()
		{
			int type = Constants.OBJ_BLOB;
			byte[] data = rng.NextBytes(streamThreshold + 5);
			ObjectId id = new ObjectInserter.Formatter().IdFor(type, data);
			byte[] gz = CompressStandardFormat(type, data);
			byte[] tr = new byte[gz.Length + 1];
			System.Array.Copy(gz, 0, tr, 0, gz.Length);
			Write(id, tr);
			ObjectLoader ol;
			{
				FileInputStream fs = new FileInputStream(Path(id));
				try
				{
					ol = UnpackedObject.Open(fs, Path(id), id, wc);
				}
				finally
				{
					fs.Close();
				}
			}
			byte[] tmp = new byte[data.Length];
			InputStream @in = ol.OpenStream();
			IOUtil.ReadFully(@in, tmp, 0, tmp.Length);
			try
			{
				@in.Close();
				NUnit.Framework.Assert.Fail("close did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectBadStream), coe.Message);
			}
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPackFormat_SmallObject()
		{
			int type = Constants.OBJ_BLOB;
			byte[] data = rng.NextBytes(300);
			byte[] gz = CompressPackFormat(type, data);
			ObjectId id = ObjectId.ZeroId;
			ObjectLoader ol = UnpackedObject.Open(new ByteArrayInputStream(gz), Path(id), id, 
				wc);
			NUnit.Framework.Assert.IsNotNull(ol, "created loader");
			NUnit.Framework.Assert.AreEqual(type, ol.GetType());
			NUnit.Framework.Assert.AreEqual(data.Length, ol.GetSize());
			NUnit.Framework.Assert.IsFalse(ol.IsLarge(), "is not large");
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(data, ol.GetCachedBytes()), "same content"
				);
			ObjectStream @in = ol.OpenStream();
			NUnit.Framework.Assert.IsNotNull(@in, "have stream");
			NUnit.Framework.Assert.AreEqual(type, @in.GetType());
			NUnit.Framework.Assert.AreEqual(data.Length, @in.GetSize());
			byte[] data2 = new byte[data.Length];
			IOUtil.ReadFully(@in, data2, 0, data.Length);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(data, ol.GetCachedBytes()), "same content"
				);
			@in.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPackFormat_LargeObject()
		{
			int type = Constants.OBJ_BLOB;
			byte[] data = rng.NextBytes(streamThreshold + 5);
			ObjectId id = new ObjectInserter.Formatter().IdFor(type, data);
			Write(id, CompressPackFormat(type, data));
			ObjectLoader ol;
			{
				FileInputStream fs = new FileInputStream(Path(id));
				try
				{
					ol = UnpackedObject.Open(fs, Path(id), id, wc);
				}
				finally
				{
					fs.Close();
				}
			}
			NUnit.Framework.Assert.IsNotNull(ol, "created loader");
			NUnit.Framework.Assert.AreEqual(type, ol.GetType());
			NUnit.Framework.Assert.AreEqual(data.Length, ol.GetSize());
			NUnit.Framework.Assert.IsTrue(ol.IsLarge(), "is large");
			try
			{
				ol.GetCachedBytes();
				NUnit.Framework.Assert.Fail("Should have thrown LargeObjectException");
			}
			catch (LargeObjectException tooBig)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().largeObjectException
					, id.Name), tooBig.Message);
			}
			ObjectStream @in = ol.OpenStream();
			NUnit.Framework.Assert.IsNotNull(@in, "have stream");
			NUnit.Framework.Assert.AreEqual(type, @in.GetType());
			NUnit.Framework.Assert.AreEqual(data.Length, @in.GetSize());
			byte[] data2 = new byte[data.Length];
			IOUtil.ReadFully(@in, data2, 0, data.Length);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(data2, data), "same content");
			NUnit.Framework.Assert.AreEqual(-1, @in.Read(), "stream at EOF");
			@in.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestPackFormat_DeltaNotAllowed()
		{
			ObjectId id = ObjectId.ZeroId;
			byte[] data = rng.NextBytes(300);
			try
			{
				byte[] gz = CompressPackFormat(Constants.OBJ_OFS_DELTA, data);
				UnpackedObject.Open(new ByteArrayInputStream(gz), Path(id), id, wc);
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectInvalidType), coe.Message);
			}
			try
			{
				byte[] gz = CompressPackFormat(Constants.OBJ_REF_DELTA, data);
				UnpackedObject.Open(new ByteArrayInputStream(gz), Path(id), id, wc);
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectInvalidType), coe.Message);
			}
			try
			{
				byte[] gz = CompressPackFormat(Constants.OBJ_TYPE_5, data);
				UnpackedObject.Open(new ByteArrayInputStream(gz), Path(id), id, wc);
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectInvalidType), coe.Message);
			}
			try
			{
				byte[] gz = CompressPackFormat(Constants.OBJ_EXT, data);
				UnpackedObject.Open(new ByteArrayInputStream(gz), Path(id), id, wc);
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException coe)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().objectIsCorrupt
					, id.Name, JGitText.Get().corruptObjectInvalidType), coe.Message);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private byte[] CompressStandardFormat(int type, byte[] data)
		{
			string typeString = Constants.TypeString(type);
			string length = data.Length.ToString();
			return CompressStandardFormat(typeString, length, data);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private byte[] CompressStandardFormat(string type, string length, byte[] data)
		{
			ByteArrayOutputStream @out = new ByteArrayOutputStream();
			DeflaterOutputStream d = new DeflaterOutputStream(@out);
			d.Write(Constants.EncodeASCII(type));
			d.Write(' ');
			d.Write(Constants.EncodeASCII(length));
			d.Write(0);
			d.Write(data);
			d.Finish();
			return @out.ToByteArray();
		}

		/// <exception cref="System.IO.IOException"></exception>
		private byte[] CompressPackFormat(int type, byte[] data)
		{
			byte[] hdr = new byte[64];
			int rawLength = data.Length;
			int nextLength = (int)(((uint)rawLength) >> 4);
			hdr[0] = unchecked((byte)((nextLength > 0 ? unchecked((int)(0x80)) : unchecked((int
				)(0x00))) | (type << 4) | (rawLength & unchecked((int)(0x0F)))));
			rawLength = nextLength;
			int n = 1;
			while (rawLength > 0)
			{
				nextLength = (int)(((uint)nextLength) >> 7);
				hdr[n++] = unchecked((byte)((nextLength > 0 ? unchecked((int)(0x80)) : unchecked(
					(int)(0x00))) | (rawLength & unchecked((int)(0x7F)))));
				rawLength = nextLength;
			}
			ByteArrayOutputStream @out = new ByteArrayOutputStream();
			@out.Write(hdr, 0, n);
			DeflaterOutputStream d = new DeflaterOutputStream(@out);
			d.Write(data);
			d.Finish();
			return @out.ToByteArray();
		}

		private FilePath Path(ObjectId id)
		{
			return ((ObjectDirectory)repo.ObjectDatabase).FileFor(id);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Write(ObjectId id, byte[] data)
		{
			FilePath path = Path(id);
			path.GetParentFile().Mkdirs();
			FileOutputStream @out = new FileOutputStream(path);
			try
			{
				@out.Write(data);
			}
			finally
			{
				@out.Close();
			}
		}
	}
}
