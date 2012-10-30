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

using System.Text;
using NGit.Util.IO;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util.IO
{
	[NUnit.Framework.TestFixture]
	public class AutoCRLFInputStreamTest
	{
		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void Test()
		{
			AssertNoCrLf(string.Empty, string.Empty);
			AssertNoCrLf("\r", "\r");
			AssertNoCrLf("\r\n", "\n");
			AssertNoCrLf("\r\n", "\r\n");
			AssertNoCrLf("\r\r", "\r\r");
			AssertNoCrLf("\r\n\r", "\n\r");
			AssertNoCrLf("\r\n\r\r", "\r\n\r\r");
			AssertNoCrLf("\r\n\r\n", "\r\n\r\n");
			AssertNoCrLf("\r\n\r\n\r", "\n\r\n\r");
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertNoCrLf(string @string, string string2)
		{
			AssertNoCrLfHelper(@string, string2);
			// \u00e5 = LATIN SMALL LETTER A WITH RING ABOVE
			// the byte value is negative
			AssertNoCrLfHelper("\u00e5" + @string, "\u00e5" + string2);
			AssertNoCrLfHelper("\u00e5" + @string + "\u00e5", "\u00e5" + string2 + "\u00e5");
			AssertNoCrLfHelper(@string + "\u00e5", string2 + "\u00e5");
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertNoCrLfHelper(string expect, string input)
		{
			byte[] inbytes = Sharpen.Runtime.GetBytesForString(input);
			byte[] expectBytes = Sharpen.Runtime.GetBytesForString(expect);
			for (int i = 0; i < 5; ++i)
			{
				byte[] buf = new byte[i];
				ByteArrayInputStream bis = new ByteArrayInputStream(inbytes);
				InputStream @in = new AutoCRLFInputStream(bis, true);
				ByteArrayOutputStream @out = new ByteArrayOutputStream();
				if (i > 0)
				{
					int n;
					while ((n = @in.Read(buf)) >= 0)
					{
						@out.Write(buf, 0, n);
					}
				}
				else
				{
					int c;
					while ((c = @in.Read()) != -1)
					{
						@out.Write(c);
					}
				}
				@out.Flush();
				@in.Close();
				@out.Close();
				byte[] actualBytes = @out.ToByteArray();
				NUnit.Framework.Assert.AreEqual(Encode(expectBytes), Encode(actualBytes), "bufsize="
					 + i);
			}
		}

		internal virtual string Encode(byte[] @in)
		{
			StringBuilder str = new StringBuilder();
			foreach (byte b in @in)
			{
				if (((sbyte)b) < 32)
				{
					str.Append(unchecked((int)(0xFF)) & b);
				}
				else
				{
					str.Append("'");
					str.Append((char)b);
					str.Append("'");
				}
				str.Append(' ');
			}
			return str.ToString();
		}
	}
}
