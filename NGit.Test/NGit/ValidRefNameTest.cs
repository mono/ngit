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
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class ValidRefNameTest
	{
		private static void AssertValid(bool exp, string name)
		{
			NUnit.Framework.Assert.AreEqual(exp, Repository.IsValidRefName(name), "\"" + name
				 + "\"");
		}

		[NUnit.Framework.Test]
		public virtual void TestEmptyString()
		{
			AssertValid(false, string.Empty);
			AssertValid(false, "/");
		}

		[NUnit.Framework.Test]
		public virtual void TestMustHaveTwoComponents()
		{
			AssertValid(false, "master");
			AssertValid(true, "heads/master");
		}

		[NUnit.Framework.Test]
		public virtual void TestValidHead()
		{
			AssertValid(true, "refs/heads/master");
			AssertValid(true, "refs/heads/pu");
			AssertValid(true, "refs/heads/z");
			AssertValid(true, "refs/heads/FoO");
		}

		[NUnit.Framework.Test]
		public virtual void TestValidTag()
		{
			AssertValid(true, "refs/tags/v1.0");
		}

		[NUnit.Framework.Test]
		public virtual void TestNoLockSuffix()
		{
			AssertValid(false, "refs/heads/master.lock");
		}

		[NUnit.Framework.Test]
		public virtual void TestNoDirectorySuffix()
		{
			AssertValid(false, "refs/heads/master/");
		}

		[NUnit.Framework.Test]
		public virtual void TestNoSpace()
		{
			AssertValid(false, "refs/heads/i haz space");
		}

		[NUnit.Framework.Test]
		public virtual void TestNoAsciiControlCharacters()
		{
			for (char c = '\0'; c < ' '; c++)
			{
				AssertValid(false, "refs/heads/mast" + c + "er");
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestNoBareDot()
		{
			AssertValid(false, "refs/heads/.");
			AssertValid(false, "refs/heads/..");
			AssertValid(false, "refs/heads/./master");
			AssertValid(false, "refs/heads/../master");
		}

		[NUnit.Framework.Test]
		public virtual void TestNoLeadingOrTrailingDot()
		{
			AssertValid(false, ".");
			AssertValid(false, "refs/heads/.bar");
			AssertValid(false, "refs/heads/..bar");
			AssertValid(false, "refs/heads/bar.");
		}

		[NUnit.Framework.Test]
		public virtual void TestContainsDot()
		{
			AssertValid(true, "refs/heads/m.a.s.t.e.r");
			AssertValid(false, "refs/heads/master..pu");
		}

		[NUnit.Framework.Test]
		public virtual void TestNoMagicRefCharacters()
		{
			AssertValid(false, "refs/heads/master^");
			AssertValid(false, "refs/heads/^master");
			AssertValid(false, "^refs/heads/master");
			AssertValid(false, "refs/heads/master~");
			AssertValid(false, "refs/heads/~master");
			AssertValid(false, "~refs/heads/master");
			AssertValid(false, "refs/heads/master:");
			AssertValid(false, "refs/heads/:master");
			AssertValid(false, ":refs/heads/master");
		}

		[NUnit.Framework.Test]
		public virtual void TestShellGlob()
		{
			AssertValid(false, "refs/heads/master?");
			AssertValid(false, "refs/heads/?master");
			AssertValid(false, "?refs/heads/master");
			AssertValid(false, "refs/heads/master[");
			AssertValid(false, "refs/heads/[master");
			AssertValid(false, "[refs/heads/master");
			AssertValid(false, "refs/heads/master*");
			AssertValid(false, "refs/heads/*master");
			AssertValid(false, "*refs/heads/master");
		}

		[NUnit.Framework.Test]
		public virtual void TestValidSpecialCharacters()
		{
			AssertValid(true, "refs/heads/!");
			AssertValid(true, "refs/heads/\"");
			AssertValid(true, "refs/heads/#");
			AssertValid(true, "refs/heads/$");
			AssertValid(true, "refs/heads/%");
			AssertValid(true, "refs/heads/&");
			AssertValid(true, "refs/heads/'");
			AssertValid(true, "refs/heads/(");
			AssertValid(true, "refs/heads/)");
			AssertValid(true, "refs/heads/+");
			AssertValid(true, "refs/heads/,");
			AssertValid(true, "refs/heads/-");
			AssertValid(true, "refs/heads/;");
			AssertValid(true, "refs/heads/<");
			AssertValid(true, "refs/heads/=");
			AssertValid(true, "refs/heads/>");
			AssertValid(true, "refs/heads/@");
			AssertValid(true, "refs/heads/]");
			AssertValid(true, "refs/heads/_");
			AssertValid(true, "refs/heads/`");
			AssertValid(true, "refs/heads/{");
			AssertValid(true, "refs/heads/|");
			AssertValid(true, "refs/heads/}");
			// This is valid on UNIX, but not on Windows
			// hence we make in invalid due to non-portability
			//
			AssertValid(false, "refs/heads/\\");
		}

		[NUnit.Framework.Test]
		public virtual void TestUnicodeNames()
		{
			AssertValid(true, "refs/heads/\u00e5ngstr\u00f6m");
		}

		[NUnit.Framework.Test]
		public virtual void TestRefLogQueryIsValidRef()
		{
			AssertValid(false, "refs/heads/master@{1}");
			AssertValid(false, "refs/heads/master@{1.hour.ago}");
		}
	}
}
