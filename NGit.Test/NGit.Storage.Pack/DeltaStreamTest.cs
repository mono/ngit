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
using NGit.Internal;
using NGit.Junit;
using NGit.Storage.Pack;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Storage.Pack
{
	[NUnit.Framework.TestFixture]
	public class DeltaStreamTest
	{
		private TestRng rng;

		private ByteArrayOutputStream deltaBuf;

		private DeltaEncoder deltaEnc;

		private byte[] @base;

		private byte[] data;

		private int dataPtr;

		private byte[] delta;

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
			deltaBuf = new ByteArrayOutputStream();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCopy_SingleOp()
		{
			Init((1 << 16) + 1, (1 << 8) + 1);
			Copy(0, data.Length);
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCopy_MaxSize()
		{
			int max = (unchecked((int)(0xff)) << 16) + (unchecked((int)(0xff)) << 8) + unchecked(
				(int)(0xff));
			Init(1 + max, max);
			Copy(1, max);
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCopy_64k()
		{
			Init(unchecked((int)(0x10000)) + 2, unchecked((int)(0x10000)) + 1);
			Copy(1, unchecked((int)(0x10000)));
			Copy(unchecked((int)(0x10001)), 1);
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCopy_Gap()
		{
			Init(256, 8);
			Copy(4, 4);
			Copy(128, 4);
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCopy_OutOfOrder()
		{
			Init((1 << 16) + 1, (1 << 16) + 1);
			Copy(1 << 8, 1 << 8);
			Copy(0, data.Length - dataPtr);
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestInsert_SingleOp()
		{
			Init((1 << 16) + 1, 2);
			Insert("hi");
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestInsertAndCopy()
		{
			Init(8, 512);
			Insert(new byte[127]);
			Insert(new byte[127]);
			Insert(new byte[127]);
			Insert(new byte[125]);
			Copy(2, 6);
			AssertValidState();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSkip()
		{
			Init(32, 15);
			Copy(2, 2);
			Insert("ab");
			Insert("cd");
			Copy(4, 4);
			Copy(0, 2);
			Insert("efg");
			AssertValidState();
			for (int p = 0; p < data.Length; p++)
			{
				byte[] act = new byte[data.Length];
				System.Array.Copy(data, 0, act, 0, p);
				DeltaStream @in = Open();
				IOUtil.SkipFully(@in, p);
				NUnit.Framework.Assert.AreEqual(data.Length - p, @in.Read(act, p, data.Length - p
					));
				NUnit.Framework.Assert.AreEqual(-1, @in.Read());
				NUnit.Framework.Assert.IsTrue(Arrays.Equals(data, act), "skipping " + p);
			}
			// Skip all the way to the end should still recognize EOF.
			DeltaStream in_1 = Open();
			IOUtil.SkipFully(in_1, data.Length);
			NUnit.Framework.Assert.AreEqual(-1, in_1.Read());
			NUnit.Framework.Assert.AreEqual(0, in_1.Skip(1));
			// Skip should not open the base as we move past it, but it
			// will open when we need to start copying data from it.
			bool[] opened = new bool[1];
			in_1 = new _DeltaStream_180(this, opened, new ByteArrayInputStream(delta));
			IOUtil.SkipFully(in_1, 7);
			NUnit.Framework.Assert.IsFalse(opened[0], "not yet open");
			NUnit.Framework.Assert.AreEqual(data[7], in_1.Read());
			NUnit.Framework.Assert.IsTrue(opened[0], "now open");
		}

		private sealed class _DeltaStream_180 : DeltaStream
		{
			public _DeltaStream_180(DeltaStreamTest _enclosing, bool[] opened, InputStream baseArg1
				) : base(baseArg1)
			{
				this._enclosing = _enclosing;
				this.opened = opened;
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal protected override long GetBaseSize()
			{
				return this._enclosing.@base.Length;
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal protected override InputStream OpenBase()
			{
				opened[0] = true;
				return new ByteArrayInputStream(this._enclosing.@base);
			}

			private readonly DeltaStreamTest _enclosing;

			private readonly bool[] opened;
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestIncorrectBaseSize()
		{
			Init(4, 4);
			Copy(0, 4);
			AssertValidState();
			DeltaStream @in = new _DeltaStream_204(this, new ByteArrayInputStream(delta));
			try
			{
				@in.Read(new byte[4]);
				NUnit.Framework.Assert.Fail("did not throw an exception");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual(JGitText.Get().baseLengthIncorrect, e.Message);
			}
			@in = new _DeltaStream_222(new ByteArrayInputStream(delta));
			try
			{
				@in.Read(new byte[4]);
				NUnit.Framework.Assert.Fail("did not throw an exception");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual(JGitText.Get().baseLengthIncorrect, e.Message);
			}
		}

		private sealed class _DeltaStream_204 : DeltaStream
		{
			public _DeltaStream_204(DeltaStreamTest _enclosing, InputStream baseArg1) : base(
				baseArg1)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal protected override long GetBaseSize()
			{
				return 128;
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal protected override InputStream OpenBase()
			{
				return new ByteArrayInputStream(this._enclosing.@base);
			}

			private readonly DeltaStreamTest _enclosing;
		}

		private sealed class _DeltaStream_222 : DeltaStream
		{
			public _DeltaStream_222(InputStream baseArg1) : base(baseArg1)
			{
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal protected override long GetBaseSize()
			{
				return 4;
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal protected override InputStream OpenBase()
			{
				return new ByteArrayInputStream(new byte[0]);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Init(int baseSize, int dataSize)
		{
			@base = GetRng().NextBytes(baseSize);
			data = new byte[dataSize];
			deltaEnc = new DeltaEncoder(deltaBuf, baseSize, dataSize);
			dataPtr = 0;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Copy(int offset, int len)
		{
			System.Array.Copy(@base, offset, data, dataPtr, len);
			deltaEnc.Copy(offset, len);
			NUnit.Framework.Assert.AreEqual(deltaBuf.Size(), deltaEnc.GetSize());
			dataPtr += len;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Insert(string text)
		{
			Insert(Constants.Encode(text));
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Insert(byte[] text)
		{
			System.Array.Copy(text, 0, data, dataPtr, text.Length);
			deltaEnc.Insert(text);
			NUnit.Framework.Assert.AreEqual(deltaBuf.Size(), deltaEnc.GetSize());
			dataPtr += text.Length;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertValidState()
		{
			NUnit.Framework.Assert.AreEqual(data.Length, dataPtr, "test filled example result"
				);
			delta = deltaBuf.ToByteArray();
			NUnit.Framework.Assert.AreEqual(@base.Length, BinaryDelta.GetBaseSize(delta));
			NUnit.Framework.Assert.AreEqual(data.Length, BinaryDelta.GetResultSize(delta));
			var appliedDelta = BinaryDelta.Apply (@base, delta);
			Assert.AreEqual (data.Length, appliedDelta.Length);
			for (int i = 0; i < data.Length; i++)
				Assert.AreEqual (data[i], appliedDelta[i]);

			// Assert that a single bulk read produces the correct result.
			//
			byte[] act = new byte[data.Length];
			DeltaStream @in = Open();
			NUnit.Framework.Assert.AreEqual(data.Length, @in.GetSize());
			NUnit.Framework.Assert.AreEqual(data.Length, @in.Read(act));
			NUnit.Framework.Assert.AreEqual(-1, @in.Read());
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(data, act), "bulk read has same content"
				);
			// Assert that smaller tiny reads have the same result too.
			//
			act = new byte[data.Length];
			@in = Open();
			int read = 0;
			while (read < data.Length)
			{
				int n = @in.Read(act, read, 128);
				if (n <= 0)
				{
					break;
				}
				read += n;
			}
			NUnit.Framework.Assert.AreEqual(data.Length, read);
			NUnit.Framework.Assert.AreEqual(-1, @in.Read());
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(data, act), "small reads have same content"
				);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private DeltaStream Open()
		{
			return new _DeltaStream_299(this, new ByteArrayInputStream(delta));
		}

		private sealed class _DeltaStream_299 : DeltaStream
		{
			public _DeltaStream_299(DeltaStreamTest _enclosing, InputStream baseArg1) : base(
				baseArg1)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal protected override long GetBaseSize()
			{
				return this._enclosing.@base.Length;
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal protected override InputStream OpenBase()
			{
				return new ByteArrayInputStream(this._enclosing.@base);
			}

			private readonly DeltaStreamTest _enclosing;
		}
	}
}
