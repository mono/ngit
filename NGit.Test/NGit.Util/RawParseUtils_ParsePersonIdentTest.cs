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
using Sharpen;

namespace NGit.Util
{
	[NUnit.Framework.TestFixture]
	public class RawParseUtils_ParsePersonIdentTest
	{
		[NUnit.Framework.Test]
		public virtual void TestParsePersonIdent_legalCases()
		{
			DateTime when = Sharpen.Extensions.CreateDate(1234567890000l);
			TimeZoneInfo tz = Sharpen.Extensions.GetTimeZone("GMT-7");
			AssertPersonIdent("Me <me@example.com> 1234567890 -0700", new PersonIdent("Me", "me@example.com"
				, when, tz));
			AssertPersonIdent(" Me <me@example.com> 1234567890 -0700", new PersonIdent(" Me", 
				"me@example.com", when, tz));
			AssertPersonIdent("A U Thor <author@example.com> 1234567890 -0700", new PersonIdent
				("A U Thor", "author@example.com", when, tz));
			AssertPersonIdent("A U Thor<author@example.com> 1234567890 -0700", new PersonIdent
				("A U Thor", "author@example.com", when, tz));
			AssertPersonIdent("A U Thor<author@example.com>1234567890 -0700", new PersonIdent
				("A U Thor", "author@example.com", when, tz));
			AssertPersonIdent(" A U Thor   < author@example.com > 1234567890 -0700", new PersonIdent
				(" A U Thor  ", " author@example.com ", when, tz));
			AssertPersonIdent("A U Thor<author@example.com>1234567890 -0700", new PersonIdent
				("A U Thor", "author@example.com", when, tz));
		}

		[NUnit.Framework.Test]
		public virtual void TestParsePersonIdent_fuzzyCases()
		{
			DateTime when = Sharpen.Extensions.CreateDate(1234567890000l);
			TimeZoneInfo tz = Sharpen.Extensions.GetTimeZone("GMT-7");
			AssertPersonIdent("A U Thor <author@example.com>,  C O. Miter <comiter@example.com> 1234567890 -0700"
				, new PersonIdent("A U Thor", "author@example.com", when, tz));
			AssertPersonIdent("A U Thor <author@example.com> and others 1234567890 -0700", new 
				PersonIdent("A U Thor", "author@example.com", when, tz));
		}

		[NUnit.Framework.Test]
		public virtual void TestParsePersonIdent_incompleteCases()
		{
			DateTime when = Sharpen.Extensions.CreateDate(1234567890000l);
			TimeZoneInfo tz = Sharpen.Extensions.GetTimeZone("GMT-7");
			AssertPersonIdent("Me <> 1234567890 -0700", new PersonIdent("Me", string.Empty, when
				, tz));
			AssertPersonIdent(" <me@example.com> 1234567890 -0700", new PersonIdent(string.Empty
				, "me@example.com", when, tz));
			AssertPersonIdent(" <> 1234567890 -0700", new PersonIdent(string.Empty, string.Empty
				, when, tz));
			AssertPersonIdent("<>", new PersonIdent(string.Empty, string.Empty, 0, 0));
			AssertPersonIdent(" <>", new PersonIdent(string.Empty, string.Empty, 0, 0));
			AssertPersonIdent("<me@example.com>", new PersonIdent(string.Empty, "me@example.com"
				, 0, 0));
			AssertPersonIdent(" <me@example.com>", new PersonIdent(string.Empty, "me@example.com"
				, 0, 0));
			AssertPersonIdent("Me <>", new PersonIdent("Me", string.Empty, 0, 0));
			AssertPersonIdent("Me <me@example.com>", new PersonIdent("Me", "me@example.com", 
				0, 0));
			AssertPersonIdent("Me <me@example.com> 1234567890", new PersonIdent("Me", "me@example.com"
				, 0, 0));
			AssertPersonIdent("Me <me@example.com> 1234567890 ", new PersonIdent("Me", "me@example.com"
				, 0, 0));
		}

		[NUnit.Framework.Test]
		public virtual void TestParsePersonIdent_malformedCases()
		{
			AssertPersonIdent("Me me@example.com> 1234567890 -0700", null);
			AssertPersonIdent("Me <me@example.com 1234567890 -0700", null);
		}

		private void AssertPersonIdent(string line, PersonIdent expected)
		{
			PersonIdent actual = RawParseUtils.ParsePersonIdent(line);
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}
	}
}
