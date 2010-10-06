using NGit;
using NGit.Diff;
using NUnit.Framework;
using Sharpen;

namespace NGit.Diff
{
	public class RawTextIgnoreWhitespaceChangeTest : TestCase
	{
		private readonly RawTextComparator cmp = RawTextComparator.WS_IGNORE_CHANGE;

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
			RawText a = new RawText(cmp, Constants.EncodeASCII("foo-a\n         \n a b c\na      \n  foo\na  b  c\n"
				));
			RawText b = new RawText(cmp, Constants.EncodeASCII("foo-a        b\n\nab  c\na\nfoo\na b     c  \n"
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
			// "a      " == "a"
			NUnit.Framework.Assert.IsTrue(cmp.Equals(a, 3, b, 3));
			NUnit.Framework.Assert.IsTrue(cmp.Equals(b, 3, a, 3));
			// "  foo" != "foo"
			NUnit.Framework.Assert.IsFalse(cmp.Equals(a, 4, b, 4));
			NUnit.Framework.Assert.IsFalse(cmp.Equals(b, 4, a, 4));
			// "a  b  c" == "a b     c  "
			NUnit.Framework.Assert.IsTrue(cmp.Equals(a, 5, b, 5));
			NUnit.Framework.Assert.IsTrue(cmp.Equals(b, 5, a, 5));
		}
	}
}
