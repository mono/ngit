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
using NGit.Junit;
using NGit.Storage.Pack;
using Sharpen;

namespace NGit.Storage.Pack
{
	[NUnit.Framework.TestFixture]
	public class DeltaIndexTest
	{
		private TestRng rng;

		private ByteArrayOutputStream actDeltaBuf;

		private ByteArrayOutputStream expDeltaBuf;

		private DeltaEncoder expDeltaEnc;

		private byte[] src;

		private byte[] dst;

		private ByteArrayOutputStream dstBuf;

		private TestRng GetRng()
		{
			if (rng == null)
			{
				rng = new TestRng(Sharpen.Extensions.GetTestName());
			}
			return rng;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			actDeltaBuf = new ByteArrayOutputStream();
			expDeltaBuf = new ByteArrayOutputStream();
			expDeltaEnc = new DeltaEncoder(expDeltaBuf, 0, 0);
			dstBuf = new ByteArrayOutputStream();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestInsertWholeObject_Length12()
		{
			src = GetRng().NextBytes(12);
			Insert(src);
			DoTest();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCopyWholeObject_Length128()
		{
			src = GetRng().NextBytes(128);
			Copy(0, 128);
			DoTest();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCopyWholeObject_Length123()
		{
			src = GetRng().NextBytes(123);
			Copy(0, 123);
			DoTest();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCopyZeros_Length128()
		{
			src = new byte[2048];
			Copy(0, src.Length);
			DoTest();
			// The index should be smaller than expected due to the chain
			// being truncated. Without truncation we would expect to have
			// more than 3584 bytes used.
			//
			NUnit.Framework.Assert.AreEqual(2636, new DeltaIndex(src).GetIndexSize());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestShuffleSegments()
		{
			src = GetRng().NextBytes(128);
			Copy(64, 64);
			Copy(0, 64);
			DoTest();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestInsertHeadMiddle()
		{
			src = GetRng().NextBytes(1024);
			Insert("foo");
			Copy(0, 512);
			Insert("yet more fooery");
			Copy(0, 512);
			DoTest();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestInsertTail()
		{
			src = GetRng().NextBytes(1024);
			Copy(0, 512);
			Insert("bar");
			DoTest();
		}

		[NUnit.Framework.Test]
		public virtual void TestIndexSize()
		{
			src = GetRng().NextBytes(1024);
			DeltaIndex di = new DeltaIndex(src);
			NUnit.Framework.Assert.AreEqual(1860, di.GetIndexSize());
			NUnit.Framework.Assert.AreEqual("DeltaIndex[2 KiB]", di.ToString());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLimitObjectSize_Length12InsertFails()
		{
			src = GetRng().NextBytes(12);
			dst = src;
			DeltaIndex di = new DeltaIndex(src);
			NUnit.Framework.Assert.IsFalse(di.Encode(actDeltaBuf, dst, src.Length));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLimitObjectSize_Length130InsertFails()
		{
			src = GetRng().NextBytes(130);
			dst = GetRng().NextBytes(130);
			DeltaIndex di = new DeltaIndex(src);
			NUnit.Framework.Assert.IsFalse(di.Encode(actDeltaBuf, dst, src.Length));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLimitObjectSize_Length130CopyOk()
		{
			src = GetRng().NextBytes(130);
			Copy(0, 130);
			dst = dstBuf.ToByteArray();
			DeltaIndex di = new DeltaIndex(src);
			NUnit.Framework.Assert.IsTrue(di.Encode(actDeltaBuf, dst, dst.Length));
			byte[] actDelta = actDeltaBuf.ToByteArray();
			byte[] expDelta = expDeltaBuf.ToByteArray();
			NUnit.Framework.Assert.AreEqual(BinaryDelta.Format(expDelta, false), BinaryDelta.
				Format(actDelta, false));
		}

		//
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLimitObjectSize_Length130CopyFails()
		{
			src = GetRng().NextBytes(130);
			Copy(0, 130);
			dst = dstBuf.ToByteArray();
			// The header requires 4 bytes for these objects, so a target length
			// of 5 is bigger than the copy instruction and should cause an abort.
			//
			DeltaIndex di = new DeltaIndex(src);
			NUnit.Framework.Assert.IsFalse(di.Encode(actDeltaBuf, dst, 5));
			NUnit.Framework.Assert.AreEqual(4, actDeltaBuf.Size());
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLimitObjectSize_InsertFrontFails()
		{
			src = GetRng().NextBytes(130);
			Insert("eight");
			Copy(0, 130);
			dst = dstBuf.ToByteArray();
			// The header requires 4 bytes for these objects, so a target length
			// of 5 is bigger than the copy instruction and should cause an abort.
			//
			DeltaIndex di = new DeltaIndex(src);
			NUnit.Framework.Assert.IsFalse(di.Encode(actDeltaBuf, dst, 5));
			NUnit.Framework.Assert.AreEqual(4, actDeltaBuf.Size());
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Copy(int offset, int len)
		{
			dstBuf.Write(src, offset, len);
			expDeltaEnc.Copy(offset, len);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Insert(string text)
		{
			Insert(Constants.Encode(text));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Insert(byte[] text)
		{
			dstBuf.Write(text);
			expDeltaEnc.Insert(text);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void DoTest()
		{
			dst = dstBuf.ToByteArray();
			DeltaIndex di = new DeltaIndex(src);
			di.Encode(actDeltaBuf, dst);
			byte[] actDelta = actDeltaBuf.ToByteArray();
			byte[] expDelta = expDeltaBuf.ToByteArray();
			NUnit.Framework.Assert.AreEqual(BinaryDelta.Format(expDelta, false), BinaryDelta.
				Format(actDelta, false));
			//
			NUnit.Framework.Assert.IsTrue(actDelta.Length > 0, "delta is not empty");
			NUnit.Framework.Assert.AreEqual(src.Length, BinaryDelta.GetBaseSize(actDelta));
			NUnit.Framework.Assert.AreEqual(dst.Length, BinaryDelta.GetResultSize(actDelta));
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(dst, BinaryDelta.Apply(src, actDelta)
				));
		}
	}
}
