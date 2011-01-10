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
using NGit;
using NGit.Transport;
using NUnit.Framework;
using Sharpen;

namespace NGit.Transport
{
	[NUnit.Framework.TestFixture]
	public class SideBandOutputStreamTest
	{
		private ByteArrayOutputStream rawOut;

		// Note, test vectors created with:
		//
		// perl -e 'printf "%4.4x%s\n", 4+length($ARGV[0]),$ARGV[0]'
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			rawOut = new ByteArrayOutputStream();
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWrite_CH_DATA()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_DATA, SideBandOutputStream
				.SMALL_BUF, rawOut);
			@out.Write(new byte[] { (byte)('a'), (byte)('b'), (byte)('c') });
			@out.Flush();
			AssertBuffer("0008\x1abc");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWrite_CH_PROGRESS()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_PROGRESS, SideBandOutputStream
				.SMALL_BUF, rawOut);
			@out.Write(new byte[] { (byte)('a'), (byte)('b'), (byte)('c') });
			@out.Flush();
			AssertBuffer("0008\x2abc");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWrite_CH_ERROR()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_ERROR, SideBandOutputStream
				.SMALL_BUF, rawOut);
			@out.Write(new byte[] { (byte)('a'), (byte)('b'), (byte)('c') });
			@out.Flush();
			AssertBuffer("0008\x3abc");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWrite_Small()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_DATA, SideBandOutputStream
				.SMALL_BUF, rawOut);
			@out.Write('a');
			@out.Write('b');
			@out.Write('c');
			@out.Flush();
			AssertBuffer("0008\x1abc");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWrite_SmallBlocks1()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_DATA, 6, rawOut);
			@out.Write('a');
			@out.Write('b');
			@out.Write('c');
			@out.Flush();
			AssertBuffer("0006\x1a0006\x1b0006\x1c");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWrite_SmallBlocks2()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_DATA, 6, rawOut);
			@out.Write(new byte[] { (byte)('a'), (byte)('b'), (byte)('c') });
			@out.Flush();
			AssertBuffer("0006\x1a0006\x1b0006\x1c");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWrite_SmallBlocks3()
		{
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_DATA, 7, rawOut);
			@out.Write('a');
			@out.Write(new byte[] { (byte)('b'), (byte)('c') });
			@out.Flush();
			AssertBuffer("0007\x1ab0006\x1c");
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWrite_Large()
		{
			int buflen = SideBandOutputStream.MAX_BUF - SideBandOutputStream.HDR_SIZE;
			byte[] buf = new byte[buflen];
			for (int i = 0; i < buf.Length; i++)
			{
				buf[i] = unchecked((byte)i);
			}
			SideBandOutputStream @out;
			@out = new SideBandOutputStream(SideBandOutputStream.CH_DATA, SideBandOutputStream
				.MAX_BUF, rawOut);
			@out.Write(buf);
			@out.Flush();
			byte[] act = rawOut.ToByteArray();
			string explen = Sharpen.Extensions.ToString(buf.Length + SideBandOutputStream.HDR_SIZE
				, 16);
			NUnit.Framework.Assert.AreEqual(SideBandOutputStream.HDR_SIZE + buf.Length, act.Length
				);
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.CreateString(act, 0, 4, "UTF-8"
				), explen);
			NUnit.Framework.Assert.AreEqual(1, act[4]);
			for (int i_1 = 0; i_1 < buf.Length; i_1++, j++)
			{
				NUnit.Framework.Assert.AreEqual(buf[i_1], act[j]);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestFlush()
		{
			int[] flushCnt = new int[1];
			OutputStream mockout = new _OutputStream_171(flushCnt);
			new SideBandOutputStream(SideBandOutputStream.CH_DATA, SideBandOutputStream.SMALL_BUF
				, mockout).Flush();
			NUnit.Framework.Assert.AreEqual(1, flushCnt[0]);
		}

		private sealed class _OutputStream_171 : OutputStream
		{
			public _OutputStream_171(int[] flushCnt)
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

		[NUnit.Framework.Test]
		public virtual void TestConstructor_RejectsBadChannel()
		{
			try
			{
				new SideBandOutputStream(-1, SideBandOutputStream.MAX_BUF, rawOut);
				NUnit.Framework.Assert.Fail("Accepted -1 channel number");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("channel -1 must be in range [0, 255]", e.Message
					);
			}
			try
			{
				new SideBandOutputStream(0, SideBandOutputStream.MAX_BUF, rawOut);
				NUnit.Framework.Assert.Fail("Accepted 0 channel number");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("channel 0 must be in range [0, 255]", e.Message);
			}
			try
			{
				new SideBandOutputStream(256, SideBandOutputStream.MAX_BUF, rawOut);
				NUnit.Framework.Assert.Fail("Accepted 256 channel number");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("channel 256 must be in range [0, 255]", e.Message
					);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestConstructor_RejectsBadBufferSize()
		{
			try
			{
				new SideBandOutputStream(SideBandOutputStream.CH_DATA, -1, rawOut);
				NUnit.Framework.Assert.Fail("Accepted -1 for buffer size");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("packet size -1 must be >= 5", e.Message);
			}
			try
			{
				new SideBandOutputStream(SideBandOutputStream.CH_DATA, 0, rawOut);
				NUnit.Framework.Assert.Fail("Accepted 0 for buffer size");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("packet size 0 must be >= 5", e.Message);
			}
			try
			{
				new SideBandOutputStream(SideBandOutputStream.CH_DATA, 1, rawOut);
				NUnit.Framework.Assert.Fail("Accepted 1 for buffer size");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual("packet size 1 must be >= 5", e.Message);
			}
			try
			{
				new SideBandOutputStream(SideBandOutputStream.CH_DATA, int.MaxValue, rawOut);
				NUnit.Framework.Assert.Fail("Accepted " + int.MaxValue + " for buffer size");
			}
			catch (ArgumentException e)
			{
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().packetSizeMustBeAtMost
					, int.MaxValue, 65520), e.Message);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertBuffer(string exp)
		{
			NUnit.Framework.Assert.AreEqual(exp, Sharpen.Extensions.CreateString(rawOut.ToByteArray
				(), Constants.CHARACTER_ENCODING));
		}
	}
}
