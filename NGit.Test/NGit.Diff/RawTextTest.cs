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
using NGit;
using NGit.Diff;
using NGit.Util;
using Sharpen;

namespace NGit.Diff
{
	[NUnit.Framework.TestFixture]
	public class RawTextTest
	{
		[NUnit.Framework.Test]
		public virtual void TestEmpty()
		{
			RawText r = new RawText(new byte[0]);
			NUnit.Framework.Assert.AreEqual(0, r.Size());
		}

		[NUnit.Framework.Test]
		public virtual void TestEquals()
		{
			RawText a = new RawText(Constants.EncodeASCII("foo-a\nfoo-b\n"));
			RawText b = new RawText(Constants.EncodeASCII("foo-b\nfoo-c\n"));
			RawTextComparator cmp = RawTextComparator.DEFAULT;
			NUnit.Framework.Assert.AreEqual(2, a.Size());
			NUnit.Framework.Assert.AreEqual(2, b.Size());
			// foo-a != foo-b
			NUnit.Framework.Assert.IsFalse(cmp.Equals(a, 0, b, 0));
			NUnit.Framework.Assert.IsFalse(cmp.Equals(b, 0, a, 0));
			// foo-b == foo-b
			NUnit.Framework.Assert.IsTrue(cmp.Equals(a, 1, b, 0));
			NUnit.Framework.Assert.IsTrue(cmp.Equals(b, 0, a, 1));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWriteLine1()
		{
			RawText a = new RawText(Constants.EncodeASCII("foo-a\nfoo-b\n"));
			ByteArrayOutputStream o = new ByteArrayOutputStream();
			a.WriteLine(o, 0);
			byte[] r = o.ToByteArray();
			NUnit.Framework.Assert.AreEqual("foo-a", RawParseUtils.Decode(r));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWriteLine2()
		{
			RawText a = new RawText(Constants.EncodeASCII("foo-a\nfoo-b"));
			ByteArrayOutputStream o = new ByteArrayOutputStream();
			a.WriteLine(o, 1);
			byte[] r = o.ToByteArray();
			NUnit.Framework.Assert.AreEqual("foo-b", RawParseUtils.Decode(r));
		}

		/// <exception cref="System.IO.IOException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestWriteLine3()
		{
			RawText a = new RawText(Constants.EncodeASCII("a\n\nb\n"));
			ByteArrayOutputStream o = new ByteArrayOutputStream();
			a.WriteLine(o, 1);
			byte[] r = o.ToByteArray();
			NUnit.Framework.Assert.AreEqual(string.Empty, RawParseUtils.Decode(r));
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestComparatorReduceCommonStartEnd()
		{
			RawTextComparator c = RawTextComparator.DEFAULT;
			Edit e;
			e = c.ReduceCommonStartEnd(T(string.Empty), T(string.Empty), new Edit(0, 0, 0, 0)
				);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 0, 0, 0), e);
			e = c.ReduceCommonStartEnd(T("a"), T("b"), new Edit(0, 1, 0, 1));
			NUnit.Framework.Assert.AreEqual(new Edit(0, 1, 0, 1), e);
			e = c.ReduceCommonStartEnd(T("a"), T("a"), new Edit(0, 1, 0, 1));
			NUnit.Framework.Assert.AreEqual(new Edit(1, 1, 1, 1), e);
			e = c.ReduceCommonStartEnd(T("axB"), T("axC"), new Edit(0, 3, 0, 3));
			NUnit.Framework.Assert.AreEqual(new Edit(2, 3, 2, 3), e);
			e = c.ReduceCommonStartEnd(T("Bxy"), T("Cxy"), new Edit(0, 3, 0, 3));
			NUnit.Framework.Assert.AreEqual(new Edit(0, 1, 0, 1), e);
			e = c.ReduceCommonStartEnd(T("bc"), T("Abc"), new Edit(0, 2, 0, 3));
			NUnit.Framework.Assert.AreEqual(new Edit(0, 0, 0, 1), e);
			e = new Edit(0, 5, 0, 5);
			e = c.ReduceCommonStartEnd(T("abQxy"), T("abRxy"), e);
			NUnit.Framework.Assert.AreEqual(new Edit(2, 3, 2, 3), e);
			RawText a = new RawText(Sharpen.Runtime.GetBytesForString("p\na b\nQ\nc d\n", "UTF-8"
				));
			RawText b = new RawText(Sharpen.Runtime.GetBytesForString("p\na  b \nR\n c  d \n"
				, "UTF-8"));
			e = new Edit(0, 4, 0, 4);
			e = RawTextComparator.WS_IGNORE_ALL.ReduceCommonStartEnd(a, b, e);
			NUnit.Framework.Assert.AreEqual(new Edit(2, 3, 2, 3), e);
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestComparatorReduceCommonStartEnd_EmptyLine()
		{
			RawText a;
			RawText b;
			Edit e;
			a = new RawText(Sharpen.Runtime.GetBytesForString("R\n y\n", "UTF-8"));
			b = new RawText(Sharpen.Runtime.GetBytesForString("S\n\n y\n", "UTF-8"));
			e = new Edit(0, 2, 0, 3);
			e = RawTextComparator.DEFAULT.ReduceCommonStartEnd(a, b, e);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 1, 0, 2), e);
			a = new RawText(Sharpen.Runtime.GetBytesForString("S\n\n y\n", "UTF-8"));
			b = new RawText(Sharpen.Runtime.GetBytesForString("R\n y\n", "UTF-8"));
			e = new Edit(0, 3, 0, 2);
			e = RawTextComparator.DEFAULT.ReduceCommonStartEnd(a, b, e);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 2, 0, 1), e);
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestComparatorReduceCommonStartButLastLineNoEol()
		{
			RawText a;
			RawText b;
			Edit e;
			a = new RawText(Sharpen.Runtime.GetBytesForString("start", "UTF-8"));
			b = new RawText(Sharpen.Runtime.GetBytesForString("start of line", "UTF-8"));
			e = new Edit(0, 1, 0, 1);
			e = RawTextComparator.DEFAULT.ReduceCommonStartEnd(a, b, e);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 1, 0, 1), e);
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestComparatorReduceCommonStartButLastLineNoEol_2()
		{
			RawText a;
			RawText b;
			Edit e;
			a = new RawText(Sharpen.Runtime.GetBytesForString("start", "UTF-8"));
			b = new RawText(Sharpen.Runtime.GetBytesForString("start of\nlastline", "UTF-8"));
			e = new Edit(0, 1, 0, 2);
			e = RawTextComparator.DEFAULT.ReduceCommonStartEnd(a, b, e);
			NUnit.Framework.Assert.AreEqual(new Edit(0, 1, 0, 2), e);
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestLineDelimiter()
		{
			RawText rt = new RawText(Constants.EncodeASCII("foo\n"));
			NUnit.Framework.Assert.AreEqual("\n", rt.GetLineDelimiter());
			NUnit.Framework.Assert.IsFalse(rt.IsMissingNewlineAtEnd());
			rt = new RawText(Constants.EncodeASCII("foo\r\n"));
			NUnit.Framework.Assert.AreEqual("\r\n", rt.GetLineDelimiter());
			NUnit.Framework.Assert.IsFalse(rt.IsMissingNewlineAtEnd());
			rt = new RawText(Constants.EncodeASCII("foo\nbar"));
			NUnit.Framework.Assert.AreEqual("\n", rt.GetLineDelimiter());
			NUnit.Framework.Assert.IsTrue(rt.IsMissingNewlineAtEnd());
			rt = new RawText(Constants.EncodeASCII("foo\r\nbar"));
			NUnit.Framework.Assert.AreEqual("\r\n", rt.GetLineDelimiter());
			NUnit.Framework.Assert.IsTrue(rt.IsMissingNewlineAtEnd());
			rt = new RawText(Constants.EncodeASCII("foo\nbar\r\n"));
			NUnit.Framework.Assert.AreEqual("\n", rt.GetLineDelimiter());
			NUnit.Framework.Assert.IsFalse(rt.IsMissingNewlineAtEnd());
			rt = new RawText(Constants.EncodeASCII("foo\r\nbar\n"));
			NUnit.Framework.Assert.AreEqual("\r\n", rt.GetLineDelimiter());
			NUnit.Framework.Assert.IsFalse(rt.IsMissingNewlineAtEnd());
			rt = new RawText(Constants.EncodeASCII("foo"));
			NUnit.Framework.Assert.IsNull(rt.GetLineDelimiter());
			NUnit.Framework.Assert.IsTrue(rt.IsMissingNewlineAtEnd());
			rt = new RawText(Constants.EncodeASCII(string.Empty));
			NUnit.Framework.Assert.IsNull(rt.GetLineDelimiter());
			NUnit.Framework.Assert.IsTrue(rt.IsMissingNewlineAtEnd());
			rt = new RawText(Constants.EncodeASCII("\n"));
			NUnit.Framework.Assert.AreEqual("\n", rt.GetLineDelimiter());
			NUnit.Framework.Assert.IsFalse(rt.IsMissingNewlineAtEnd());
			rt = new RawText(Constants.EncodeASCII("\r\n"));
			NUnit.Framework.Assert.AreEqual("\r\n", rt.GetLineDelimiter());
			NUnit.Framework.Assert.IsFalse(rt.IsMissingNewlineAtEnd());
		}

		private static RawText T(string text)
		{
			StringBuilder r = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				r.Append(text[i]);
				r.Append('\n');
			}
			try
			{
				return new RawText(Sharpen.Runtime.GetBytesForString(r.ToString(), "UTF-8"));
			}
			catch (UnsupportedEncodingException e)
			{
				throw new RuntimeException(e);
			}
		}
	}
}
