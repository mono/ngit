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

using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	[NUnit.Framework.TestFixture]
	public class StringUtilsTest
	{
		[NUnit.Framework.Test]
		public virtual void TestToLowerCaseChar()
		{
			NUnit.Framework.Assert.AreEqual('a', StringUtils.ToLowerCase('A'));
			NUnit.Framework.Assert.AreEqual('z', StringUtils.ToLowerCase('Z'));
			NUnit.Framework.Assert.AreEqual('a', StringUtils.ToLowerCase('a'));
			NUnit.Framework.Assert.AreEqual('z', StringUtils.ToLowerCase('z'));
			NUnit.Framework.Assert.AreEqual((char)0, StringUtils.ToLowerCase((char)0));
			NUnit.Framework.Assert.AreEqual((char)unchecked((int)(0xffff)), StringUtils.ToLowerCase
				((char)unchecked((int)(0xffff))));
		}

		[NUnit.Framework.Test]
		public virtual void TestToLowerCaseString()
		{
			NUnit.Framework.Assert.AreEqual("\n abcdefghijklmnopqrstuvwxyz\n", StringUtils.ToLowerCase
				("\n ABCDEFGHIJKLMNOPQRSTUVWXYZ\n"));
		}

		[NUnit.Framework.Test]
		public virtual void TestEqualsIgnoreCase1()
		{
			string a = "FOO";
			NUnit.Framework.Assert.IsTrue(StringUtils.EqualsIgnoreCase(a, a));
		}

		[NUnit.Framework.Test]
		public virtual void TestEqualsIgnoreCase2()
		{
			NUnit.Framework.Assert.IsFalse(StringUtils.EqualsIgnoreCase("a", string.Empty));
		}

		[NUnit.Framework.Test]
		public virtual void TestEqualsIgnoreCase3()
		{
			NUnit.Framework.Assert.IsFalse(StringUtils.EqualsIgnoreCase("a", "b"));
			NUnit.Framework.Assert.IsFalse(StringUtils.EqualsIgnoreCase("ac", "ab"));
		}

		[NUnit.Framework.Test]
		public virtual void TestEqualsIgnoreCase4()
		{
			NUnit.Framework.Assert.IsTrue(StringUtils.EqualsIgnoreCase("a", "a"));
			NUnit.Framework.Assert.IsTrue(StringUtils.EqualsIgnoreCase("A", "a"));
			NUnit.Framework.Assert.IsTrue(StringUtils.EqualsIgnoreCase("a", "A"));
		}
	}
}
