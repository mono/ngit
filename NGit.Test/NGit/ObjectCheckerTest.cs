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
using NGit.Errors;
using Sharpen;

namespace NGit
{
	[NUnit.Framework.TestFixture]
	public class ObjectCheckerTest
	{
		private ObjectChecker checker;

		/// <exception cref="System.Exception"></exception>
		[NUnit.Framework.SetUp]
		public virtual void SetUp()
		{
			checker = new ObjectChecker();
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidType()
		{
			try
			{
				checker.Check(Constants.OBJ_BAD, new byte[0]);
				NUnit.Framework.Assert.Fail("Did not throw CorruptObjectException");
			}
			catch (CorruptObjectException e)
			{
				string m = e.Message;
				NUnit.Framework.Assert.AreEqual(MessageFormat.Format(JGitText.Get().corruptObjectInvalidType2
					, Constants.OBJ_BAD), m);
			}
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestCheckBlob()
		{
			// Any blob should pass...
			checker.CheckBlob(new byte[0]);
			checker.CheckBlob(new byte[1]);
			checker.Check(Constants.OBJ_BLOB, new byte[0]);
			checker.Check(Constants.OBJ_BLOB, new byte[1]);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidCommitNoParent()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author A. U. Thor <author@localhost> 1 +0000\n");
			b.Append("committer A. U. Thor <author@localhost> 1 +0000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckCommit(data);
			checker.Check(Constants.OBJ_COMMIT, data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidCommitBlankAuthor()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author <> 0 +0000\n");
			b.Append("committer <> 0 +0000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckCommit(data);
			checker.Check(Constants.OBJ_COMMIT, data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidCommit1Parent()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("parent ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author A. U. Thor <author@localhost> 1 +0000\n");
			b.Append("committer A. U. Thor <author@localhost> 1 +0000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckCommit(data);
			checker.Check(Constants.OBJ_COMMIT, data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidCommit2Parent()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("parent ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("parent ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author A. U. Thor <author@localhost> 1 +0000\n");
			b.Append("committer A. U. Thor <author@localhost> 1 +0000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckCommit(data);
			checker.Check(Constants.OBJ_COMMIT, data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidCommit128Parent()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			for (int i = 0; i < 128; i++)
			{
				b.Append("parent ");
				b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
				b.Append('\n');
			}
			b.Append("author A. U. Thor <author@localhost> 1 +0000\n");
			b.Append("committer A. U. Thor <author@localhost> 1 +0000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckCommit(data);
			checker.Check(Constants.OBJ_COMMIT, data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidCommitNormalTime()
		{
			StringBuilder b = new StringBuilder();
			string when = "1222757360 -0730";
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author A. U. Thor <author@localhost> " + when + "\n");
			b.Append("committer A. U. Thor <author@localhost> " + when + "\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckCommit(data);
			checker.Check(Constants.OBJ_COMMIT, data);
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitNoTree1()
		{
			StringBuilder b = new StringBuilder();
			b.Append("parent ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no tree header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitNoTree2()
		{
			StringBuilder b = new StringBuilder();
			b.Append("trie ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no tree header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitNoTree3()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no tree header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitNoTree4()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree\t");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no tree header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidTree1()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("zzzzfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid tree", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidTree2()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append("z\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid tree", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidTree3()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9b");
			b.Append("\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid tree", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidTree4()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree  ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid tree", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidParent1()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("parent ");
			b.Append("\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid parent", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidParent2()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("parent ");
			b.Append("zzzzfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append("\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid parent", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidParent3()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("parent  ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append("\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid parent", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidParent4()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("parent  ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append("z\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid parent", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidParent5()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("parent\t");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append("\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				// Yes, really, we complain about author not being
				// found as the invalid parent line wasn't consumed.
				NUnit.Framework.Assert.AreEqual("no author", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitNoAuthor()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("committer A. U. Thor <author@localhost> 1 +0000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				// Yes, really, we complain about author not being
				// found as the invalid parent line wasn't consumed.
				NUnit.Framework.Assert.AreEqual("no author", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitNoCommitter1()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author A. U. Thor <author@localhost> 1 +0000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				// Yes, really, we complain about author not being
				// found as the invalid parent line wasn't consumed.
				NUnit.Framework.Assert.AreEqual("no committer", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitNoCommitter2()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author A. U. Thor <author@localhost> 1 +0000\n");
			b.Append("\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				// Yes, really, we complain about author not being
				// found as the invalid parent line wasn't consumed.
				NUnit.Framework.Assert.AreEqual("no committer", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidAuthor1()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author A. U. Thor <foo 1 +0000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				// Yes, really, we complain about author not being
				// found as the invalid parent line wasn't consumed.
				NUnit.Framework.Assert.AreEqual("invalid author", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidAuthor2()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author A. U. Thor foo> 1 +0000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				// Yes, really, we complain about author not being
				// found as the invalid parent line wasn't consumed.
				NUnit.Framework.Assert.AreEqual("invalid author", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidAuthor3()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author 1 +0000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				// Yes, really, we complain about author not being
				// found as the invalid parent line wasn't consumed.
				NUnit.Framework.Assert.AreEqual("invalid author", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidAuthor4()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author a <b> +0000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				// Yes, really, we complain about author not being
				// found as the invalid parent line wasn't consumed.
				NUnit.Framework.Assert.AreEqual("invalid author", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidAuthor5()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author a <b>\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				// Yes, really, we complain about author not being
				// found as the invalid parent line wasn't consumed.
				NUnit.Framework.Assert.AreEqual("invalid author", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidAuthor6()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author a <b> z");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				// Yes, really, we complain about author not being
				// found as the invalid parent line wasn't consumed.
				NUnit.Framework.Assert.AreEqual("invalid author", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidAuthor7()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author a <b> 1 z");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				// Yes, really, we complain about author not being
				// found as the invalid parent line wasn't consumed.
				NUnit.Framework.Assert.AreEqual("invalid author", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidCommitInvalidCommitter()
		{
			StringBuilder b = new StringBuilder();
			b.Append("tree ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("author a <b> 1 +0000\n");
			b.Append("committer a <");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckCommit(data);
				NUnit.Framework.Assert.Fail("Did not catch corrupt object");
			}
			catch (CorruptObjectException e)
			{
				// Yes, really, we complain about author not being
				// found as the invalid parent line wasn't consumed.
				NUnit.Framework.Assert.AreEqual("invalid committer", e.Message);
			}
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTag()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("type commit\n");
			b.Append("tag test-tag\n");
			b.Append("tagger A. U. Thor <author@localhost> 1 +0000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTag(data);
			checker.Check(Constants.OBJ_TAG, data);
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoObject1()
		{
			StringBuilder b = new StringBuilder();
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no object header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoObject2()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object\t");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no object header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoObject3()
		{
			StringBuilder b = new StringBuilder();
			b.Append("obejct ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no object header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoObject4()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("zz9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid object", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoObject5()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append(" \n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid object", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoObject6()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid object", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoType1()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no type header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoType2()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("type\tcommit\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no type header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoType3()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("tpye commit\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no type header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoType4()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("type commit");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no tag header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoTagHeader1()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("type commit\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no tag header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoTagHeader2()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("type commit\n");
			b.Append("tag\tfoo\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no tag header", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagNoTagHeader3()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("type commit\n");
			b.Append("tga foo\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("no tag header", e.Message);
			}
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTagHasNoTaggerHeader()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("type commit\n");
			b.Append("tag foo\n");
			checker.CheckTag(Constants.EncodeASCII(b.ToString()));
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagInvalidTaggerHeader1()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("type commit\n");
			b.Append("tag foo\n");
			b.Append("tagger \n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid tagger", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTagInvalidTaggerHeader3()
		{
			StringBuilder b = new StringBuilder();
			b.Append("object ");
			b.Append("be9bfa841874ccc9f2ef7c48d0c76226f89b7189");
			b.Append('\n');
			b.Append("type commit\n");
			b.Append("tag foo\n");
			b.Append("tagger a < 1 +000\n");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTag(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted invalid tag");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid tagger", e.Message);
			}
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidEmptyTree()
		{
			checker.CheckTree(new byte[0]);
			checker.Check(Constants.OBJ_TREE, new byte[0]);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTree1()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 regular-file");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTree2()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100755 executable");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTree3()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "40000 tree");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTree4()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "120000 symlink");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTree5()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "160000 git link");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTree6()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 .a");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTreeSorting1()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 fooaaa");
			Entry(b, "100755 foobar");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTreeSorting2()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100755 fooaaa");
			Entry(b, "100644 foobar");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTreeSorting3()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "40000 a");
			Entry(b, "100644 b");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTreeSorting4()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 a");
			Entry(b, "40000 b");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTreeSorting5()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 a.c");
			Entry(b, "40000 a");
			Entry(b, "100644 a0c");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTreeSorting6()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "40000 a");
			Entry(b, "100644 apple");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTreeSorting7()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "40000 an orang");
			Entry(b, "40000 an orange");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		[NUnit.Framework.Test]
		public virtual void TestValidTreeSorting8()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 a");
			Entry(b, "100644 a0c");
			Entry(b, "100644 b");
			byte[] data = Constants.EncodeASCII(b.ToString());
			checker.CheckTree(data);
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeModeStartsWithZero1()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "0 a");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("mode starts with '0'", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeModeStartsWithZero2()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "0100644 a");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("mode starts with '0'", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeModeStartsWithZero3()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "040000 a");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("mode starts with '0'", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeModeNotOctal1()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "8 a");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid mode character", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeModeNotOctal2()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "Z a");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid mode character", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeModeNotSupportedMode1()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "1 a");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid mode 1", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeModeNotSupportedMode2()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "170000 a");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid mode " + 0xf000, e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeModeMissingName()
		{
			StringBuilder b = new StringBuilder();
			b.Append("100644");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("truncated in mode", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeNameContainsSlash()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 a/b");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("name contains '/'", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeNameIsEmpty()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 ");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("zero length name", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeNameIsDot()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 .");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid name '.'", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeNameIsDotDot()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 ..");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("invalid name '..'", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeTruncatedInName()
		{
			StringBuilder b = new StringBuilder();
			b.Append("100644 b");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("truncated in name", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeTruncatedInObjectId()
		{
			StringBuilder b = new StringBuilder();
			b.Append("100644 b\x0\x1\x2");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("truncated in object id", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeBadSorting1()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 foobar");
			Entry(b, "100644 fooaaa");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("incorrectly sorted", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeBadSorting2()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "40000 a");
			Entry(b, "100644 a.c");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("incorrectly sorted", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeBadSorting3()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 a0c");
			Entry(b, "40000 a");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("incorrectly sorted", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeDuplicateNames1()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 a");
			Entry(b, "100644 a");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("duplicate entry names", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeDuplicateNames2()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 a");
			Entry(b, "100755 a");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("duplicate entry names", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeDuplicateNames3()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 a");
			Entry(b, "40000 a");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("duplicate entry names", e.Message);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestInvalidTreeDuplicateNames4()
		{
			StringBuilder b = new StringBuilder();
			Entry(b, "100644 a");
			Entry(b, "100644 a.c");
			Entry(b, "100644 a.d");
			Entry(b, "100644 a.e");
			Entry(b, "40000 a");
			Entry(b, "100644 zoo");
			byte[] data = Constants.EncodeASCII(b.ToString());
			try
			{
				checker.CheckTree(data);
				NUnit.Framework.Assert.Fail("incorrectly accepted an invalid tree");
			}
			catch (CorruptObjectException e)
			{
				NUnit.Framework.Assert.AreEqual("duplicate entry names", e.Message);
			}
		}

		private static void Entry(StringBuilder b, string modeName)
		{
			b.Append(modeName);
			b.Append('\0');
			for (int i = 0; i < Constants.OBJECT_ID_LENGTH; i++)
			{
				b.Append((char)i);
			}
		}
	}
}
