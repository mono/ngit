using NGit.Ignore;
using NUnit.Framework;
using Sharpen;

namespace NGit.Ignore
{
	/// <summary>Tests ignore pattern matches</summary>
	public class IgnoreMatcherTest : TestCase
	{
		public virtual void TestBasic()
		{
			string pattern = "/test.stp";
			AssertMatched(pattern, "/test.stp");
			pattern = "#/test.stp";
			AssertNotMatched(pattern, "/test.stp");
		}

		public virtual void TestFileNameWildcards()
		{
			//Test basic * and ? for any pattern + any character
			string pattern = "*.st?";
			AssertMatched(pattern, "/test.stp");
			AssertMatched(pattern, "/anothertest.stg");
			AssertMatched(pattern, "/anothertest.st0");
			AssertNotMatched(pattern, "/anothertest.sta1");
			//Check that asterisk does not expand to "/"
			AssertNotMatched(pattern, "/another/test.sta1");
			//Same as above, with a leading slash to ensure that doesn't cause problems
			pattern = "/*.st?";
			AssertMatched(pattern, "/test.stp");
			AssertMatched(pattern, "/anothertest.stg");
			AssertMatched(pattern, "/anothertest.st0");
			AssertNotMatched(pattern, "/anothertest.sta1");
			//Check that asterisk does not expand to "/"
			AssertNotMatched(pattern, "/another/test.sta1");
			//Test for numbers
			pattern = "*.sta[0-5]";
			AssertMatched(pattern, "/test.sta5");
			AssertMatched(pattern, "/test.sta4");
			AssertMatched(pattern, "/test.sta3");
			AssertMatched(pattern, "/test.sta2");
			AssertMatched(pattern, "/test.sta1");
			AssertMatched(pattern, "/test.sta0");
			AssertMatched(pattern, "/anothertest.sta2");
			AssertNotMatched(pattern, "test.stag");
			AssertNotMatched(pattern, "test.sta6");
			//Test for letters
			pattern = "/[tv]est.sta[a-d]";
			AssertMatched(pattern, "/test.staa");
			AssertMatched(pattern, "/test.stab");
			AssertMatched(pattern, "/test.stac");
			AssertMatched(pattern, "/test.stad");
			AssertMatched(pattern, "/vest.stac");
			AssertNotMatched(pattern, "test.stae");
			AssertNotMatched(pattern, "test.sta9");
			//Test child directory/file is matched
			pattern = "/src/ne?";
			AssertMatched(pattern, "/src/new/");
			AssertMatched(pattern, "/src/new");
			AssertMatched(pattern, "/src/new/a.c");
			AssertMatched(pattern, "/src/new/a/a.c");
			AssertNotMatched(pattern, "/src/new.c");
			//Test name-only fnmatcher matches
			pattern = "ne?";
			AssertMatched(pattern, "/src/new/");
			AssertMatched(pattern, "/src/new");
			AssertMatched(pattern, "/src/new/a.c");
			AssertMatched(pattern, "/src/new/a/a.c");
			AssertMatched(pattern, "/neb");
			AssertNotMatched(pattern, "/src/new.c");
		}

		public virtual void TestTargetWithoutLeadingSlash()
		{
			//Test basic * and ? for any pattern + any character
			string pattern = "/*.st?";
			AssertMatched(pattern, "test.stp");
			AssertMatched(pattern, "anothertest.stg");
			AssertMatched(pattern, "anothertest.st0");
			AssertNotMatched(pattern, "anothertest.sta1");
			//Check that asterisk does not expand to ""
			AssertNotMatched(pattern, "another/test.sta1");
			//Same as above, with a leading slash to ensure that doesn't cause problems
			pattern = "/*.st?";
			AssertMatched(pattern, "test.stp");
			AssertMatched(pattern, "anothertest.stg");
			AssertMatched(pattern, "anothertest.st0");
			AssertNotMatched(pattern, "anothertest.sta1");
			//Check that asterisk does not expand to ""
			AssertNotMatched(pattern, "another/test.sta1");
			//Test for numbers
			pattern = "/*.sta[0-5]";
			AssertMatched(pattern, "test.sta5");
			AssertMatched(pattern, "test.sta4");
			AssertMatched(pattern, "test.sta3");
			AssertMatched(pattern, "test.sta2");
			AssertMatched(pattern, "test.sta1");
			AssertMatched(pattern, "test.sta0");
			AssertMatched(pattern, "anothertest.sta2");
			AssertNotMatched(pattern, "test.stag");
			AssertNotMatched(pattern, "test.sta6");
			//Test for letters
			pattern = "/[tv]est.sta[a-d]";
			AssertMatched(pattern, "test.staa");
			AssertMatched(pattern, "test.stab");
			AssertMatched(pattern, "test.stac");
			AssertMatched(pattern, "test.stad");
			AssertMatched(pattern, "vest.stac");
			AssertNotMatched(pattern, "test.stae");
			AssertNotMatched(pattern, "test.sta9");
			//Test child directory/file is matched
			pattern = "/src/ne?";
			AssertMatched(pattern, "src/new/");
			AssertMatched(pattern, "src/new");
			AssertMatched(pattern, "src/new/a.c");
			AssertMatched(pattern, "src/new/a/a.c");
			AssertNotMatched(pattern, "src/new.c");
			//Test name-only fnmatcher matches
			pattern = "ne?";
			AssertMatched(pattern, "src/new/");
			AssertMatched(pattern, "src/new");
			AssertMatched(pattern, "src/new/a.c");
			AssertMatched(pattern, "src/new/a/a.c");
			AssertMatched(pattern, "neb");
			AssertNotMatched(pattern, "src/new.c");
		}

		public virtual void TestParentDirectoryGitIgnores()
		{
			//Contains git ignore patterns such as might be seen in a parent directory
			//Test for wildcards
			string pattern = "/*/*.c";
			AssertMatched(pattern, "/file/a.c");
			AssertMatched(pattern, "/src/a.c");
			AssertNotMatched(pattern, "/src/new/a.c");
			//Test child directory/file is matched
			pattern = "/src/new";
			AssertMatched(pattern, "/src/new/");
			AssertMatched(pattern, "/src/new");
			AssertMatched(pattern, "/src/new/a.c");
			AssertMatched(pattern, "/src/new/a/a.c");
			AssertNotMatched(pattern, "/src/new.c");
			//Test child directory is matched, slash after name
			pattern = "/src/new/";
			AssertMatched(pattern, "/src/new/");
			AssertMatched(pattern, "/src/new/a.c");
			AssertMatched(pattern, "/src/new/a/a.c");
			AssertNotMatched(pattern, "/src/new");
			AssertNotMatched(pattern, "/src/new.c");
			//Test directory is matched by name only
			pattern = "b1";
			AssertMatched(pattern, "/src/new/a/b1/a.c");
			AssertNotMatched(pattern, "/src/new/a/b2/file.c");
			AssertNotMatched(pattern, "/src/new/a/bb1/file.c");
			AssertNotMatched(pattern, "/src/new/a/file.c");
		}

		public virtual void TestTrailingSlash()
		{
			string pattern = "/src/";
			AssertMatched(pattern, "/src/");
			AssertMatched(pattern, "/src/new");
			AssertMatched(pattern, "/src/new/a.c");
			AssertMatched(pattern, "/src/a.c");
			AssertNotMatched(pattern, "/src");
			AssertNotMatched(pattern, "/srcA/");
		}

		public virtual void TestNameOnlyMatches()
		{
			//Test matches for file extension
			string pattern = "*.stp";
			AssertMatched(pattern, "/test.stp");
			AssertMatched(pattern, "/src/test.stp");
			AssertNotMatched(pattern, "/test.stp1");
			AssertNotMatched(pattern, "/test.astp");
			//Test matches for name-only, applies to file name or folder name
			pattern = "src";
			AssertMatched(pattern, "/src/a.c");
			AssertMatched(pattern, "/src/new/a.c");
			AssertMatched(pattern, "/new/src/a.c");
			AssertMatched(pattern, "/file/src");
			AssertMatched(pattern, "/src/");
			//Test matches for name-only, applies to file name or folder name
			//With a small wildcard
			pattern = "?rc";
			AssertMatched(pattern, "/src/a.c");
			AssertMatched(pattern, "/src/new/a.c");
			AssertMatched(pattern, "/new/src/a.c");
			AssertMatched(pattern, "/file/src");
			AssertMatched(pattern, "/src/");
			//Test matches for name-only, applies to file name or folder name
			//With a small wildcard
			pattern = "?r[a-c]";
			AssertMatched(pattern, "/src/a.c");
			AssertMatched(pattern, "/src/new/a.c");
			AssertMatched(pattern, "/new/src/a.c");
			AssertMatched(pattern, "/file/src");
			AssertMatched(pattern, "/src/");
			AssertMatched(pattern, "/srb/a.c");
			AssertMatched(pattern, "/grb/new/a.c");
			AssertMatched(pattern, "/new/crb/a.c");
			AssertMatched(pattern, "/file/3rb");
			AssertMatched(pattern, "/xrb/");
			AssertMatched(pattern, "/3ra/a.c");
			AssertMatched(pattern, "/5ra/new/a.c");
			AssertMatched(pattern, "/new/1ra/a.c");
			AssertMatched(pattern, "/file/dra");
			AssertMatched(pattern, "/era/");
			AssertNotMatched(pattern, "/crg");
			AssertNotMatched(pattern, "/cr3");
		}

		public virtual void TestNegation()
		{
			string pattern = "!/test.stp";
			AssertMatched(pattern, "/test.stp");
		}

		public virtual void TestGetters()
		{
			IgnoreRule r = new IgnoreRule("/pattern/");
			NUnit.Framework.Assert.IsFalse(r.GetNameOnly());
			NUnit.Framework.Assert.IsTrue(r.DirOnly());
			NUnit.Framework.Assert.IsFalse(r.GetNegation());
			NUnit.Framework.Assert.AreEqual(r.GetPattern(), "/pattern");
			r = new IgnoreRule("/patter?/");
			NUnit.Framework.Assert.IsFalse(r.GetNameOnly());
			NUnit.Framework.Assert.IsTrue(r.DirOnly());
			NUnit.Framework.Assert.IsFalse(r.GetNegation());
			NUnit.Framework.Assert.AreEqual(r.GetPattern(), "/patter?");
			r = new IgnoreRule("patt*");
			NUnit.Framework.Assert.IsTrue(r.GetNameOnly());
			NUnit.Framework.Assert.IsFalse(r.DirOnly());
			NUnit.Framework.Assert.IsFalse(r.GetNegation());
			NUnit.Framework.Assert.AreEqual(r.GetPattern(), "patt*");
			r = new IgnoreRule("pattern");
			NUnit.Framework.Assert.IsTrue(r.GetNameOnly());
			NUnit.Framework.Assert.IsFalse(r.DirOnly());
			NUnit.Framework.Assert.IsFalse(r.GetNegation());
			NUnit.Framework.Assert.AreEqual(r.GetPattern(), "pattern");
			r = new IgnoreRule("!pattern");
			NUnit.Framework.Assert.IsTrue(r.GetNameOnly());
			NUnit.Framework.Assert.IsFalse(r.DirOnly());
			NUnit.Framework.Assert.IsTrue(r.GetNegation());
			NUnit.Framework.Assert.AreEqual(r.GetPattern(), "pattern");
			r = new IgnoreRule("!/pattern");
			NUnit.Framework.Assert.IsFalse(r.GetNameOnly());
			NUnit.Framework.Assert.IsFalse(r.DirOnly());
			NUnit.Framework.Assert.IsTrue(r.GetNegation());
			NUnit.Framework.Assert.AreEqual(r.GetPattern(), "/pattern");
			r = new IgnoreRule("!/patter?");
			NUnit.Framework.Assert.IsFalse(r.GetNameOnly());
			NUnit.Framework.Assert.IsFalse(r.DirOnly());
			NUnit.Framework.Assert.IsTrue(r.GetNegation());
			NUnit.Framework.Assert.AreEqual(r.GetPattern(), "/patter?");
		}

		/// <summary>Check for a match.</summary>
		/// <remarks>
		/// Check for a match. If target ends with "/", match will assume that the
		/// target is meant to be a directory.
		/// </remarks>
		/// <param name="pattern">Pattern as it would appear in a .gitignore file</param>
		/// <param name="target">Target file path relative to repository's GIT_DIR</param>
		public virtual void AssertMatched(string pattern, string target)
		{
			bool value = Match(pattern, target);
			NUnit.Framework.Assert.IsTrue("Expected a match for: " + pattern + " with: " + target
				, value);
		}

		/// <summary>Check for a match.</summary>
		/// <remarks>
		/// Check for a match. If target ends with "/", match will assume that the
		/// target is meant to be a directory.
		/// </remarks>
		/// <param name="pattern">Pattern as it would appear in a .gitignore file</param>
		/// <param name="target">Target file path relative to repository's GIT_DIR</param>
		public virtual void AssertNotMatched(string pattern, string target)
		{
			bool value = Match(pattern, target);
			NUnit.Framework.Assert.IsFalse("Expected no match for: " + pattern + " with: " + 
				target, value);
		}

		/// <summary>Check for a match.</summary>
		/// <remarks>
		/// Check for a match. If target ends with "/", match will assume that the
		/// target is meant to be a directory.
		/// </remarks>
		/// <param name="pattern">Pattern as it would appear in a .gitignore file</param>
		/// <param name="target">Target file path relative to repository's GIT_DIR</param>
		/// <returns>
		/// Result of
		/// <see cref="IgnoreRule.IsMatch(string, bool)">IgnoreRule.IsMatch(string, bool)</see>
		/// </returns>
		private bool Match(string pattern, string target)
		{
			IgnoreRule r = new IgnoreRule(pattern);
			//If speed of this test is ever an issue, we can use a presetRule field
			//to avoid recompiling a pattern each time.
			return r.IsMatch(target, target.EndsWith("/"));
		}
	}
}
