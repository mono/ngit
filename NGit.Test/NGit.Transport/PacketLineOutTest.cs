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
using NGit.Transport;
using NUnit.Framework;
using Sharpen;

namespace NGit.Transport
{
	[NUnit.Framework.TestFixture]
	public class PacketLineOutTest
	{
		private ByteArrayOutputStream rawOut;

		private PacketLineOut @out;

		// Note, test vectors created with:
		//
		// perl -e 'printf "%4.4x%s\n", 4+length($ARGV[0]),$ARGV[0]'
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			rawOut = new ByteArrayOutputStream();
			@out = new PacketLineOut(rawOut);
		}

		// writeString
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWriteString1()
		{
			@out.WriteString("a");
			@out.WriteString("bc");
			AssertBuffer("0005a0006bc");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWriteString2()
		{
			@out.WriteString("a\n");
			@out.WriteString("bc\n");
			AssertBuffer("0006a\n0007bc\n");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWriteString3()
		{
			@out.WriteString(string.Empty);
			AssertBuffer("0004");
		}

		// end
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWriteEnd()
		{
			int[] flushCnt = new int[1];
			OutputStream mockout = new _OutputStream_99(this, flushCnt);
			new PacketLineOut(mockout).End();
			AssertBuffer("0000");
			NUnit.Framework.Assert.AreEqual(1, flushCnt[0]);
		}

		private sealed class _OutputStream_99 : OutputStream
		{
			public _OutputStream_99(PacketLineOutTest _enclosing, int[] flushCnt)
			{
				this._enclosing = _enclosing;
				this.flushCnt = flushCnt;
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Write(int arg0)
			{
				this._enclosing.rawOut.Write(arg0);
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Flush()
			{
				flushCnt[0]++;
			}

			private readonly PacketLineOutTest _enclosing;

			private readonly int[] flushCnt;
		}

		// writePacket
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePacket1()
		{
			@out.WritePacket(new byte[] { (byte)('a') });
			AssertBuffer("0005a");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePacket2()
		{
			@out.WritePacket(new byte[] { (byte)('a'), (byte)('b'), (byte)('c'), (byte)('d') }
				);
			AssertBuffer("0008abcd");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWritePacket3()
		{
			int buflen = SideBandOutputStream.MAX_BUF - 5;
			byte[] buf = new byte[buflen];
			for (int i = 0; i < buf.Length; i++)
			{
				buf[i] = unchecked((byte)i);
			}
			@out.WritePacket(buf);
			@out.Flush();
			byte[] act = rawOut.ToByteArray();
			string explen = Sharpen.Extensions.ToString(buf.Length + 4, 16);
			NUnit.Framework.Assert.AreEqual(4 + buf.Length, act.Length);
			NUnit.Framework.Assert.AreEqual(Sharpen.Runtime.GetStringForBytes(act, 0, 4, "UTF-8"
				), explen);
			for (int i_1 = 0, j = 4; i_1 < buf.Length; i_1++, j++)
			{
				NUnit.Framework.Assert.AreEqual(buf[i_1], act[j]);
			}
		}

		// flush
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFlush()
		{
			int[] flushCnt = new int[1];
			OutputStream mockout = new _OutputStream_154(flushCnt);
			new PacketLineOut(mockout).Flush();
			NUnit.Framework.Assert.AreEqual(1, flushCnt[0]);
		}

		private sealed class _OutputStream_154 : OutputStream
		{
			public _OutputStream_154(int[] flushCnt)
			{
				this.flushCnt = flushCnt;
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Write(int arg0)
			{
				NUnit.Framework.Assert.Fail("should not write");
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void Flush()
			{
				flushCnt[0]++;
			}

			private readonly int[] flushCnt;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertBuffer(string exp)
		{
			NUnit.Framework.Assert.AreEqual(exp, Sharpen.Runtime.GetStringForBytes(rawOut.ToByteArray
				(), Constants.CHARACTER_ENCODING));
		}
	}
}
