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
using Sharpen;

namespace NGit.Util
{
	[NUnit.Framework.TestFixture]
	public class QuotedStringGitPathStyleTest
	{
		private static void AssertQuote(string exp, string @in)
		{
			string r = QuotedString.GIT_PATH.Quote(@in);
			NUnit.Framework.Assert.AreNotSame(@in, r);
			NUnit.Framework.Assert.IsFalse(@in.Equals(r));
			NUnit.Framework.Assert.AreEqual('"' + exp + '"', r);
		}

		private static void AssertDequote(string exp, string @in)
		{
			byte[] b;
			try
			{
				b = Sharpen.Runtime.GetBytesForString(('"' + @in + '"'), "ISO-8859-1");
			}
			catch (UnsupportedEncodingException e)
			{
				throw new RuntimeException(e);
			}
			string r = QuotedString.GIT_PATH.Dequote(b, 0, b.Length);
			NUnit.Framework.Assert.AreEqual(exp, r);
		}

		[NUnit.Framework.Test]
		public virtual void TestQuote_Empty()
		{
			NUnit.Framework.Assert.AreEqual("\"\"", QuotedString.GIT_PATH.Quote(string.Empty)
				);
		}

		[NUnit.Framework.Test]
		public virtual void TestDequote_Empty1()
		{
			NUnit.Framework.Assert.AreEqual(string.Empty, QuotedString.GIT_PATH.Dequote(new byte
				[0], 0, 0));
		}

		[NUnit.Framework.Test]
		public virtual void TestDequote_Empty2()
		{
			NUnit.Framework.Assert.AreEqual(string.Empty, QuotedString.GIT_PATH.Dequote(new byte
				[] { (byte)('"'), (byte)('"') }, 0, 2));
		}

		[NUnit.Framework.Test]
		public virtual void TestDequote_SoleDq()
		{
			NUnit.Framework.Assert.AreEqual("\"", QuotedString.GIT_PATH.Dequote(new byte[] { 
				(byte)('"') }, 0, 1));
		}

		[NUnit.Framework.Test]
		public virtual void TestQuote_BareA()
		{
			string @in = "a";
			NUnit.Framework.Assert.AreSame(@in, QuotedString.GIT_PATH.Quote(@in));
		}

		[NUnit.Framework.Test]
		public virtual void TestDequote_BareA()
		{
			string @in = "a";
			byte[] b = Constants.Encode(@in);
			NUnit.Framework.Assert.AreEqual(@in, QuotedString.GIT_PATH.Dequote(b, 0, b.Length
				));
		}

		[NUnit.Framework.Test]
		public virtual void TestDequote_BareABCZ_OnlyBC()
		{
			string @in = "abcz";
			byte[] b = Constants.Encode(@in);
			int p = @in.IndexOf('b');
			NUnit.Framework.Assert.AreEqual("bc", QuotedString.GIT_PATH.Dequote(b, p, p + 2));
		}

		[NUnit.Framework.Test]
		public virtual void TestDequote_LoneBackslash()
		{
			AssertDequote("\\", "\\");
		}

		[NUnit.Framework.Test]
		public virtual void TestQuote_NamedEscapes()
		{
			AssertQuote("\\a", "\u0007");
			AssertQuote("\\b", "\b");
			AssertQuote("\\f", "\f");
			AssertQuote("\\n", "\n");
			AssertQuote("\\r", "\r");
			AssertQuote("\\t", "\t");
			AssertQuote("\\v", "\u000B");
			AssertQuote("\\\\", "\\");
			AssertQuote("\\\"", "\"");
		}

		[NUnit.Framework.Test]
		public virtual void TestDequote_NamedEscapes()
		{
			AssertDequote("\u0007", "\\a");
			AssertDequote("\b", "\\b");
			AssertDequote("\f", "\\f");
			AssertDequote("\n", "\\n");
			AssertDequote("\r", "\\r");
			AssertDequote("\t", "\\t");
			AssertDequote("\u000B", "\\v");
			AssertDequote("\\", "\\\\");
			AssertDequote("\"", "\\\"");
		}

		[NUnit.Framework.Test]
		public virtual void TestDequote_OctalAll()
		{
			for (int i = 0; i < 127; i++)
			{
				AssertDequote(string.Empty + (char)i, OctalEscape(i));
			}
			for (int i_1 = 128; i_1 < 256; i_1++)
			{
				int f = unchecked((int)(0xC0)) | (i_1 >> 6);
				int s = unchecked((int)(0x80)) | (i_1 & unchecked((int)(0x3f)));
				AssertDequote(string.Empty + (char)i_1, OctalEscape(f) + OctalEscape(s));
			}
		}

		private string OctalEscape(int i)
		{
			string s = Sharpen.Extensions.ToOctalString(i);
			while (s.Length < 3)
			{
				s = "0" + s;
			}
			return "\\" + s;
		}

		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("Mono does not handle this kind of string escaping")]
		public virtual void TestQuote_OctalAll()
		{
			AssertQuote("\\001", "\x1");
			AssertQuote("\\177", "\u007f");
			AssertQuote("\\303\\277", "\u00ff");
		}

		// \u00ff in UTF-8
		[NUnit.Framework.Test]
		public virtual void TestDequote_UnknownEscapeQ()
		{
			AssertDequote("\\q", "\\q");
		}

		[NUnit.Framework.Test]
		public virtual void TestDequote_FooTabBar()
		{
			AssertDequote("foo\tbar", "foo\\tbar");
		}

		[NUnit.Framework.Test]
		public virtual void TestDequote_Latin1()
		{
			AssertDequote("\u00c5ngstr\u00f6m", "\\305ngstr\\366m");
		}

		// Latin1
		[NUnit.Framework.Test]
		public virtual void TestDequote_UTF8()
		{
			AssertDequote("\u00c5ngstr\u00f6m", "\\303\\205ngstr\\303\\266m");
		}

		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("Mono does not handle this kind of string escaping")]
		public virtual void TestDequote_RawUTF8()
		{
			AssertDequote("\u00c5ngstr\u00f6m", "\x12f\xcdngstr\x12f\x10am");
		}

		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("Mono does not handle this kind of string escaping")]
		public virtual void TestDequote_RawLatin1()
		{
			AssertDequote("\u00c5ngstr\u00f6m", "\x131ngstr\x16em");
		}

		[NUnit.Framework.Test]
		public virtual void TestQuote_Ang()
		{
			AssertQuote("\\303\\205ngstr\\303\\266m", "\u00c5ngstr\u00f6m");
		}

		[NUnit.Framework.Test]
		public virtual void TestQuoteAtAndNumber()
		{
			NUnit.Framework.Assert.AreSame("abc@2x.png", QuotedString.GIT_PATH.Quote("abc@2x.png"
				));
			AssertDequote("abc@2x.png", "abc\\1002x.png");
		}
	}
}
