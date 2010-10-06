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
using NGit.Diff;
using NUnit.Framework;
using Sharpen;

namespace NGit.Diff
{
	public class RawTextIgnoreLeadingWhitespaceTest : TestCase
	{
		private readonly RawTextComparator cmp = RawTextComparator.WS_IGNORE_LEADING;

		public virtual void TestEqualsWithoutWhitespace()
		{
			RawText a = new RawText(cmp, Constants.EncodeASCII("foo-a\nfoo-b\nfoo\n"));
			RawText b = new RawText(cmp, Constants.EncodeASCII("foo-b\nfoo-c\nf\n"));
			NUnit.Framework.Assert.AreEqual(3, a.Size());
			NUnit.Framework.Assert.AreEqual(3, b.Size());
			// foo-a != foo-b
			NUnit.Framework.Assert.IsFalse(cmp.Equals(a, 0, b, 0));
			NUnit.Framework.Assert.IsFalse(cmp.Equals(b, 0, a, 0));
			// foo-b == foo-b
			NUnit.Framework.Assert.IsTrue(cmp.Equals(a, 1, b, 0));
			NUnit.Framework.Assert.IsTrue(cmp.Equals(b, 0, a, 1));
			// foo != f
			NUnit.Framework.Assert.IsFalse(cmp.Equals(a, 2, b, 2));
			NUnit.Framework.Assert.IsFalse(cmp.Equals(b, 2, a, 2));
		}

		public virtual void TestEqualsWithWhitespace()
		{
			RawText a = new RawText(cmp, Constants.EncodeASCII("foo-a\n         \n a b c\n      a\nb    \n"
				));
			RawText b = new RawText(cmp, Constants.EncodeASCII("foo-a        b\n\nab  c\na\nb\n"
				));
			// "foo-a" != "foo-a        b"
			NUnit.Framework.Assert.IsFalse(cmp.Equals(a, 0, b, 0));
			NUnit.Framework.Assert.IsFalse(cmp.Equals(b, 0, a, 0));
			// "         " == ""
			NUnit.Framework.Assert.IsTrue(cmp.Equals(a, 1, b, 1));
			NUnit.Framework.Assert.IsTrue(cmp.Equals(b, 1, a, 1));
			// " a b c" != "ab  c"
			NUnit.Framework.Assert.IsFalse(cmp.Equals(a, 2, b, 2));
			NUnit.Framework.Assert.IsFalse(cmp.Equals(b, 2, a, 2));
			// "      a" == "a"
			NUnit.Framework.Assert.IsTrue(cmp.Equals(a, 3, b, 3));
			NUnit.Framework.Assert.IsTrue(cmp.Equals(b, 3, a, 3));
			// "b    " != "b"
			NUnit.Framework.Assert.IsFalse(cmp.Equals(a, 4, b, 4));
			NUnit.Framework.Assert.IsFalse(cmp.Equals(b, 4, a, 4));
		}
	}
}
