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
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class RawParseUtils_HexParseTest : TestCase
	{
		public virtual void TestInt4_1()
		{
			NUnit.Framework.Assert.AreEqual(0, RawParseUtils.ParseHexInt4(unchecked((byte)'0'
				)));
			NUnit.Framework.Assert.AreEqual(1, RawParseUtils.ParseHexInt4(unchecked((byte)'1'
				)));
			NUnit.Framework.Assert.AreEqual(2, RawParseUtils.ParseHexInt4(unchecked((byte)'2'
				)));
			NUnit.Framework.Assert.AreEqual(3, RawParseUtils.ParseHexInt4(unchecked((byte)'3'
				)));
			NUnit.Framework.Assert.AreEqual(4, RawParseUtils.ParseHexInt4(unchecked((byte)'4'
				)));
			NUnit.Framework.Assert.AreEqual(5, RawParseUtils.ParseHexInt4(unchecked((byte)'5'
				)));
			NUnit.Framework.Assert.AreEqual(6, RawParseUtils.ParseHexInt4(unchecked((byte)'6'
				)));
			NUnit.Framework.Assert.AreEqual(7, RawParseUtils.ParseHexInt4(unchecked((byte)'7'
				)));
			NUnit.Framework.Assert.AreEqual(8, RawParseUtils.ParseHexInt4(unchecked((byte)'8'
				)));
			NUnit.Framework.Assert.AreEqual(9, RawParseUtils.ParseHexInt4(unchecked((byte)'9'
				)));
			NUnit.Framework.Assert.AreEqual(10, RawParseUtils.ParseHexInt4(unchecked((byte)'a'
				)));
			NUnit.Framework.Assert.AreEqual(11, RawParseUtils.ParseHexInt4(unchecked((byte)'b'
				)));
			NUnit.Framework.Assert.AreEqual(12, RawParseUtils.ParseHexInt4(unchecked((byte)'c'
				)));
			NUnit.Framework.Assert.AreEqual(13, RawParseUtils.ParseHexInt4(unchecked((byte)'d'
				)));
			NUnit.Framework.Assert.AreEqual(14, RawParseUtils.ParseHexInt4(unchecked((byte)'e'
				)));
			NUnit.Framework.Assert.AreEqual(15, RawParseUtils.ParseHexInt4(unchecked((byte)'f'
				)));
			NUnit.Framework.Assert.AreEqual(10, RawParseUtils.ParseHexInt4(unchecked((byte)'A'
				)));
			NUnit.Framework.Assert.AreEqual(11, RawParseUtils.ParseHexInt4(unchecked((byte)'B'
				)));
			NUnit.Framework.Assert.AreEqual(12, RawParseUtils.ParseHexInt4(unchecked((byte)'C'
				)));
			NUnit.Framework.Assert.AreEqual(13, RawParseUtils.ParseHexInt4(unchecked((byte)'D'
				)));
			NUnit.Framework.Assert.AreEqual(14, RawParseUtils.ParseHexInt4(unchecked((byte)'E'
				)));
			NUnit.Framework.Assert.AreEqual(15, RawParseUtils.ParseHexInt4(unchecked((byte)'F'
				)));
			AssertNotHex('q');
			AssertNotHex(' ');
			AssertNotHex('.');
		}

		private static void AssertNotHex(char c)
		{
			try
			{
				RawParseUtils.ParseHexInt4(unchecked((byte)c));
				NUnit.Framework.Assert.Fail("Incorrectly acccepted " + c);
			}
			catch (IndexOutOfRangeException)
			{
			}
		}

		// pass
		public virtual void TestInt16()
		{
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x0000)), Parse16("0000"));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x0001)), Parse16("0001"));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x1234)), Parse16("1234"));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xdead)), Parse16("dead"));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xBEEF)), Parse16("BEEF"));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x4321)), Parse16("4321"));
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xffff)), Parse16("ffff"));
			try
			{
				Parse16("noth");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"noth\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
			// pass
			try
			{
				Parse16("01");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"01\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
			// pass
			try
			{
				Parse16("000.");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"000.\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
		}

		// pass
		private static int Parse16(string str)
		{
			return RawParseUtils.ParseHexInt16(Constants.EncodeASCII(str), 0);
		}

		public virtual void TestInt32()
		{
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x00000000)), Parse32("00000000")
				);
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0x00000001)), Parse32("00000001")
				);
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xc0ffEE42)), Parse32("c0ffEE42")
				);
			NUnit.Framework.Assert.AreEqual(unchecked((int)(0xffffffff)), Parse32("ffffffff")
				);
			NUnit.Framework.Assert.AreEqual(-1, Parse32("ffffffff"));
			try
			{
				Parse32("noth");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"noth\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
			// pass
			try
			{
				Parse32("notahexs");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"notahexs\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
			// pass
			try
			{
				Parse32("01");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"01\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
			// pass
			try
			{
				Parse32("0000000.");
				NUnit.Framework.Assert.Fail("Incorrectly acccepted \"0000000.\"");
			}
			catch (IndexOutOfRangeException)
			{
			}
		}

		// pass
		private static int Parse32(string str)
		{
			return RawParseUtils.ParseHexInt32(Constants.EncodeASCII(str), 0);
		}
	}
}
