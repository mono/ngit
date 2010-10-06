using NGit.Util;
using NUnit.Framework;
using Sharpen;

namespace NGit.Util
{
	public class StringUtilsTest : TestCase
	{
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

		public virtual void TestToLowerCaseString()
		{
			NUnit.Framework.Assert.AreEqual("\n abcdefghijklmnopqrstuvwxyz\n", StringUtils.ToLowerCase
				("\n ABCDEFGHIJKLMNOPQRSTUVWXYZ\n"));
		}

		public virtual void TestEqualsIgnoreCase1()
		{
			string a = "FOO";
			NUnit.Framework.Assert.IsTrue(StringUtils.EqualsIgnoreCase(a, a));
		}

		public virtual void TestEqualsIgnoreCase2()
		{
			NUnit.Framework.Assert.IsFalse(StringUtils.EqualsIgnoreCase("a", string.Empty));
		}

		public virtual void TestEqualsIgnoreCase3()
		{
			NUnit.Framework.Assert.IsFalse(StringUtils.EqualsIgnoreCase("a", "b"));
			NUnit.Framework.Assert.IsFalse(StringUtils.EqualsIgnoreCase("ac", "ab"));
		}

		public virtual void TestEqualsIgnoreCase4()
		{
			NUnit.Framework.Assert.IsTrue(StringUtils.EqualsIgnoreCase("a", "a"));
			NUnit.Framework.Assert.IsTrue(StringUtils.EqualsIgnoreCase("A", "a"));
			NUnit.Framework.Assert.IsTrue(StringUtils.EqualsIgnoreCase("a", "A"));
		}
	}
}
