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

using System.Collections.Generic;
using System.Text;
using NGit;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public class FooterLineTest : RepositoryTestCase
	{
		[NUnit.Framework.Test]
		public virtual void TestNoFooters_EmptyBody()
		{
			RevCommit commit = Parse(string.Empty);
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		[NUnit.Framework.Test]
		public virtual void TestNoFooters_NewlineOnlyBody1()
		{
			RevCommit commit = Parse("\n");
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		[NUnit.Framework.Test]
		public virtual void TestNoFooters_NewlineOnlyBody5()
		{
			RevCommit commit = Parse("\n\n\n\n\n");
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		[NUnit.Framework.Test]
		public virtual void TestNoFooters_OneLineBodyNoLF()
		{
			RevCommit commit = Parse("this is a commit");
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		[NUnit.Framework.Test]
		public virtual void TestNoFooters_OneLineBodyWithLF()
		{
			RevCommit commit = Parse("this is a commit\n");
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		[NUnit.Framework.Test]
		public virtual void TestNoFooters_ShortBodyNoLF()
		{
			RevCommit commit = Parse("subject\n\nbody of commit");
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		[NUnit.Framework.Test]
		public virtual void TestNoFooters_ShortBodyWithLF()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n");
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		[NUnit.Framework.Test]
		public virtual void TestSignedOffBy_OneUserNoLF()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Signed-off-by: A. U. Thor <a@example.com>"
				);
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("A. U. Thor <a@example.com>", f.GetValue());
			NUnit.Framework.Assert.AreEqual("a@example.com", f.GetEmailAddress());
		}

		[NUnit.Framework.Test]
		public virtual void TestSignedOffBy_OneUserWithLF()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Signed-off-by: A. U. Thor <a@example.com>\n"
				);
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("A. U. Thor <a@example.com>", f.GetValue());
			NUnit.Framework.Assert.AreEqual("a@example.com", f.GetEmailAddress());
		}

		[NUnit.Framework.Test]
		public virtual void TestSignedOffBy_IgnoreWhitespace()
		{
			// We only ignore leading whitespace on the value, trailing
			// is assumed part of the value.
			//
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Signed-off-by:   A. U. Thor <a@example.com>  \n"
				);
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("A. U. Thor <a@example.com>  ", f.GetValue());
			NUnit.Framework.Assert.AreEqual("a@example.com", f.GetEmailAddress());
		}

		[NUnit.Framework.Test]
		public virtual void TestEmptyValueNoLF()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Signed-off-by:");
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual(string.Empty, f.GetValue());
			NUnit.Framework.Assert.IsNull(f.GetEmailAddress());
		}

		[NUnit.Framework.Test]
		public virtual void TestEmptyValueWithLF()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Signed-off-by:\n"
				);
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual(string.Empty, f.GetValue());
			NUnit.Framework.Assert.IsNull(f.GetEmailAddress());
		}

		[NUnit.Framework.Test]
		public virtual void TestShortKey()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "K:V\n");
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("K", f.GetKey());
			NUnit.Framework.Assert.AreEqual("V", f.GetValue());
			NUnit.Framework.Assert.IsNull(f.GetEmailAddress());
		}

		[NUnit.Framework.Test]
		public virtual void TestNonDelimtedEmail()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Acked-by: re@example.com\n"
				);
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Acked-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("re@example.com", f.GetValue());
			NUnit.Framework.Assert.AreEqual("re@example.com", f.GetEmailAddress());
		}

		[NUnit.Framework.Test]
		public virtual void TestNotEmail()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Acked-by: Main Tain Er\n"
				);
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Acked-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("Main Tain Er", f.GetValue());
			NUnit.Framework.Assert.IsNull(f.GetEmailAddress());
		}

		[NUnit.Framework.Test]
		public virtual void TestSignedOffBy_ManyUsers()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "Not-A-Footer-Line: this line must not be read as a footer\n"
				 + "\n" + "Signed-off-by: A. U. Thor <a@example.com>\n" + "CC:            <some.mailing.list@example.com>\n"
				 + "Acked-by: Some Reviewer <sr@example.com>\n" + "Signed-off-by: Main Tain Er <mte@example.com>\n"
				);
			// paragraph break, now footers appear in final block
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(4, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("A. U. Thor <a@example.com>", f.GetValue());
			NUnit.Framework.Assert.AreEqual("a@example.com", f.GetEmailAddress());
			f = footers[1];
			NUnit.Framework.Assert.AreEqual("CC", f.GetKey());
			NUnit.Framework.Assert.AreEqual("<some.mailing.list@example.com>", f.GetValue());
			NUnit.Framework.Assert.AreEqual("some.mailing.list@example.com", f.GetEmailAddress
				());
			f = footers[2];
			NUnit.Framework.Assert.AreEqual("Acked-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("Some Reviewer <sr@example.com>", f.GetValue());
			NUnit.Framework.Assert.AreEqual("sr@example.com", f.GetEmailAddress());
			f = footers[3];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("Main Tain Er <mte@example.com>", f.GetValue());
			NUnit.Framework.Assert.AreEqual("mte@example.com", f.GetEmailAddress());
		}

		[NUnit.Framework.Test]
		public virtual void TestSignedOffBy_SkipNonFooter()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "Not-A-Footer-Line: this line must not be read as a footer\n"
				 + "\n" + "Signed-off-by: A. U. Thor <a@example.com>\n" + "CC:            <some.mailing.list@example.com>\n"
				 + "not really a footer line but we'll skip it anyway\n" + "Acked-by: Some Reviewer <sr@example.com>\n"
				 + "Signed-off-by: Main Tain Er <mte@example.com>\n");
			// paragraph break, now footers appear in final block
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(4, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("A. U. Thor <a@example.com>", f.GetValue());
			f = footers[1];
			NUnit.Framework.Assert.AreEqual("CC", f.GetKey());
			NUnit.Framework.Assert.AreEqual("<some.mailing.list@example.com>", f.GetValue());
			f = footers[2];
			NUnit.Framework.Assert.AreEqual("Acked-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("Some Reviewer <sr@example.com>", f.GetValue());
			f = footers[3];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("Main Tain Er <mte@example.com>", f.GetValue());
		}

		[NUnit.Framework.Test]
		public virtual void TestFilterFootersIgnoreCase()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "Not-A-Footer-Line: this line must not be read as a footer\n"
				 + "\n" + "Signed-Off-By: A. U. Thor <a@example.com>\n" + "CC:            <some.mailing.list@example.com>\n"
				 + "Acked-by: Some Reviewer <sr@example.com>\n" + "signed-off-by: Main Tain Er <mte@example.com>\n"
				);
			// paragraph break, now footers appear in final block
			IList<string> footers = commit.GetFooterLines("signed-off-by");
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(2, footers.Count);
			NUnit.Framework.Assert.AreEqual("A. U. Thor <a@example.com>", footers[0]);
			NUnit.Framework.Assert.AreEqual("Main Tain Er <mte@example.com>", footers[1]);
		}

		[NUnit.Framework.Test]
		public virtual void TestMatchesBugId()
		{
			RevCommit commit = Parse("this is a commit subject for test\n" + "\n" + "Simple-Bug-Id: 42\n"
				);
			// paragraph break, now footers appear in final block
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			FooterLine line = footers[0];
			NUnit.Framework.Assert.IsNotNull(line);
			NUnit.Framework.Assert.AreEqual("Simple-Bug-Id", line.GetKey());
			NUnit.Framework.Assert.AreEqual("42", line.GetValue());
			FooterKey bugid = new FooterKey("Simple-Bug-Id");
			NUnit.Framework.Assert.IsTrue(line.Matches(bugid), "matches Simple-Bug-Id");
			NUnit.Framework.Assert.IsFalse(line.Matches(FooterKey.SIGNED_OFF_BY), "not Signed-off-by"
				);
			NUnit.Framework.Assert.IsFalse(line.Matches(FooterKey.CC), "not CC");
		}

		private RevCommit Parse(string msg)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("tree " + ObjectId.ZeroId.Name + "\n");
			buf.Append("author A. U. Thor <a@example.com> 1 +0000\n");
			buf.Append("committer A. U. Thor <a@example.com> 1 +0000\n");
			buf.Append("\n");
			buf.Append(msg);
			RevWalk walk = new RevWalk(db);
			walk.SetRetainBody(true);
			RevCommit c = new RevCommit(ObjectId.ZeroId);
			c.ParseCanonical(walk, Constants.Encode(buf.ToString()));
			return c;
		}
	}
}
