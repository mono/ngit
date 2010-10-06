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
using NGit.Util.IO;
using Sharpen;

namespace NGit.Util.IO
{
	[NUnit.Framework.TestFixture]
	public class EolCanonicalizingInputStreamTest
	{
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLF()
		{
			byte[] bytes = AsBytes("1\n2\n3");
			Test(bytes, bytes);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCR()
		{
			byte[] bytes = AsBytes("1\r2\r3");
			Test(bytes, bytes);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCRLF()
		{
			Test(AsBytes("1\r\n2\r\n3"), AsBytes("1\n2\n3"));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLFCR()
		{
			byte[] bytes = AsBytes("1\n\r2\n\r3");
			Test(bytes, bytes);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Test(byte[] input, byte[] expected)
		{
			InputStream bis1 = new ByteArrayInputStream(input);
			InputStream cis1 = new EolCanonicalizingInputStream(bis1);
			int index1 = 0;
			for (int b = cis1.Read(); b != -1; b = cis1.Read())
			{
				NUnit.Framework.Assert.AreEqual(expected[index1], unchecked((byte)b));
				index1++;
			}
			NUnit.Framework.Assert.AreEqual(expected.Length, index1);
			for (int bufferSize = 1; bufferSize < 10; bufferSize++)
			{
				byte[] buffer = new byte[bufferSize];
				InputStream bis2 = new ByteArrayInputStream(input);
				InputStream cis2 = new EolCanonicalizingInputStream(bis2);
				int read = 0;
				for (int readNow = cis2.Read(buffer, 0, buffer.Length); readNow != -1 && read < expected
					.Length; readNow = cis2.Read(buffer, 0, buffer.Length))
				{
					for (int index2 = 0; index2 < readNow; index2++)
					{
						NUnit.Framework.Assert.AreEqual(expected[read + index2], buffer[index2]);
					}
					read += readNow;
				}
				NUnit.Framework.Assert.AreEqual(expected.Length, read);
			}
		}

		private static byte[] AsBytes(string @in)
		{
			try
			{
				return Sharpen.Runtime.GetBytesForString(@in, "UTF-8");
			}
			catch (UnsupportedEncodingException)
			{
				throw new Exception();
			}
		}
	}
}
