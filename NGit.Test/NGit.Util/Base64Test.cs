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
using NGit.Junit;
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	[NUnit.Framework.TestFixture]
	public class Base64Test
	{
		[NUnit.Framework.Test]
		public virtual void TestEncode()
		{
			NUnit.Framework.Assert.AreEqual("aGkK", Base64.EncodeBytes(B("hi\n")));
			NUnit.Framework.Assert.AreEqual("AAECDQoJcQ==", Base64.EncodeBytes(B("\x0\x1\x2\r\n\tq"
				)));
		}

		[NUnit.Framework.Test]
		public virtual void TestDecode()
		{
			JGitTestUtil.AssertEquals(B("hi\n"), Base64.DecodeBytes("aGkK"));
			JGitTestUtil.AssertEquals(B("\x0\x1\x2\r\n\tq"), Base64.DecodeBytes("AAECDQoJcQ=="));
			JGitTestUtil.AssertEquals(B("\x0\x1\x2\r\n\tq"), Base64.DecodeBytes("A A E\tC D\rQ o\nJ c Q=="
				));
			JGitTestUtil.AssertEquals(B("\u000EB"), Base64.DecodeBytes("DkL="));
		}

		[NUnit.Framework.Test]
		public virtual void TestDecodeFail_NonBase64Character()
		{
			try
			{
				Base64.DecodeBytes("! a bad base64 string !");
				NUnit.Framework.Assert.Fail("Accepted bad string in decode");
			}
			catch (ArgumentException)
			{
			}
		}

		// Expected
		[NUnit.Framework.Test]
		public virtual void TestEncodeMatchesDecode()
		{
			string[] testStrings = new string[] { string.Empty, "cow", "a", "a secret string"
				, "\x0\x1\x2\r\n\t" };
			//
			//
			//
			//
			//
			foreach (string e in testStrings)
			{
				JGitTestUtil.AssertEquals(B(e), Base64.DecodeBytes(Base64.EncodeBytes(B(e))));
			}
		}

		private static byte[] B(string str)
		{
			return Constants.Encode(str);
		}
	}
}
