using System.Text;
using NGit;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public class RevTagParseTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestTagBlob()
		{
			TestOneType(Constants.OBJ_BLOB);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTagTree()
		{
			TestOneType(Constants.OBJ_TREE);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTagCommit()
		{
			TestOneType(Constants.OBJ_COMMIT);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTagTag()
		{
			TestOneType(Constants.OBJ_TAG);
		}

		/// <exception cref="System.Exception"></exception>
		private void TestOneType(int typeCode)
		{
			ObjectId id = Id("9788669ad918b6fcce64af8882fc9a81cb6aba67");
			StringBuilder b = new StringBuilder();
			b.Append("object " + id.Name + "\n");
			b.Append("type " + Constants.TypeString(typeCode) + "\n");
			b.Append("tag v1.2.3.4.5\n");
			b.Append("tagger A U. Thor <a_u_thor@example.com> 1218123387 +0700\n");
			b.Append("\n");
			RevWalk rw = new RevWalk(db);
			RevTag c;
			c = new RevTag(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			NUnit.Framework.Assert.IsNull(c.GetObject());
			NUnit.Framework.Assert.IsNull(c.GetTagName());
			c.ParseCanonical(rw, Sharpen.Runtime.GetBytesForString(b.ToString(), "UTF-8"));
			NUnit.Framework.Assert.IsNotNull(c.GetObject());
			AssertEquals(id, c.GetObject().Id);
			NUnit.Framework.Assert.AreSame(rw.LookupAny(id, typeCode), c.GetObject());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestParseAllFields()
		{
			ObjectId treeId = Id("9788669ad918b6fcce64af8882fc9a81cb6aba67");
			string name = "v1.2.3.4.5";
			string taggerName = "A U. Thor";
			string taggerEmail = "a_u_thor@example.com";
			int taggerTime = 1218123387;
			StringBuilder body = new StringBuilder();
			body.Append("object ");
			body.Append(treeId.Name);
			body.Append("\n");
			body.Append("type tree\n");
			body.Append("tag ");
			body.Append(name);
			body.Append("\n");
			body.Append("tagger ");
			body.Append(taggerName);
			body.Append(" <");
			body.Append(taggerEmail);
			body.Append("> ");
			body.Append(taggerTime);
			body.Append(" +0700\n");
			body.Append("\n");
			RevWalk rw = new RevWalk(db);
			RevTag c;
			c = new RevTag(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			NUnit.Framework.Assert.IsNull(c.GetObject());
			NUnit.Framework.Assert.IsNull(c.GetTagName());
			c.ParseCanonical(rw, Sharpen.Runtime.GetBytesForString(body.ToString(), "UTF-8"));
			NUnit.Framework.Assert.IsNotNull(c.GetObject());
			AssertEquals(treeId, c.GetObject().Id);
			NUnit.Framework.Assert.AreSame(rw.LookupTree(treeId), c.GetObject());
			NUnit.Framework.Assert.IsNotNull(c.GetTagName());
			NUnit.Framework.Assert.AreEqual(name, c.GetTagName());
			NUnit.Framework.Assert.AreEqual(string.Empty, c.GetFullMessage());
			PersonIdent cTagger = c.GetTaggerIdent();
			NUnit.Framework.Assert.IsNotNull(cTagger);
			NUnit.Framework.Assert.AreEqual(taggerName, cTagger.GetName());
			NUnit.Framework.Assert.AreEqual(taggerEmail, cTagger.GetEmailAddress());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestParseOldStyleNoTagger()
		{
			ObjectId treeId = Id("9788669ad918b6fcce64af8882fc9a81cb6aba67");
			string name = "v1.2.3.4.5";
			string message = "test\n" + "\n" + "-----BEGIN PGP SIGNATURE-----\n" + "Version: GnuPG v1.4.1 (GNU/Linux)\n"
				 + "\n" + "iD8DBQBC0b9oF3Y\n" + "-----END PGP SIGNATURE------n";
			//
			//
			//
			//
			//
			//
			StringBuilder body = new StringBuilder();
			body.Append("object ");
			body.Append(treeId.Name);
			body.Append("\n");
			body.Append("type tree\n");
			body.Append("tag ");
			body.Append(name);
			body.Append("\n");
			body.Append("\n");
			body.Append(message);
			RevWalk rw = new RevWalk(db);
			RevTag c;
			c = new RevTag(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			NUnit.Framework.Assert.IsNull(c.GetObject());
			NUnit.Framework.Assert.IsNull(c.GetTagName());
			c.ParseCanonical(rw, Sharpen.Runtime.GetBytesForString(body.ToString(), "UTF-8"));
			NUnit.Framework.Assert.IsNotNull(c.GetObject());
			AssertEquals(treeId, c.GetObject().Id);
			NUnit.Framework.Assert.AreSame(rw.LookupTree(treeId), c.GetObject());
			NUnit.Framework.Assert.IsNotNull(c.GetTagName());
			NUnit.Framework.Assert.AreEqual(name, c.GetTagName());
			NUnit.Framework.Assert.AreEqual("test", c.GetShortMessage());
			NUnit.Framework.Assert.AreEqual(message, c.GetFullMessage());
			NUnit.Framework.Assert.IsNull(c.GetTaggerIdent());
		}

		/// <exception cref="System.Exception"></exception>
		private RevTag Create(string msg)
		{
			StringBuilder b = new StringBuilder();
			b.Append("object 9788669ad918b6fcce64af8882fc9a81cb6aba67\n");
			b.Append("type tree\n");
			b.Append("tag v1.2.3.4.5\n");
			b.Append("tagger A U. Thor <a_u_thor@example.com> 1218123387 +0700\n");
			b.Append("\n");
			b.Append(msg);
			RevTag c;
			c = new RevTag(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			c.ParseCanonical(new RevWalk(db), Sharpen.Runtime.GetBytesForString(b.ToString(), 
				"UTF-8"));
			return c;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestParse_implicit_UTF8_encoded()
		{
			ByteArrayOutputStream b = new ByteArrayOutputStream();
			b.Write(Sharpen.Runtime.GetBytesForString("object 9788669ad918b6fcce64af8882fc9a81cb6aba67\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("type tree\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("tag v1.2.3.4.5\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("tagger F\u00f6r fattare <a_u_thor@example.com> 1218123387 +0700\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("Sm\u00f6rg\u00e5sbord\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\u304d\u308c\u3044\n", "UTF-8"));
			RevTag c;
			c = new RevTag(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			c.ParseCanonical(new RevWalk(db), b.ToByteArray());
			NUnit.Framework.Assert.AreEqual("F\u00f6r fattare", c.GetTaggerIdent().GetName());
			NUnit.Framework.Assert.AreEqual("Sm\u00f6rg\u00e5sbord", c.GetShortMessage());
			NUnit.Framework.Assert.AreEqual("Sm\u00f6rg\u00e5sbord\n\n\u304d\u308c\u3044\n", 
				c.GetFullMessage());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestParse_implicit_mixed_encoded()
		{
			ByteArrayOutputStream b = new ByteArrayOutputStream();
			b.Write(Sharpen.Runtime.GetBytesForString("object 9788669ad918b6fcce64af8882fc9a81cb6aba67\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("type tree\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("tag v1.2.3.4.5\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("tagger F\u00f6r fattare <a_u_thor@example.com> 1218123387 +0700\n"
				, "ISO-8859-1"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("Sm\u00f6rg\u00e5sbord\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\u304d\u308c\u3044\n", "UTF-8"));
			RevTag c;
			c = new RevTag(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			c.ParseCanonical(new RevWalk(db), b.ToByteArray());
			NUnit.Framework.Assert.AreEqual("F\u00f6r fattare", c.GetTaggerIdent().GetName());
			NUnit.Framework.Assert.AreEqual("Sm\u00f6rg\u00e5sbord", c.GetShortMessage());
			NUnit.Framework.Assert.AreEqual("Sm\u00f6rg\u00e5sbord\n\n\u304d\u308c\u3044\n", 
				c.GetFullMessage());
		}

		/// <summary>Test parsing of a commit whose encoding is given and works.</summary>
		/// <remarks>Test parsing of a commit whose encoding is given and works.</remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual void TestParse_explicit_encoded()
		{
			ByteArrayOutputStream b = new ByteArrayOutputStream();
			b.Write(Sharpen.Runtime.GetBytesForString("object 9788669ad918b6fcce64af8882fc9a81cb6aba67\n"
				, "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("type tree\n", "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("tag v1.2.3.4.5\n", "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("tagger F\u00f6r fattare <a_u_thor@example.com> 1218123387 +0700\n"
				, "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("encoding euc_JP\n", "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("\u304d\u308c\u3044\n", "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "EUC-JP"));
			b.Write(Sharpen.Runtime.GetBytesForString("Hi\n", "EUC-JP"));
			RevTag c;
			c = new RevTag(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			c.ParseCanonical(new RevWalk(db), b.ToByteArray());
			NUnit.Framework.Assert.AreEqual("F\u00f6r fattare", c.GetTaggerIdent().GetName());
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
		public virtual void TestParse_explicit_bad_encoded()
		{
			ByteArrayOutputStream b = new ByteArrayOutputStream();
			b.Write(Sharpen.Runtime.GetBytesForString("object 9788669ad918b6fcce64af8882fc9a81cb6aba67\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("type tree\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("tag v1.2.3.4.5\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("tagger F\u00f6r fattare <a_u_thor@example.com> 1218123387 +0700\n"
				, "ISO-8859-1"));
			b.Write(Sharpen.Runtime.GetBytesForString("encoding EUC-JP\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\u304d\u308c\u3044\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("Hi\n", "UTF-8"));
			RevTag c;
			c = new RevTag(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			c.ParseCanonical(new RevWalk(db), b.ToByteArray());
			NUnit.Framework.Assert.AreEqual("F\u00f6r fattare", c.GetTaggerIdent().GetName());
			NUnit.Framework.Assert.AreEqual("\u304d\u308c\u3044", c.GetShortMessage());
			NUnit.Framework.Assert.AreEqual("\u304d\u308c\u3044\n\nHi\n", c.GetFullMessage());
		}

		/// <summary>This is a twisted case too, but show what we expect here.</summary>
		/// <remarks>
		/// This is a twisted case too, but show what we expect here. We can revise
		/// the expectations provided this case is updated.
		/// What happens here is that an encoding us given, but data is not encoded
		/// that way (and we can detect it), so we try other encodings. Here data
		/// could actually be decoded in the stated encoding, but we override using
		/// UTF-8.
		/// </remarks>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual void TestParse_explicit_bad_encoded2()
		{
			ByteArrayOutputStream b = new ByteArrayOutputStream();
			b.Write(Sharpen.Runtime.GetBytesForString("object 9788669ad918b6fcce64af8882fc9a81cb6aba67\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("type tree\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("tag v1.2.3.4.5\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("tagger F\u00f6r fattare <a_u_thor@example.com> 1218123387 +0700\n"
				, "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("encoding ISO-8859-1\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\u304d\u308c\u3044\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("\n", "UTF-8"));
			b.Write(Sharpen.Runtime.GetBytesForString("Hi\n", "UTF-8"));
			RevTag c;
			c = new RevTag(Id("9473095c4cb2f12aefe1db8a355fe3fafba42f67"));
			c.ParseCanonical(new RevWalk(db), b.ToByteArray());
			NUnit.Framework.Assert.AreEqual("F\u00f6r fattare", c.GetTaggerIdent().GetName());
			NUnit.Framework.Assert.AreEqual("\u304d\u308c\u3044", c.GetShortMessage());
			NUnit.Framework.Assert.AreEqual("\u304d\u308c\u3044\n\nHi\n", c.GetFullMessage());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestParse_NoMessage()
		{
			string msg = string.Empty;
			RevTag c = Create(msg);
			NUnit.Framework.Assert.AreEqual(msg, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(msg, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestParse_OnlyLFMessage()
		{
			RevTag c = Create("\n");
			NUnit.Framework.Assert.AreEqual("\n", c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(string.Empty, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestParse_ShortLineOnlyNoLF()
		{
			string shortMsg = "This is a short message.";
			RevTag c = Create(shortMsg);
			NUnit.Framework.Assert.AreEqual(shortMsg, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(shortMsg, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestParse_ShortLineOnlyEndLF()
		{
			string shortMsg = "This is a short message.";
			string fullMsg = shortMsg + "\n";
			RevTag c = Create(fullMsg);
			NUnit.Framework.Assert.AreEqual(fullMsg, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(shortMsg, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestParse_ShortLineOnlyEmbeddedLF()
		{
			string fullMsg = "This is a\nshort message.";
			string shortMsg = fullMsg.Replace('\n', ' ');
			RevTag c = Create(fullMsg);
			NUnit.Framework.Assert.AreEqual(fullMsg, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(shortMsg, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestParse_ShortLineOnlyEmbeddedAndEndingLF()
		{
			string fullMsg = "This is a\nshort message.\n";
			string shortMsg = "This is a short message.";
			RevTag c = Create(fullMsg);
			NUnit.Framework.Assert.AreEqual(fullMsg, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(shortMsg, c.GetShortMessage());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestParse_GitStyleMessage()
		{
			string shortMsg = "This fixes a bug.";
			string body = "We do it with magic and pixie dust and stuff.\n" + "\n" + "Signed-off-by: A U. Thor <author@example.com>\n";
			string fullMsg = shortMsg + "\n" + "\n" + body;
			RevTag c = Create(fullMsg);
			NUnit.Framework.Assert.AreEqual(fullMsg, c.GetFullMessage());
			NUnit.Framework.Assert.AreEqual(shortMsg, c.GetShortMessage());
		}

		/// <exception cref="NGit.Errors.CorruptObjectException"></exception>
		public virtual void TestParse_PublicParseMethod()
		{
			ObjectInserter.Formatter fmt = new ObjectInserter.Formatter();
			TagBuilder src = new TagBuilder();
			src.SetObjectId(fmt.IdFor(Constants.OBJ_TREE, new byte[] {  }), Constants.OBJ_TREE
				);
			src.SetTagger(committer);
			src.SetTag("a.test");
			src.SetMessage("Test tag\n\nThis is a test.\n");
			RevTag p = RevTag.Parse(src.Format());
			AssertEquals(src.GetObjectId(), p.GetObject());
			NUnit.Framework.Assert.AreEqual(committer, p.GetTaggerIdent());
			NUnit.Framework.Assert.AreEqual("a.test", p.GetTagName());
			NUnit.Framework.Assert.AreEqual("Test tag", p.GetShortMessage());
			NUnit.Framework.Assert.AreEqual(src.GetMessage(), p.GetFullMessage());
		}

		private static ObjectId Id(string str)
		{
			return ObjectId.FromString(str);
		}
	}
}
