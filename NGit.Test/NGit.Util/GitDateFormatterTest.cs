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
using NGit.Junit;
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	[NUnit.Framework.TestFixture]
	public class GitDateFormatterTest
	{
		private MockSystemReader mockSystemReader;

		private PersonIdent ident;

		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			mockSystemReader = new _MockSystemReader_62();
			SystemReader.SetInstance(mockSystemReader);
			ident = RawParseUtils.ParsePersonIdent("A U Thor <author@example.com> 1316560165 -0400"
				);
		}

		private sealed class _MockSystemReader_62 : MockSystemReader
		{
			public _MockSystemReader_62()
			{
			}

			public override long GetCurrentTime()
			{
				return 1318125997291L;
			}
		}

		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("Matching java date formatting is not critical right now")]
		public virtual void DEFAULT()
		{
			NUnit.Framework.Assert.AreEqual("Tue Sep 20 19:09:25 2011 -0400", new GitDateFormatter
				(GitDateFormatter.Format.DEFAULT).FormatDate(ident));
		}

		[NUnit.Framework.Test]
		public virtual void RELATIVE()
		{
			NUnit.Framework.Assert.AreEqual("3 weeks ago", new GitDateFormatter(GitDateFormatter.Format
				.RELATIVE).FormatDate(ident));
		}

		[NUnit.Framework.Test]
		public virtual void LOCAL()
		{
			NUnit.Framework.Assert.AreEqual("Tue Sep 20 19:39:25 2011", new GitDateFormatter(
				GitDateFormatter.Format.LOCAL).FormatDate(ident));
		}

		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("Matching java date formatting is not critical right now")]
		public virtual void ISO()
		{
			NUnit.Framework.Assert.AreEqual("2011-09-20 19:09:25 -0400", new GitDateFormatter
				(GitDateFormatter.Format.ISO).FormatDate(ident));
		}

		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("Matching java date formatting is not critical right now")]
		public virtual void RFC()
		{
			NUnit.Framework.Assert.AreEqual("Tue, 20 Sep 2011 19:09:25 -0400", new GitDateFormatter
				(GitDateFormatter.Format.RFC).FormatDate(ident));
		}

		[NUnit.Framework.Test]
		public virtual void SHORT()
		{
			NUnit.Framework.Assert.AreEqual("2011-09-20", new GitDateFormatter(GitDateFormatter.Format
				.SHORT).FormatDate(ident));
		}

		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("Matching java date formatting is not critical right now")]
		public virtual void RAW()
		{
			NUnit.Framework.Assert.AreEqual("1316560165 -0400", new GitDateFormatter(GitDateFormatter.Format
				.RAW).FormatDate(ident));
		}

		[NUnit.Framework.Test]
		[NUnit.Framework.Ignore ("Matching java date formatting is not critical right now")]
		public virtual void LOCALE()
		{
			NUnit.Framework.Assert.AreEqual("Sep 20, 2011 7:09:25 PM -0400", new GitDateFormatter
				(GitDateFormatter.Format.LOCALE).FormatDate(ident));
		}

		[NUnit.Framework.Ignore ("Matching java date formatting is not critical right now")]
		[NUnit.Framework.Test]
		public virtual void LOCALELOCAL()
		{
			NUnit.Framework.Assert.AreEqual("Sep 20, 2011 7:39:25 PM", new GitDateFormatter(GitDateFormatter.Format
				.LOCALELOCAL).FormatDate(ident));
		}
	}
}
