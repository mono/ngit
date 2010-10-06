using NGit;
using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class QuotedStringBourneStyleTest : TestCase
	{
		private static void AssertQuote(string @in, string exp)
		{
			string r = QuotedString.BOURNE.Quote(@in);
			NUnit.Framework.Assert.AreNotSame(@in, r);
			NUnit.Framework.Assert.IsFalse(@in.Equals(r));
			NUnit.Framework.Assert.AreEqual('\'' + exp + '\'', r);
		}

		private static void AssertDequote(string exp, string @in)
		{
			byte[] b = Constants.Encode('\'' + @in + '\'');
			string r = QuotedString.BOURNE.Dequote(b, 0, b.Length);
			NUnit.Framework.Assert.AreEqual(exp, r);
		}

		public virtual void TestQuote_Empty()
		{
			NUnit.Framework.Assert.AreEqual("''", QuotedString.BOURNE.Quote(string.Empty));
		}

		public virtual void TestDequote_Empty1()
		{
			NUnit.Framework.Assert.AreEqual(string.Empty, QuotedString.BOURNE.Dequote(new byte
				[0], 0, 0));
		}

		public virtual void TestDequote_Empty2()
		{
			NUnit.Framework.Assert.AreEqual(string.Empty, QuotedString.BOURNE.Dequote(new byte
				[] { (byte)('\''), (byte)('\'') }, 0, 2));
		}

		public virtual void TestDequote_SoleSq()
		{
			NUnit.Framework.Assert.AreEqual(string.Empty, QuotedString.BOURNE.Dequote(new byte
				[] { (byte)('\'') }, 0, 1));
		}

		public virtual void TestQuote_BareA()
		{
			AssertQuote("a", "a");
		}

		public virtual void TestDequote_BareA()
		{
			string @in = "a";
			byte[] b = Constants.Encode(@in);
			NUnit.Framework.Assert.AreEqual(@in, QuotedString.BOURNE.Dequote(b, 0, b.Length));
		}

		public virtual void TestDequote_BareABCZ_OnlyBC()
		{
			string @in = "abcz";
			byte[] b = Constants.Encode(@in);
			int p = @in.IndexOf('b');
			NUnit.Framework.Assert.AreEqual("bc", QuotedString.BOURNE.Dequote(b, p, p + 2));
		}

		public virtual void TestDequote_LoneBackslash()
		{
			AssertDequote("\\", "\\");
		}

		public virtual void TestQuote_NamedEscapes()
		{
			AssertQuote("'", "'\\''");
			AssertQuote("!", "'\\!'");
			AssertQuote("a'b", "a'\\''b");
			AssertQuote("a!b", "a'\\!'b");
		}

		public virtual void TestDequote_NamedEscapes()
		{
			AssertDequote("'", "'\\''");
			AssertDequote("!", "'\\!'");
			AssertDequote("a'b", "a'\\''b");
			AssertDequote("a!b", "a'\\!'b");
		}
	}
}
