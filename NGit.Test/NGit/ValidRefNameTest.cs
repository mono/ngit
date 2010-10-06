using NGit;
using NUnit.Framework;
using Sharpen;

namespace NGit
{
	public class ValidRefNameTest : TestCase
	{
		private static void AssertValid(bool exp, string name)
		{
			NUnit.Framework.Assert.AreEqual("\"" + name + "\"", exp, Repository.IsValidRefName
				(name));
		}

		public virtual void TestEmptyString()
		{
			AssertValid(false, string.Empty);
			AssertValid(false, "/");
		}

		public virtual void TestMustHaveTwoComponents()
		{
			AssertValid(false, "master");
			AssertValid(true, "heads/master");
		}

		public virtual void TestValidHead()
		{
			AssertValid(true, "refs/heads/master");
			AssertValid(true, "refs/heads/pu");
			AssertValid(true, "refs/heads/z");
			AssertValid(true, "refs/heads/FoO");
		}

		public virtual void TestValidTag()
		{
			AssertValid(true, "refs/tags/v1.0");
		}

		public virtual void TestNoLockSuffix()
		{
			AssertValid(false, "refs/heads/master.lock");
		}

		public virtual void TestNoDirectorySuffix()
		{
			AssertValid(false, "refs/heads/master/");
		}

		public virtual void TestNoSpace()
		{
			AssertValid(false, "refs/heads/i haz space");
		}

		public virtual void TestNoAsciiControlCharacters()
		{
			for (char c = '\0'; c < ' '; c++)
			{
				AssertValid(false, "refs/heads/mast" + c + "er");
			}
		}

		public virtual void TestNoBareDot()
		{
			AssertValid(false, "refs/heads/.");
			AssertValid(false, "refs/heads/..");
			AssertValid(false, "refs/heads/./master");
			AssertValid(false, "refs/heads/../master");
		}

		public virtual void TestNoLeadingOrTrailingDot()
		{
			AssertValid(false, ".");
			AssertValid(false, "refs/heads/.bar");
			AssertValid(false, "refs/heads/..bar");
			AssertValid(false, "refs/heads/bar.");
		}

		public virtual void TestContainsDot()
		{
			AssertValid(true, "refs/heads/m.a.s.t.e.r");
			AssertValid(false, "refs/heads/master..pu");
		}

		public virtual void TestNoMagicRefCharacters()
		{
			AssertValid(false, "refs/heads/master^");
			AssertValid(false, "refs/heads/^master");
			AssertValid(false, "^refs/heads/master");
			AssertValid(false, "refs/heads/master~");
			AssertValid(false, "refs/heads/~master");
			AssertValid(false, "~refs/heads/master");
			AssertValid(false, "refs/heads/master:");
			AssertValid(false, "refs/heads/:master");
			AssertValid(false, ":refs/heads/master");
		}

		public virtual void TestShellGlob()
		{
			AssertValid(false, "refs/heads/master?");
			AssertValid(false, "refs/heads/?master");
			AssertValid(false, "?refs/heads/master");
			AssertValid(false, "refs/heads/master[");
			AssertValid(false, "refs/heads/[master");
			AssertValid(false, "[refs/heads/master");
			AssertValid(false, "refs/heads/master*");
			AssertValid(false, "refs/heads/*master");
			AssertValid(false, "*refs/heads/master");
		}

		public virtual void TestValidSpecialCharacters()
		{
			AssertValid(true, "refs/heads/!");
			AssertValid(true, "refs/heads/\"");
			AssertValid(true, "refs/heads/#");
			AssertValid(true, "refs/heads/$");
			AssertValid(true, "refs/heads/%");
			AssertValid(true, "refs/heads/&");
			AssertValid(true, "refs/heads/'");
			AssertValid(true, "refs/heads/(");
			AssertValid(true, "refs/heads/)");
			AssertValid(true, "refs/heads/+");
			AssertValid(true, "refs/heads/,");
			AssertValid(true, "refs/heads/-");
			AssertValid(true, "refs/heads/;");
			AssertValid(true, "refs/heads/<");
			AssertValid(true, "refs/heads/=");
			AssertValid(true, "refs/heads/>");
			AssertValid(true, "refs/heads/@");
			AssertValid(true, "refs/heads/]");
			AssertValid(true, "refs/heads/_");
			AssertValid(true, "refs/heads/`");
			AssertValid(true, "refs/heads/{");
			AssertValid(true, "refs/heads/|");
			AssertValid(true, "refs/heads/}");
			// This is valid on UNIX, but not on Windows
			// hence we make in invalid due to non-portability
			//
			AssertValid(false, "refs/heads/\\");
		}

		public virtual void TestUnicodeNames()
		{
			AssertValid(true, "refs/heads/\u00e5ngstr\u00f6m");
		}

		public virtual void TestRefLogQueryIsValidRef()
		{
			AssertValid(false, "refs/heads/master@{1}");
			AssertValid(false, "refs/heads/master@{1.hour.ago}");
		}
	}
}
