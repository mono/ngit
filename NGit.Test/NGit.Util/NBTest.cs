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

using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class NBTest : TestCase
	{
		public virtual void TestCompareUInt32()
		{
			NUnit.Framework.Assert.IsTrue(NB.CompareUInt32(0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(NB.CompareUInt32(1, 0) > 0);
			NUnit.Framework.Assert.IsTrue(NB.CompareUInt32(0, 1) < 0);
			NUnit.Framework.Assert.IsTrue(NB.CompareUInt32(-1, 0) > 0);
			NUnit.Framework.Assert.IsTrue(NB.CompareUInt32(0, -1) < 0);
			NUnit.Framework.Assert.IsTrue(NB.CompareUInt32(-1, 1) > 0);
			NUnit.Framework.Assert.IsTrue(NB.CompareUInt32(1, -1) < 0);
		}

		public virtual void TestDecodeUInt16()
		{
			NUnit.Framework.Assert.AreEqual(0, NB.DecodeUInt16(B(0, 0), 0));
			NUnit.Framework.Assert.AreEqual(0, NB.DecodeUInt16(Padb(3, 0, 0), 3));
			NUnit.Framework.Assert.AreEqual(3, NB.DecodeUInt16(B(0, 3), 0));
			NUnit.Framework.Assert.AreEqual(3, NB.DecodeUInt16(Padb(3, 0, 3), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xde03)), NB.DecodeUInt16(B(unchecked(
				(int)(0xde)), 3), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xde03)), NB.DecodeUInt16(Padb(3, 
				unchecked((int)(0xde)), 3), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x03de)), NB.DecodeUInt16(B(3, unchecked(
				(int)(0xde))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x03de)), NB.DecodeUInt16(Padb(3, 
				3, unchecked((int)(0xde))), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xffff)), NB.DecodeUInt16(B(unchecked(
				(int)(0xff)), unchecked((int)(0xff))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xffff)), NB.DecodeUInt16(Padb(3, 
				unchecked((int)(0xff)), unchecked((int)(0xff))), 3));
		}

		public virtual void TestDecodeInt32()
		{
			NUnit.Framework.Assert.AreEqual(0, NB.DecodeInt32(B(0, 0, 0, 0), 0));
			NUnit.Framework.Assert.AreEqual(0, NB.DecodeInt32(Padb(3, 0, 0, 0, 0), 3));
			NUnit.Framework.Assert.AreEqual(3, NB.DecodeInt32(B(0, 0, 0, 3), 0));
			NUnit.Framework.Assert.AreEqual(3, NB.DecodeInt32(Padb(3, 0, 0, 0, 3), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xdeadbeef)), NB.DecodeInt32(B(unchecked(
				(int)(0xde)), unchecked((int)(0xad)), unchecked((int)(0xbe)), unchecked((int)(0xef
				))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xdeadbeef)), NB.DecodeInt32(Padb
				(3, unchecked((int)(0xde)), unchecked((int)(0xad)), unchecked((int)(0xbe)), unchecked(
				(int)(0xef))), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x0310adef)), NB.DecodeInt32(B(unchecked(
				(int)(0x03)), unchecked((int)(0x10)), unchecked((int)(0xad)), unchecked((int)(0xef
				))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x0310adef)), NB.DecodeInt32(Padb
				(3, unchecked((int)(0x03)), unchecked((int)(0x10)), unchecked((int)(0xad)), unchecked(
				(int)(0xef))), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xffffffff)), NB.DecodeInt32(B(unchecked(
				(int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff
				))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xffffffff)), NB.DecodeInt32(Padb
				(3, unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked(
				(int)(0xff))), 3));
		}

		public virtual void TestDecodeUInt32()
		{
			NUnit.Framework.Assert.AreEqual(0L, NB.DecodeUInt32(B(0, 0, 0, 0), 0));
			NUnit.Framework.Assert.AreEqual(0L, NB.DecodeUInt32(Padb(3, 0, 0, 0, 0), 3));
			NUnit.Framework.Assert.AreEqual(3L, NB.DecodeUInt32(B(0, 0, 0, 3), 0));
			NUnit.Framework.Assert.AreEqual(3L, NB.DecodeUInt32(Padb(3, 0, 0, 0, 3), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0xdeadbeefL)), NB.DecodeUInt32(B
				(unchecked((int)(0xde)), unchecked((int)(0xad)), unchecked((int)(0xbe)), unchecked(
				(int)(0xef))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0xdeadbeefL)), NB.DecodeUInt32(Padb
				(3, unchecked((int)(0xde)), unchecked((int)(0xad)), unchecked((int)(0xbe)), unchecked(
				(int)(0xef))), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x0310adefL)), NB.DecodeUInt32(B
				(unchecked((int)(0x03)), unchecked((int)(0x10)), unchecked((int)(0xad)), unchecked(
				(int)(0xef))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x0310adefL)), NB.DecodeUInt32(Padb
				(3, unchecked((int)(0x03)), unchecked((int)(0x10)), unchecked((int)(0xad)), unchecked(
				(int)(0xef))), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0xffffffffL)), NB.DecodeUInt32(B
				(unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked(
				(int)(0xff))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0xffffffffL)), NB.DecodeUInt32(Padb
				(3, unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked(
				(int)(0xff))), 3));
		}

		public virtual void TestDecodeUInt64()
		{
			NUnit.Framework.Assert.AreEqual(0L, NB.DecodeUInt64(B(0, 0, 0, 0, 0, 0, 0, 0), 0)
				);
			NUnit.Framework.Assert.AreEqual(0L, NB.DecodeUInt64(Padb(3, 0, 0, 0, 0, 0, 0, 0, 
				0), 3));
			NUnit.Framework.Assert.AreEqual(3L, NB.DecodeUInt64(B(0, 0, 0, 0, 0, 0, 0, 3), 0)
				);
			NUnit.Framework.Assert.AreEqual(3L, NB.DecodeUInt64(Padb(3, 0, 0, 0, 0, 0, 0, 0, 
				3), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0xdeadbeefL)), NB.DecodeUInt64(B
				(0, 0, 0, 0, unchecked((int)(0xde)), unchecked((int)(0xad)), unchecked((int)(0xbe
				)), unchecked((int)(0xef))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0xdeadbeefL)), NB.DecodeUInt64(Padb
				(3, 0, 0, 0, 0, unchecked((int)(0xde)), unchecked((int)(0xad)), unchecked((int)(
				0xbe)), unchecked((int)(0xef))), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x0310adefL)), NB.DecodeUInt64(B
				(0, 0, 0, 0, unchecked((int)(0x03)), unchecked((int)(0x10)), unchecked((int)(0xad
				)), unchecked((int)(0xef))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x0310adefL)), NB.DecodeUInt64(Padb
				(3, 0, 0, 0, 0, unchecked((int)(0x03)), unchecked((int)(0x10)), unchecked((int)(
				0xad)), unchecked((int)(0xef))), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0xc0ffee78deadbeefL)), NB.DecodeUInt64
				(B(unchecked((int)(0xc0)), unchecked((int)(0xff)), unchecked((int)(0xee)), unchecked(
				(int)(0x78)), unchecked((int)(0xde)), unchecked((int)(0xad)), unchecked((int)(0xbe
				)), unchecked((int)(0xef))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0xc0ffee78deadbeefL)), NB.DecodeUInt64
				(Padb(3, unchecked((int)(0xc0)), unchecked((int)(0xff)), unchecked((int)(0xee)), 
				unchecked((int)(0x78)), unchecked((int)(0xde)), unchecked((int)(0xad)), unchecked(
				(int)(0xbe)), unchecked((int)(0xef))), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x00000000ffffffffL)), NB.DecodeUInt64
				(B(0, 0, 0, 0, unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff
				)), unchecked((int)(0xff))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0x00000000ffffffffL)), NB.DecodeUInt64
				(Padb(3, 0, 0, 0, 0, unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((
				int)(0xff)), unchecked((int)(0xff))), 3));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0xffffffffffffffffL)), NB.DecodeUInt64
				(B(unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked(
				(int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff
				)), unchecked((int)(0xff))), 0));
			NUnit.Framework.Assert.AreEqual(unchecked((long)(0xffffffffffffffffL)), NB.DecodeUInt64
				(Padb(3, unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff)), 
				unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked(
				(int)(0xff)), unchecked((int)(0xff))), 3));
		}

		public virtual void TestEncodeInt16()
		{
			byte[] @out = new byte[16];
			PrepareOutput(@out);
			NB.EncodeInt16(@out, 0, 0);
			AssertOutput(B(0, 0), @out, 0);
			PrepareOutput(@out);
			NB.EncodeInt16(@out, 3, 0);
			AssertOutput(B(0, 0), @out, 3);
			PrepareOutput(@out);
			NB.EncodeInt16(@out, 0, 3);
			AssertOutput(B(0, 3), @out, 0);
			PrepareOutput(@out);
			NB.EncodeInt16(@out, 3, 3);
			AssertOutput(B(0, 3), @out, 3);
			PrepareOutput(@out);
			NB.EncodeInt16(@out, 0, unchecked((int)(0xdeac)));
			AssertOutput(B(unchecked((int)(0xde)), unchecked((int)(0xac))), @out, 0);
			PrepareOutput(@out);
			NB.EncodeInt16(@out, 3, unchecked((int)(0xdeac)));
			AssertOutput(B(unchecked((int)(0xde)), unchecked((int)(0xac))), @out, 3);
			PrepareOutput(@out);
			NB.EncodeInt16(@out, 3, -1);
			AssertOutput(B(unchecked((int)(0xff)), unchecked((int)(0xff))), @out, 3);
		}

		public virtual void TestEncodeInt32()
		{
			byte[] @out = new byte[16];
			PrepareOutput(@out);
			NB.EncodeInt32(@out, 0, 0);
			AssertOutput(B(0, 0, 0, 0), @out, 0);
			PrepareOutput(@out);
			NB.EncodeInt32(@out, 3, 0);
			AssertOutput(B(0, 0, 0, 0), @out, 3);
			PrepareOutput(@out);
			NB.EncodeInt32(@out, 0, 3);
			AssertOutput(B(0, 0, 0, 3), @out, 0);
			PrepareOutput(@out);
			NB.EncodeInt32(@out, 3, 3);
			AssertOutput(B(0, 0, 0, 3), @out, 3);
			PrepareOutput(@out);
			NB.EncodeInt32(@out, 0, unchecked((int)(0xdeac)));
			AssertOutput(B(0, 0, unchecked((int)(0xde)), unchecked((int)(0xac))), @out, 0);
			PrepareOutput(@out);
			NB.EncodeInt32(@out, 3, unchecked((int)(0xdeac)));
			AssertOutput(B(0, 0, unchecked((int)(0xde)), unchecked((int)(0xac))), @out, 3);
			PrepareOutput(@out);
			NB.EncodeInt32(@out, 0, unchecked((int)(0xdeac9853)));
			AssertOutput(B(unchecked((int)(0xde)), unchecked((int)(0xac)), unchecked((int)(0x98
				)), unchecked((int)(0x53))), @out, 0);
			PrepareOutput(@out);
			NB.EncodeInt32(@out, 3, unchecked((int)(0xdeac9853)));
			AssertOutput(B(unchecked((int)(0xde)), unchecked((int)(0xac)), unchecked((int)(0x98
				)), unchecked((int)(0x53))), @out, 3);
			PrepareOutput(@out);
			NB.EncodeInt32(@out, 3, -1);
			AssertOutput(B(unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff
				)), unchecked((int)(0xff))), @out, 3);
		}

		public virtual void TestEncodeInt64()
		{
			byte[] @out = new byte[16];
			PrepareOutput(@out);
			NB.EncodeInt64(@out, 0, 0L);
			AssertOutput(B(0, 0, 0, 0, 0, 0, 0, 0), @out, 0);
			PrepareOutput(@out);
			NB.EncodeInt64(@out, 3, 0L);
			AssertOutput(B(0, 0, 0, 0, 0, 0, 0, 0), @out, 3);
			PrepareOutput(@out);
			NB.EncodeInt64(@out, 0, 3L);
			AssertOutput(B(0, 0, 0, 0, 0, 0, 0, 3), @out, 0);
			PrepareOutput(@out);
			NB.EncodeInt64(@out, 3, 3L);
			AssertOutput(B(0, 0, 0, 0, 0, 0, 0, 3), @out, 3);
			PrepareOutput(@out);
			NB.EncodeInt64(@out, 0, unchecked((long)(0xdeacL)));
			AssertOutput(B(0, 0, 0, 0, 0, 0, unchecked((int)(0xde)), unchecked((int)(0xac))), 
				@out, 0);
			PrepareOutput(@out);
			NB.EncodeInt64(@out, 3, unchecked((long)(0xdeacL)));
			AssertOutput(B(0, 0, 0, 0, 0, 0, unchecked((int)(0xde)), unchecked((int)(0xac))), 
				@out, 3);
			PrepareOutput(@out);
			NB.EncodeInt64(@out, 0, unchecked((long)(0xdeac9853L)));
			AssertOutput(B(0, 0, 0, 0, unchecked((int)(0xde)), unchecked((int)(0xac)), unchecked(
				(int)(0x98)), unchecked((int)(0x53))), @out, 0);
			PrepareOutput(@out);
			NB.EncodeInt64(@out, 3, unchecked((long)(0xdeac9853L)));
			AssertOutput(B(0, 0, 0, 0, unchecked((int)(0xde)), unchecked((int)(0xac)), unchecked(
				(int)(0x98)), unchecked((int)(0x53))), @out, 3);
			PrepareOutput(@out);
			NB.EncodeInt64(@out, 0, unchecked((long)(0xac431242deac9853L)));
			AssertOutput(B(unchecked((int)(0xac)), unchecked((int)(0x43)), unchecked((int)(0x12
				)), unchecked((int)(0x42)), unchecked((int)(0xde)), unchecked((int)(0xac)), unchecked(
				(int)(0x98)), unchecked((int)(0x53))), @out, 0);
			PrepareOutput(@out);
			NB.EncodeInt64(@out, 3, unchecked((long)(0xac431242deac9853L)));
			AssertOutput(B(unchecked((int)(0xac)), unchecked((int)(0x43)), unchecked((int)(0x12
				)), unchecked((int)(0x42)), unchecked((int)(0xde)), unchecked((int)(0xac)), unchecked(
				(int)(0x98)), unchecked((int)(0x53))), @out, 3);
			PrepareOutput(@out);
			NB.EncodeInt64(@out, 3, -1L);
			AssertOutput(B(unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff
				)), unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked(
				(int)(0xff)), unchecked((int)(0xff))), @out, 3);
		}

		private static void PrepareOutput(byte[] buf)
		{
			for (int i = 0; i < buf.Length; i++)
			{
				buf[i] = unchecked((byte)(unchecked((int)(0x77)) + i));
			}
		}

		private static void AssertOutput(byte[] expect, byte[] buf, int offset)
		{
			for (int i = 0; i < offset; i++)
			{
				NUnit.Framework.Assert.AreEqual(unchecked((byte)(unchecked((int)(0x77)) + i)), buf
					[i]);
			}
			for (int i_1 = 0; i_1 < expect.Length; i_1++)
			{
				NUnit.Framework.Assert.AreEqual(expect[i_1], buf[offset + i_1]);
			}
			for (int i_2 = offset + expect.Length; i_2 < buf.Length; i_2++)
			{
				NUnit.Framework.Assert.AreEqual(unchecked((byte)(unchecked((int)(0x77)) + i_2)), 
					buf[i_2]);
			}
		}

		private static byte[] B(int a, int b)
		{
			return new byte[] { unchecked((byte)a), unchecked((byte)b) };
		}

		private static byte[] Padb(int len, int a, int b)
		{
			byte[] r = new byte[len + 2];
			for (int i = 0; i < len; i++)
			{
				r[i] = unchecked((byte)unchecked((int)(0xaf)));
			}
			r[len] = unchecked((byte)a);
			r[len + 1] = unchecked((byte)b);
			return r;
		}

		private static byte[] B(int a, int b, int c, int d)
		{
			return new byte[] { unchecked((byte)a), unchecked((byte)b), unchecked((byte)c), unchecked(
				(byte)d) };
		}

		private static byte[] Padb(int len, int a, int b, int c, int d)
		{
			byte[] r = new byte[len + 4];
			for (int i = 0; i < len; i++)
			{
				r[i] = unchecked((byte)unchecked((int)(0xaf)));
			}
			r[len] = unchecked((byte)a);
			r[len + 1] = unchecked((byte)b);
			r[len + 2] = unchecked((byte)c);
			r[len + 3] = unchecked((byte)d);
			return r;
		}

		private static byte[] B(int a, int b, int c, int d, int e, int f, int g, int h)
		{
			return new byte[] { unchecked((byte)a), unchecked((byte)b), unchecked((byte)c), unchecked(
				(byte)d), unchecked((byte)e), unchecked((byte)f), unchecked((byte)g), unchecked(
				(byte)h) };
		}

		private static byte[] Padb(int len, int a, int b, int c, int d, int e, int f, int
			 g, int h)
		{
			byte[] r = new byte[len + 8];
			for (int i = 0; i < len; i++)
			{
				r[i] = unchecked((byte)unchecked((int)(0xaf)));
			}
			r[len] = unchecked((byte)a);
			r[len + 1] = unchecked((byte)b);
			r[len + 2] = unchecked((byte)c);
			r[len + 3] = unchecked((byte)d);
			r[len + 4] = unchecked((byte)e);
			r[len + 5] = unchecked((byte)f);
			r[len + 6] = unchecked((byte)g);
			r[len + 7] = unchecked((byte)h);
			return r;
		}
	}
}
