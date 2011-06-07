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
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class ConstantsEncodingTest
	{
		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEncodeASCII_SimpleASCII()
		{
			string src = "abc";
			byte[] exp = new byte[] { (byte)('a'), (byte)('b'), (byte)('c') };
			byte[] res = Constants.EncodeASCII(src);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(exp, res));
			NUnit.Framework.Assert.AreEqual(src, Sharpen.Runtime.GetStringForBytes(res, 0, res
				.Length, "UTF-8"));
		}

		[NUnit.Framework.Test]
		public virtual void TestEncodeASCII_FailOnNonASCII()
		{
			string src = "≈™nƒ≠c≈çdeÃΩ";
			try
			{
				Constants.EncodeASCII(src);
				NUnit.Framework.Assert.Fail("Incorrectly accepted a Unicode character");
			}
			catch (ArgumentException err)
			{
				NUnit.Framework.Assert.AreEqual("Not ASCII string: " + src, err.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestEncodeASCII_Number13()
		{
			long src = 13;
			byte[] exp = new byte[] { (byte)('1'), (byte)('3') };
			byte[] res = Constants.EncodeASCII(src);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(exp, res));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEncode_SimpleASCII()
		{
			string src = "abc";
			byte[] exp = new byte[] { (byte)('a'), (byte)('b'), (byte)('c') };
			byte[] res = Constants.Encode(src);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(exp, res));
			NUnit.Framework.Assert.AreEqual(src, Sharpen.Runtime.GetStringForBytes(res, 0, res
				.Length, "UTF-8"));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestEncode_Unicode()
		{
			string src = "≈™nƒ≠c≈çdeÃΩ";
			byte[] exp = new byte[] { unchecked((byte)unchecked((int)(0xC5))), unchecked((byte
				)unchecked((int)(0xAA))), unchecked((int)(0x6E)), unchecked((byte)unchecked((int
				)(0xC4))), unchecked((byte)unchecked((int)(0xAD))), unchecked((int)(0x63)), unchecked(
				(byte)unchecked((int)(0xC5))), unchecked((byte)unchecked((int)(0x8D))), unchecked(
				(int)(0x64)), unchecked((int)(0x65)), unchecked((byte)unchecked((int)(0xCC))), unchecked(
				(byte)unchecked((int)(0xBD))) };
			byte[] res = Constants.Encode(src);
			NUnit.Framework.Assert.IsTrue(Arrays.Equals(exp, res));
			NUnit.Framework.Assert.AreEqual(src, Sharpen.Runtime.GetStringForBytes(res, 0, res
				.Length, "UTF-8"));
		}
	}
}
