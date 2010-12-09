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
using System.Text;
using NGit;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public class RevCommitParseTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_NoParents()
		{
			ObjectId treeId = Id("9788669ad918b6fcce64af8882fc9a81cb6aba67");
			string authorName = "A U. Thor";
			string authorEmail = "a_u_thor@example.com";
			int authorTime = 1218123387;
			string authorTimeZone = "+0700";
			string committerName = "C O. Miter";
			string committerEmail = "comiter@example.com";
			int committerTime = 1218123390;
			string committerTimeZone = "-0500";
			StringBuilder body = new StringBuilder();
			body.Append("tree ");
			body.Append(treeId.Name);
			body.Append("\n");
			body.Append("author ");
			body.Append(authorName);
			body.Append(" <");
			body.Append(authorEmail);
			body.Append("> ");
			body.Append(authorTime);
			body.Append(" ");
			body.Append(authorTimeZone);
			body.Append(" \n");
			body.Append("committer ");
			body.Append(committerName);
			body.Append(" <");
			body.Append(committerEmail);
			body.Append("> ");
			body.Append(committerTime);
			body.Append(" ");
			body.Append(committerTimeZone);
			body.Append("\n");
			body.Append("\n");
			RevWalk rw = new RevWalk(db);
			RevCommit c;
			c = new RevCommit(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			NUnit.Framework.Assert.IsNull(c.Tree);
			NUnit.Framework.Assert.IsNull(c.parents);
			c.ParseCanonical(rw, Sharpen.Runtime.GetBytesForString(body.ToString(), "UTF-8"));
			NUnit.Framework.Assert.IsNotNull(c.Tree);
			AssertEquals(treeId, c.Tree.Id);
			NUnit.Framework.Assert.AreSame(rw.LookupTree(treeId), c.Tree);
			NUnit.Framework.Assert.IsNotNull(c.parents);
			NUnit.Framework.Assert.AreEqual(0, c.parents.Length);
			NUnit.Framework.Assert.AreEqual(string.Empty, c.GetFullMessage());
			PersonIdent cAuthor = c.GetAuthorIdent();
			NUnit.Framework.Assert.IsNotNull(cAuthor);
			NUnit.Framework.Assert.AreEqual(authorName, cAuthor.GetName());
			NUnit.Framework.Assert.AreEqual(authorEmail, cAuthor.GetEmailAddress());
			NUnit.Framework.Assert.AreEqual((long)authorTime * 1000, cAuthor.GetWhen().GetTime
				());
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.GetTimeZone("GMT" + authorTimeZone
				), cAuthor.GetTimeZone());
			PersonIdent cCommitter = c.GetCommitterIdent();
			NUnit.Framework.Assert.IsNotNull(cCommitter);
			NUnit.Framework.Assert.AreEqual(committerName, cCommitter.GetName());
			NUnit.Framework.Assert.AreEqual(committerEmail, cCommitter.GetEmailAddress());
			NUnit.Framework.Assert.AreEqual((long)committerTime * 1000, cCommitter.GetWhen().
				GetTime());
			NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.GetTimeZone("GMT" + committerTimeZone
				), cCommitter.GetTimeZone());
		}

		/// <exception cref="System.Exception"></exception>
		private RevCommit Create(string msg)
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree 9788669ad918b6fcce64af8882fc9a81cb6aba67\n");
			b.Append("author A U. Thor <a_u_thor@example.com> 1218123387 +0700\n");
			b.Append("committer C O. Miter <c@example.com> 1218123390 -0500\n");
			b.Append("\n");
			b.Append(msg);
			RevCommit c;
			c = new RevCommit(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			c.ParseCanonical(new RevWalk(db), Sharpen.Runtime.GetBytesForString(b.ToString(), 
				"UTF-8"));
			return c;
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_WeirdHeaderOnlyCommit()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree 9788669ad918b6fcce64af8882fc9a81cb6aba67\n");
			b.Append("author A U. Thor <a_u_thor@example.com> 1218123387 +0700\n");
			b.Append("committer C O. Miter <c@example.com> 1218123390 -0500\n");
			RevCommit c;
			c = new RevCommit(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			c.ParseCanonical(new RevWalk(db), Sharpen.Runtime.GetBytesForString(b.ToString(), 
				"UTF-8"));
			NUnit.Framework.Assert.AreEqual(string.Empty, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(string.Empty, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_implicit_UTF8_encoded()
		{
			ByteArrayOutputStream b = new ByteArrayOutputStream();
			b.Write(Sharpen.Runtime.GetBytesForString("tree 9788669ad918b6fcce64af8882fc9a81cb6aba67\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("author F\u00f6r fattare <a_u_thor@example.com> 1218123387 +0700\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("committer C O. Miter <c@example.com> 1218123390 -0500\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("Sm\u00f6rg\u00e5sbord\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\u304d\u308c\u3044\n", "UTF-8"));
			RevCommit c;
			c = new RevCommit(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			// bogus id
			c.ParseCanonical(new RevWalk(db), b.ToByteArray());
			NUnit.Framework.Assert.AreSame(Constants.CHARSET, c.Encoding);
			NUnit.Framework.Assert.AreEqual("F\u00f6r fattare", c.GetAuthorIdent().GetName());
			NUnit.Framework.Assert.AreEqual("Sm\u00f6rg\u00e5sbord", c.GetShortMessage());
			NUnit.Framework.Assert.AreEqual("Sm\u00f6rg\u00e5sbord\n\n\u304d\u308c\u3044\n", 
				c.GetFullMessage());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_implicit_mixed_encoded()
		{
			ByteArrayOutputStream b = new ByteArrayOutputStream();
			b.Write(Sharpen.Runtime.GetBytesForString("tree 9788669ad918b6fcce64af8882fc9a81cb6aba67\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("author F\u00f6r fattare <a_u_thor@example.com> 1218123387 +0700\n"
				, "ISO-8859-1"));
			b.Write(Sharpen.Runtime.GetBytesForString("committer C O. Miter <c@example.com> 1218123390 -0500\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("Sm\u00f6rg\u00e5sbord\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\u304d\u308c\u3044\n", "UTF-8"));
			RevCommit c;
			c = new RevCommit(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			// bogus id
			c.ParseCanonical(new RevWalk(db), b.ToByteArray());
			NUnit.Framework.Assert.AreSame(Constants.CHARSET, c.Encoding);
			NUnit.Framework.Assert.AreEqual("F\u00f6r fattare", c.GetAuthorIdent().GetName());
			NUnit.Framework.Assert.AreEqual("Sm\u00f6rg\u00e5sbord", c.GetShortMessage());
			NUnit.Framework.Assert.AreEqual("Sm\u00f6rg\u00e5sbord\n\n\u304d\u308c\u3044\n", 
				c.GetFullMessage());
		}

		/// <summary>Test parsing of a commit whose encoding is given and works.</summary>
		/// <remarks>Test parsing of a commit whose encoding is given and works.</remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_explicit_encoded()
		{
			ByteArrayOutputStream b = new ByteArrayOutputStream();
			b.Write(Sharpen.Runtime.GetBytesForString("tree 9788669ad918b6fcce64af8882fc9a81cb6aba67\n"
				, "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("author F\u00f6r fattare <a_u_thor@example.com> 1218123387 +0700\n"
				, "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("committer C O. Miter <c@example.com> 1218123390 -0500\n"
				, "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("encoding euc-JP\n", "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("\u304d\u308c\u3044\n", "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("Hi\n", "EUC-JP"));
			RevCommit c;
			c = new RevCommit(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			// bogus id
			c.ParseCanonical(new RevWalk(db), b.ToByteArray());
			NUnit.Framework.Assert.AreEqual("Japanese (EUC)", c.Encoding.EncodingName);
			NUnit.Framework.Assert.AreEqual("F\u00f6r fattare", c.GetAuthorIdent().GetName());
			NUnit.Framework.Assert.AreEqual("\u304d\u308c\u3044", c.GetShortMessage());
			NUnit.Framework.Assert.AreEqual("\u304d\u308c\u3044\n\nHi\n", c.GetFullMessage());
		}

		/// <summary>This is a twisted case, but show what we expect here.</summary>
		/// <remarks>
		/// This is a twisted case, but show what we expect here. We can revise the
		/// expectations provided this case is updated.
		/// What happens here is that an encoding us given, but data is not encoded
		/// that way (and we can detect it), so we try other encodings.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_explicit_bad_encoded()
		{
			ByteArrayOutputStream b = new ByteArrayOutputStream();
			b.Write(Sharpen.Runtime.GetBytesForString("tree 9788669ad918b6fcce64af8882fc9a81cb6aba67\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("author F\u00f6r fattare <a_u_thor@example.com> 1218123387 +0700\n"
				, "ISO-8859-1"));
			b.Write(Sharpen.Runtime.GetBytesForString("committer C O. Miter <c@example.com> 1218123390 -0500\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("encoding EUC-JP\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\u304d\u308c\u3044\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("Hi\n", "UTF-8"));
			RevCommit c;
			c = new RevCommit(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			// bogus id
			c.ParseCanonical(new RevWalk(db), b.ToByteArray());
			NUnit.Framework.Assert.AreEqual("Japanese (EUC)", c.Encoding.EncodingName);
			NUnit.Framework.Assert.AreEqual("F\u00f6r fattare", c.GetAuthorIdent().GetName());
			NUnit.Framework.Assert.AreEqual("\u304d\u308c\u3044", c.GetShortMessage());
			NUnit.Framework.Assert.AreEqual("\u304d\u308c\u3044\n\nHi\n", c.GetFullMessage());
		}

		/// <summary>This is a twisted case too, but show what we expect here.</summary>
		/// <remarks>
		/// This is a twisted case too, but show what we expect here. We can revise the
		/// expectations provided this case is updated.
		/// What happens here is that an encoding us given, but data is not encoded
		/// that way (and we can detect it), so we try other encodings. Here data could
		/// actually be decoded in the stated encoding, but we override using UTF-8.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_explicit_bad_encoded2()
		{
			ByteArrayOutputStream b = new ByteArrayOutputStream();
			b.Write(Sharpen.Runtime.GetBytesForString("tree 9788669ad918b6fcce64af8882fc9a81cb6aba67\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("author F\u00f6r fattare <a_u_thor@example.com> 1218123387 +0700\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("committer C O. Miter <c@example.com> 1218123390 -0500\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("encoding ISO-8859-1\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\u304d\u308c\u3044\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("Hi\n", "UTF-8"));
			RevCommit c;
			c = new RevCommit(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			// bogus id
			c.ParseCanonical(new RevWalk(db), b.ToByteArray());
			NUnit.Framework.Assert.AreEqual("ISO-8859-1", c.Encoding.EncodingName);
			NUnit.Framework.Assert.AreEqual("F\u00f6r fattare", c.GetAuthorIdent().GetName());
			NUnit.Framework.Assert.AreEqual("\u304d\u308c\u3044", c.GetShortMessage());
			NUnit.Framework.Assert.AreEqual("\u304d\u308c\u3044\n\nHi\n", c.GetFullMessage());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_NoMessage()
		{
			string msg = string.Empty;
			RevCommit c = Create(msg);
			NUnit.Framework.Assert.AreEqual(msg, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(msg, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_OnlyLFMessage()
		{
			RevCommit c = Create("\n");
			NUnit.Framework.Assert.AreEqual("\n", c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(string.Empty, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_ShortLineOnlyNoLF()
		{
			string shortMsg = "This is a short message.";
			RevCommit c = Create(shortMsg);
			NUnit.Framework.Assert.AreEqual(shortMsg, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(shortMsg, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_ShortLineOnlyEndLF()
		{
			string shortMsg = "This is a short message.";
			string fullMsg = shortMsg + "\n";
			RevCommit c = Create(fullMsg);
			NUnit.Framework.Assert.AreEqual(fullMsg, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(shortMsg, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_ShortLineOnlyEmbeddedLF()
		{
			string fullMsg = "This is a\nshort message.";
			string shortMsg = fullMsg.Replace('\n', ' ');
			RevCommit c = Create(fullMsg);
			NUnit.Framework.Assert.AreEqual(fullMsg, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(shortMsg, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_ShortLineOnlyEmbeddedAndEndingLF()
		{
			string fullMsg = "This is a\nshort message.\n";
			string shortMsg = "This is a short message.";
			RevCommit c = Create(fullMsg);
			NUnit.Framework.Assert.AreEqual(fullMsg, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(shortMsg, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_GitStyleMessage()
		{
			string shortMsg = "This fixes a bug.";
			string body = "We do it with magic and pixie dust and stuff.\n" + "\n" + "Signed-off-by: A U. Thor <author@example.com>\n";
			string fullMsg = shortMsg + "\n" + "\n" + body;
			RevCommit c = Create(fullMsg);
			NUnit.Framework.Assert.AreEqual(fullMsg, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(shortMsg, c.GetShortMessage());
		}

		/// <exception cref="Sharpen.UnsupportedEncodingException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestParse_PublicParseMethod()
		{
			ObjectInserter.Formatter fmt = new ObjectInserter.Formatter();
			NGit.CommitBuilder src = new NGit.CommitBuilder();
			src.TreeId = fmt.IdFor(Constants.OBJ_TREE, new byte[] {  });
			src.Author = author;
			src.Committer = committer;
			src.Message = "Test commit\n\nThis is a test.\n";
			RevCommit p = RevCommit.Parse(src.Build());
			AssertEquals(src.TreeId, p.Tree);
			NUnit.Framework.Assert.AreEqual(0, p.ParentCount);
			NUnit.Framework.Assert.AreEqual(author, p.GetAuthorIdent());
			NUnit.Framework.Assert.AreEqual(committer, p.GetCommitterIdent());
			NUnit.Framework.Assert.AreEqual("Test commit", p.GetShortMessage());
			NUnit.Framework.Assert.AreEqual(src.Message, p.GetFullMessage());
		}

		private static ObjectId Id(string str)
		{
			return ObjectId.FromString(str);
		}
	}
}
