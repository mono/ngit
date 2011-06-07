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
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class ObjectLoaderTest
	{
		private TestRng rng;

		private TestRng GetRng()
		{
			if (rng == null)
			{
				rng = new TestRng(Sharpen.Extensions.GetTestName());
			}
			return rng;
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestSmallObjectLoader()
		{
			byte[] act = GetRng().NextBytes(512);
			ObjectLoader ldr = new ObjectLoader.SmallObject(Constants.OBJ_BLOB, act);
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_BLOB, ldr.GetType());
			NUnit.Framework.Assert.AreEqual(act.Length, ldr.GetSize());
			NUnit.Framework.Assert.IsFalse(ldr.IsLarge(), "not is large");
			NUnit.Framework.Assert.AreSame(act, ldr.GetCachedBytes());
			NUnit.Framework.Assert.AreSame(act, ldr.GetCachedBytes(1));
			NUnit.Framework.Assert.AreSame(act, ldr.GetCachedBytes(int.MaxValue));
			byte[] copy = ldr.GetBytes();
			NUnit.Framework.Assert.AreNotSame(act, copy);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(act, copy), "same content");
			copy = ldr.GetBytes(1);
			NUnit.Framework.Assert.AreNotSame(act, copy);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(act, copy), "same content");
			copy = ldr.GetBytes(int.MaxValue);
			NUnit.Framework.Assert.AreNotSame(act, copy);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(act, copy), "same content");
			ObjectStream @in = ldr.OpenStream();
			NUnit.Framework.Assert.IsNotNull(@in, "has stream");
			NUnit.Framework.Assert.IsTrue(@in is ObjectStream.SmallStream, "is small stream");
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_BLOB, @in.GetType());
			NUnit.Framework.Assert.AreEqual(act.Length, @in.GetSize());
			NUnit.Framework.Assert.AreEqual(act.Length, @in.Available());
			NUnit.Framework.Assert.IsTrue(@in.MarkSupported(), "mark supported");
			copy = new byte[act.Length];
			NUnit.Framework.Assert.AreEqual(act.Length, @in.Read(copy));
			NUnit.Framework.Assert.AreEqual(0, @in.Available());
			NUnit.Framework.Assert.AreEqual(-1, @in.Read());
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(act, copy), "same content");
			ByteArrayOutputStream tmp = new ByteArrayOutputStream();
			ldr.CopyTo(tmp);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(act, tmp.ToByteArray()), "same content"
				);
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLargeObjectLoader()
		{
			byte[] act = GetRng().NextBytes(512);
			ObjectLoader ldr = new _ObjectLoader_122(act);
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_BLOB, ldr.GetType());
			NUnit.Framework.Assert.AreEqual(act.Length, ldr.GetSize());
			NUnit.Framework.Assert.IsTrue(ldr.IsLarge(), "is large");
			try
			{
				ldr.GetCachedBytes();
				NUnit.Framework.Assert.Fail("did not throw on getCachedBytes()");
			}
			catch (LargeObjectException)
			{
			}
			// expected
			try
			{
				ldr.GetBytes();
				NUnit.Framework.Assert.Fail("did not throw on getBytes()");
			}
			catch (LargeObjectException)
			{
			}
			// expected
			try
			{
				ldr.GetCachedBytes(64);
				NUnit.Framework.Assert.Fail("did not throw on getCachedBytes(64)");
			}
			catch (LargeObjectException)
			{
			}
			// expected
			byte[] copy = ldr.GetCachedBytes(1024);
			NUnit.Framework.Assert.AreNotSame(act, copy);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(act, copy), "same content");
			ObjectStream @in = ldr.OpenStream();
			NUnit.Framework.Assert.IsNotNull(@in, "has stream");
			NUnit.Framework.Assert.AreEqual(Constants.OBJ_BLOB, @in.GetType());
			NUnit.Framework.Assert.AreEqual(act.Length, @in.GetSize());
			NUnit.Framework.Assert.AreEqual(act.Length, @in.Available());
			NUnit.Framework.Assert.IsTrue(@in.MarkSupported(), "mark supported");
			copy = new byte[act.Length];
			NUnit.Framework.Assert.AreEqual(act.Length, @in.Read(copy));
			NUnit.Framework.Assert.AreEqual(0, @in.Available());
			NUnit.Framework.Assert.AreEqual(-1, @in.Read());
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(act, copy), "same content");
			ByteArrayOutputStream tmp = new ByteArrayOutputStream();
			ldr.CopyTo(tmp);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(act, tmp.ToByteArray()), "same content"
				);
		}

		private sealed class _ObjectLoader_122 : ObjectLoader
		{
			public _ObjectLoader_122(byte[] act)
			{
				this.act = act;
			}

			/// <exception cref="NGit.Errors.LargeObjectException"></exception>
			public override byte[] GetCachedBytes()
			{
				throw new LargeObjectException();
			}

			public override long GetSize()
			{
				return act.Length;
			}

			public override int GetType()
			{
				return Constants.OBJ_BLOB;
			}

			/// <exception cref="NGit.Errors.MissingObjectException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override ObjectStream OpenStream()
			{
				return new ObjectStream.Filter(this.GetType(), act.Length, new ByteArrayInputStream
					(act));
			}

			private readonly byte[] act;
		}

		/// <exception cref="NGit.Errors.LargeObjectException"></exception>
		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLimitedGetCachedBytes()
		{
			byte[] act = GetRng().NextBytes(512);
			ObjectLoader ldr = new _SmallObject_196(Constants.OBJ_BLOB, act);
			NUnit.Framework.Assert.IsTrue(ldr.IsLarge(), "is large");
			try
			{
				ldr.GetCachedBytes(10);
				NUnit.Framework.Assert.Fail("Did not throw LargeObjectException");
			}
			catch (LargeObjectException)
			{
			}
			// Expected result.
			byte[] copy = ldr.GetCachedBytes(512);
			NUnit.Framework.Assert.AreNotSame(act, copy);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(act, copy), "same content");
			copy = ldr.GetCachedBytes(1024);
			NUnit.Framework.Assert.AreNotSame(act, copy);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(act, copy), "same content");
		}

		private sealed class _SmallObject_196 : ObjectLoader.SmallObject
		{
			public _SmallObject_196(int baseArg1, byte[] baseArg2) : base(baseArg1, baseArg2)
			{
			}

			public override bool IsLarge()
			{
				return true;
			}
		}

		/// <exception cref="NGit.Errors.LargeObjectException"></exception>
		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLimitedGetCachedBytesExceedsJavaLimits()
		{
			ObjectLoader ldr = new _ObjectLoader_223();
			NUnit.Framework.Assert.IsTrue(ldr.IsLarge(), "is large");
			try
			{
				ldr.GetCachedBytes(10);
				NUnit.Framework.Assert.Fail("Did not throw LargeObjectException");
			}
			catch (LargeObjectException)
			{
			}
			// Expected result.
			try
			{
				ldr.GetCachedBytes(int.MaxValue);
				NUnit.Framework.Assert.Fail("Did not throw LargeObjectException");
			}
			catch (LargeObjectException)
			{
			}
		}

		private sealed class _ObjectLoader_223 : ObjectLoader
		{
			public _ObjectLoader_223()
			{
			}

			public override bool IsLarge()
			{
				return true;
			}

			/// <exception cref="NGit.Errors.LargeObjectException"></exception>
			public override byte[] GetCachedBytes()
			{
				throw new LargeObjectException();
			}

			public override long GetSize()
			{
				return long.MaxValue;
			}

			public override int GetType()
			{
				return Constants.OBJ_BLOB;
			}

			/// <exception cref="NGit.Errors.MissingObjectException"></exception>
			/// <exception cref="System.IO.IOException"></exception>
			public override ObjectStream OpenStream()
			{
				return new _ObjectStream_247();
			}

			private sealed class _ObjectStream_247 : ObjectStream
			{
				public _ObjectStream_247()
				{
				}

				public override long GetSize()
				{
					return long.MaxValue;
				}

				public override int GetType()
				{
					return Constants.OBJ_BLOB;
				}

				/// <exception cref="System.IO.IOException"></exception>
				public override int Read()
				{
					NUnit.Framework.Assert.Fail("never should have reached read");
					return -1;
				}
			}
		}
		// Expected result.
	}
}
