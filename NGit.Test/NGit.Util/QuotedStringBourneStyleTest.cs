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
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class QuotedStringBourneStyleTest : TestCase
	{
		private static void AssertQuote(string @in, string exp)
		{
			string r = QuotedString.BOURNE.Quote(@in);
			NUnit.Framework.Assert.AreNotSame(@in, r);
			NUnit.Framework.Assert.IsFalse(@in.Equals(r));
			NUnit.Framework.Assert.AreEqual('\'' + exp + '\'', r);
		}

		private static void AssertDequote(string exp, string @in)
		{
			byte[] b = Constants.Encode('\'' + @in + '\'');
			string r = QuotedString.BOURNE.Dequote(b, 0, b.Length);
			NUnit.Framework.Assert.AreEqual(exp, r);
		}

		public virtual void TestQuote_Empty()
		{
			NUnit.Framework.Assert.AreEqual("''", QuotedString.BOURNE.Quote(string.Empty));
		}

		public virtual void TestDequote_Empty1()
		{
			NUnit.Framework.Assert.AreEqual(string.Empty, QuotedString.BOURNE.Dequote(new byte
				[0], 0, 0));
		}

		public virtual void TestDequote_Empty2()
		{
			NUnit.Framework.Assert.AreEqual(string.Empty, QuotedString.BOURNE.Dequote(new byte
				[] { (byte)('\''), (byte)('\'') }, 0, 2));
		}

		public virtual void TestDequote_SoleSq()
		{
			NUnit.Framework.Assert.AreEqual(string.Empty, QuotedString.BOURNE.Dequote(new byte
				[] { (byte)('\'') }, 0, 1));
		}

		public virtual void TestQuote_BareA()
		{
			AssertQuote("a", "a");
		}

		public virtual void TestDequote_BareA()
		{
			string @in = "a";
			byte[] b = Constants.Encode(@in);
			NUnit.Framework.Assert.AreEqual(@in, QuotedString.BOURNE.Dequote(b, 0, b.Length));
		}

		public virtual void TestDequote_BareABCZ_OnlyBC()
		{
			string @in = "abcz";
			byte[] b = Constants.Encode(@in);
			int p = @in.IndexOf('b');
			NUnit.Framework.Assert.AreEqual("bc", QuotedString.BOURNE.Dequote(b, p, p + 2));
		}

		public virtual void TestDequote_LoneBackslash()
		{
			AssertDequote("\\", "\\");
		}

		public virtual void TestQuote_NamedEscapes()
		{
			AssertQuote("'", "'\\''");
			AssertQuote("!", "'\\!'");
			AssertQuote("a'b", "a'\\''b");
			AssertQuote("a!b", "a'\\!'b");
		}

		public virtual void TestDequote_NamedEscapes()
		{
			AssertDequote("'", "'\\''");
			AssertDequote("!", "'\\!'");
			AssertDequote("a'b", "a'\\''b");
			AssertDequote("a!b", "a'\\!'b");
		}
	}
}
