using NGit.Errors;
using NGit.Fnmatch;
using NUnit.Framework;
using Sharpen;

namespace NGit.Fnmatch
{
	public class FileNameMatcherTest : TestCase
	{
		/// <exception cref="NGit.Errors.InvalidPatternException"></exception>
		private void AssertMatch(string pattern, string input, bool matchExpected, bool appendCanMatchExpected
			)
		{
			FileNameMatcher matcher = new FileNameMatcher(pattern, null);
			matcher.Append(input);
			NUnit.Framework.Assert.AreEqual(matchExpected, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(appendCanMatchExpected, matcher.CanAppendMatch());
		}

		/// <exception cref="NGit.Errors.InvalidPatternException"></exception>
		private void AssertFileNameMatch(string pattern, string input, char excludedCharacter
			, bool matchExpected, bool appendCanMatchExpected)
		{
			FileNameMatcher matcher = new FileNameMatcher(pattern, excludedCharacter);
			matcher.Append(input);
			NUnit.Framework.Assert.AreEqual(matchExpected, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(appendCanMatchExpected, matcher.CanAppendMatch());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimplePatternCase0()
		{
			AssertMatch(string.Empty, string.Empty, true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimplePatternCase1()
		{
			AssertMatch("ab", "a", false, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimplePatternCase2()
		{
			AssertMatch("ab", "ab", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimplePatternCase3()
		{
			AssertMatch("ab", "ac", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimplePatternCase4()
		{
			AssertMatch("ab", "abc", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleWirdcardCase0()
		{
			AssertMatch("?", "a", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleWildCardCase1()
		{
			AssertMatch("??", "a", false, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleWildCardCase2()
		{
			AssertMatch("??", "ab", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleWildCardCase3()
		{
			AssertMatch("??", "abc", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleStarCase0()
		{
			AssertMatch("*", string.Empty, true, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleStarCase1()
		{
			AssertMatch("*", "a", true, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleStarCase2()
		{
			AssertMatch("*", "ab", true, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSimpleStarCase0()
		{
			AssertMatch("a*b", "a", false, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSimpleStarCase1()
		{
			AssertMatch("a*c", "ac", true, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSimpleStarCase2()
		{
			AssertMatch("a*c", "ab", false, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSimpleStarCase3()
		{
			AssertMatch("a*c", "abc", true, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestManySolutionsCase0()
		{
			AssertMatch("a*a*a", "aaa", true, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestManySolutionsCase1()
		{
			AssertMatch("a*a*a", "aaaa", true, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestManySolutionsCase2()
		{
			AssertMatch("a*a*a", "ababa", true, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestManySolutionsCase3()
		{
			AssertMatch("a*a*a", "aaaaaaaa", true, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestManySolutionsCase4()
		{
			AssertMatch("a*a*a", "aaaaaaab", false, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleGroupCase0()
		{
			AssertMatch("[ab]", "a", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleGroupCase1()
		{
			AssertMatch("[ab]", "b", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleGroupCase2()
		{
			AssertMatch("[ab]", "ab", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleGroupRangeCase0()
		{
			AssertMatch("[b-d]", "a", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleGroupRangeCase1()
		{
			AssertMatch("[b-d]", "b", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleGroupRangeCase2()
		{
			AssertMatch("[b-d]", "c", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleGroupRangeCase3()
		{
			AssertMatch("[b-d]", "d", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleGroupRangeCase4()
		{
			AssertMatch("[b-d]", "e", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestVerySimpleGroupRangeCase5()
		{
			AssertMatch("[b-d]", "-", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoGroupsCase0()
		{
			AssertMatch("[b-d][ab]", "bb", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoGroupsCase1()
		{
			AssertMatch("[b-d][ab]", "ca", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoGroupsCase2()
		{
			AssertMatch("[b-d][ab]", "fa", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoGroupsCase3()
		{
			AssertMatch("[b-d][ab]", "bc", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoRangesInOneGroupCase0()
		{
			AssertMatch("[b-ce-e]", "a", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoRangesInOneGroupCase1()
		{
			AssertMatch("[b-ce-e]", "b", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoRangesInOneGroupCase2()
		{
			AssertMatch("[b-ce-e]", "c", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoRangesInOneGroupCase3()
		{
			AssertMatch("[b-ce-e]", "d", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoRangesInOneGroupCase4()
		{
			AssertMatch("[b-ce-e]", "e", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestTwoRangesInOneGroupCase5()
		{
			AssertMatch("[b-ce-e]", "f", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestIncompleteRangesInOneGroupCase0()
		{
			AssertMatch("a[b-]", "ab", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestIncompleteRangesInOneGroupCase1()
		{
			AssertMatch("a[b-]", "ac", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestIncompleteRangesInOneGroupCase2()
		{
			AssertMatch("a[b-]", "a-", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCombinedRangesInOneGroupCase0()
		{
			AssertMatch("[a-c-e]", "b", true, false);
		}

		/// <summary>The c belongs to the range a-c.</summary>
		/// <remarks>
		/// The c belongs to the range a-c. "-e" is no valid range so d should not
		/// match.
		/// </remarks>
		/// <exception cref="System.Exception">for some reasons</exception>
		public virtual void TestCombinedRangesInOneGroupCase1()
		{
			AssertMatch("[a-c-e]", "d", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCombinedRangesInOneGroupCase2()
		{
			AssertMatch("[a-c-e]", "e", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInversedGroupCase0()
		{
			AssertMatch("[!b-c]", "a", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInversedGroupCase1()
		{
			AssertMatch("[!b-c]", "b", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInversedGroupCase2()
		{
			AssertMatch("[!b-c]", "c", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestInversedGroupCase3()
		{
			AssertMatch("[!b-c]", "d", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestAlphaGroupCase0()
		{
			AssertMatch("[[:alpha:]]", "d", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestAlphaGroupCase1()
		{
			AssertMatch("[[:alpha:]]", ":", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestAlphaGroupCase2()
		{
			// \u00f6 = 'o' with dots on it
			AssertMatch("[[:alpha:]]", "\u00f6", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test2AlphaGroupsCase0()
		{
			// \u00f6 = 'o' with dots on it
			AssertMatch("[[:alpha:]][[:alpha:]]", "a\u00f6", true, false);
			AssertMatch("[[:alpha:]][[:alpha:]]", "a1", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestAlnumGroupCase0()
		{
			AssertMatch("[[:alnum:]]", "a", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestAlnumGroupCase1()
		{
			AssertMatch("[[:alnum:]]", "1", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestAlnumGroupCase2()
		{
			AssertMatch("[[:alnum:]]", ":", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBlankGroupCase0()
		{
			AssertMatch("[[:blank:]]", " ", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBlankGroupCase1()
		{
			AssertMatch("[[:blank:]]", "\t", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBlankGroupCase2()
		{
			AssertMatch("[[:blank:]]", "\r", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBlankGroupCase3()
		{
			AssertMatch("[[:blank:]]", "\n", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestBlankGroupCase4()
		{
			AssertMatch("[[:blank:]]", "a", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCntrlGroupCase0()
		{
			AssertMatch("[[:cntrl:]]", "a", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCntrlGroupCase1()
		{
			AssertMatch("[[:cntrl:]]", (char)7.ToString(), true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDigitGroupCase0()
		{
			AssertMatch("[[:digit:]]", "0", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDigitGroupCase1()
		{
			AssertMatch("[[:digit:]]", "5", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDigitGroupCase2()
		{
			AssertMatch("[[:digit:]]", "9", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDigitGroupCase3()
		{
			// \u06f9 = EXTENDED ARABIC-INDIC DIGIT NINE
			AssertMatch("[[:digit:]]", "\u06f9", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDigitGroupCase4()
		{
			AssertMatch("[[:digit:]]", "a", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDigitGroupCase5()
		{
			AssertMatch("[[:digit:]]", "]", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGraphGroupCase0()
		{
			AssertMatch("[[:graph:]]", "]", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGraphGroupCase1()
		{
			AssertMatch("[[:graph:]]", "a", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGraphGroupCase2()
		{
			AssertMatch("[[:graph:]]", ".", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGraphGroupCase3()
		{
			AssertMatch("[[:graph:]]", "0", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGraphGroupCase4()
		{
			AssertMatch("[[:graph:]]", " ", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestGraphGroupCase5()
		{
			// \u00f6 = 'o' with dots on it
			AssertMatch("[[:graph:]]", "\u00f6", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestLowerGroupCase0()
		{
			AssertMatch("[[:lower:]]", "a", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestLowerGroupCase1()
		{
			AssertMatch("[[:lower:]]", "h", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestLowerGroupCase2()
		{
			AssertMatch("[[:lower:]]", "A", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestLowerGroupCase3()
		{
			AssertMatch("[[:lower:]]", "H", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestLowerGroupCase4()
		{
			// \u00e4 = small 'a' with dots on it
			AssertMatch("[[:lower:]]", "\u00e4", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestLowerGroupCase5()
		{
			AssertMatch("[[:lower:]]", ".", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPrintGroupCase0()
		{
			AssertMatch("[[:print:]]", "]", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPrintGroupCase1()
		{
			AssertMatch("[[:print:]]", "a", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPrintGroupCase2()
		{
			AssertMatch("[[:print:]]", ".", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPrintGroupCase3()
		{
			AssertMatch("[[:print:]]", "0", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPrintGroupCase4()
		{
			AssertMatch("[[:print:]]", " ", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPrintGroupCase5()
		{
			// \u00f6 = 'o' with dots on it
			AssertMatch("[[:print:]]", "\u00f6", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPunctGroupCase0()
		{
			AssertMatch("[[:punct:]]", ".", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPunctGroupCase1()
		{
			AssertMatch("[[:punct:]]", "@", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPunctGroupCase2()
		{
			AssertMatch("[[:punct:]]", " ", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestPunctGroupCase3()
		{
			AssertMatch("[[:punct:]]", "a", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpaceGroupCase0()
		{
			AssertMatch("[[:space:]]", " ", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpaceGroupCase1()
		{
			AssertMatch("[[:space:]]", "\t", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpaceGroupCase2()
		{
			AssertMatch("[[:space:]]", "\r", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpaceGroupCase3()
		{
			AssertMatch("[[:space:]]", "\n", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpaceGroupCase4()
		{
			AssertMatch("[[:space:]]", "a", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestUpperGroupCase0()
		{
			AssertMatch("[[:upper:]]", "a", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestUpperGroupCase1()
		{
			AssertMatch("[[:upper:]]", "h", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestUpperGroupCase2()
		{
			AssertMatch("[[:upper:]]", "A", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestUpperGroupCase3()
		{
			AssertMatch("[[:upper:]]", "H", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestUpperGroupCase4()
		{
			// \u00c4 = 'A' with dots on it
			AssertMatch("[[:upper:]]", "\u00c4", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestUpperGroupCase5()
		{
			AssertMatch("[[:upper:]]", ".", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestXDigitGroupCase0()
		{
			AssertMatch("[[:xdigit:]]", "a", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestXDigitGroupCase1()
		{
			AssertMatch("[[:xdigit:]]", "d", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestXDigitGroupCase2()
		{
			AssertMatch("[[:xdigit:]]", "f", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestXDigitGroupCase3()
		{
			AssertMatch("[[:xdigit:]]", "0", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestXDigitGroupCase4()
		{
			AssertMatch("[[:xdigit:]]", "5", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestXDigitGroupCase5()
		{
			AssertMatch("[[:xdigit:]]", "9", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestXDigitGroupCase6()
		{
			AssertMatch("[[:xdigit:]]", "۹", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestXDigitGroupCase7()
		{
			AssertMatch("[[:xdigit:]]", ".", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWordroupCase0()
		{
			AssertMatch("[[:word:]]", "g", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWordroupCase1()
		{
			// \u00f6 = 'o' with dots on it
			AssertMatch("[[:word:]]", "\u00f6", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWordroupCase2()
		{
			AssertMatch("[[:word:]]", "5", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWordroupCase3()
		{
			AssertMatch("[[:word:]]", "_", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWordroupCase4()
		{
			AssertMatch("[[:word:]]", " ", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWordroupCase5()
		{
			AssertMatch("[[:word:]]", ".", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMixedGroupCase0()
		{
			AssertMatch("[A[:lower:]C3-5]", "A", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMixedGroupCase1()
		{
			AssertMatch("[A[:lower:]C3-5]", "C", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMixedGroupCase2()
		{
			AssertMatch("[A[:lower:]C3-5]", "e", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMixedGroupCase3()
		{
			AssertMatch("[A[:lower:]C3-5]", "3", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMixedGroupCase4()
		{
			AssertMatch("[A[:lower:]C3-5]", "4", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMixedGroupCase5()
		{
			AssertMatch("[A[:lower:]C3-5]", "5", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMixedGroupCase6()
		{
			AssertMatch("[A[:lower:]C3-5]", "B", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMixedGroupCase7()
		{
			AssertMatch("[A[:lower:]C3-5]", "2", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMixedGroupCase8()
		{
			AssertMatch("[A[:lower:]C3-5]", "6", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMixedGroupCase9()
		{
			AssertMatch("[A[:lower:]C3-5]", ".", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpecialGroupCase0()
		{
			AssertMatch("[[]", "[", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpecialGroupCase1()
		{
			AssertMatch("[]]", "]", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpecialGroupCase2()
		{
			AssertMatch("[]a]", "]", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpecialGroupCase3()
		{
			AssertMatch("[a[]", "[", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpecialGroupCase4()
		{
			AssertMatch("[a[]", "a", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpecialGroupCase5()
		{
			AssertMatch("[!]]", "]", false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpecialGroupCase6()
		{
			AssertMatch("[!]]", "x", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpecialGroupCase7()
		{
			AssertMatch("[:]]", ":]", true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpecialGroupCase8()
		{
			AssertMatch("[:]]", ":", false, true);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestSpecialGroupCase9()
		{
			try
			{
				AssertMatch("[[:]", ":", true, true);
				NUnit.Framework.Assert.Fail("InvalidPatternException expected");
			}
			catch (InvalidPatternException)
			{
			}
		}

		// expected
		/// <exception cref="System.Exception"></exception>
		public virtual void TestUnsupportedGroupCase0()
		{
			try
			{
				AssertMatch("[[=a=]]", "b", false, false);
				NUnit.Framework.Assert.Fail("InvalidPatternException expected");
			}
			catch (InvalidPatternException e)
			{
				NUnit.Framework.Assert.IsTrue(e.Message.Contains("[=a=]"));
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestUnsupportedGroupCase1()
		{
			try
			{
				AssertMatch("[[.a.]]", "b", false, false);
				NUnit.Framework.Assert.Fail("InvalidPatternException expected");
			}
			catch (InvalidPatternException e)
			{
				NUnit.Framework.Assert.IsTrue(e.Message.Contains("[.a.]"));
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilePathSimpleCase()
		{
			AssertFileNameMatch("a/b", "a/b", '/', true, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilePathCase0()
		{
			AssertFileNameMatch("a*b", "a/b", '/', false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilePathCase1()
		{
			AssertFileNameMatch("a?b", "a/b", '/', false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilePathCase2()
		{
			AssertFileNameMatch("a*b", "a\\b", '\\', false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFilePathCase3()
		{
			AssertFileNameMatch("a?b", "a\\b", '\\', false, false);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestReset()
		{
			string pattern = "helloworld";
			FileNameMatcher matcher = new FileNameMatcher(pattern, null);
			matcher.Append("helloworld");
			NUnit.Framework.Assert.AreEqual(true, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, matcher.CanAppendMatch());
			matcher.Reset();
			matcher.Append("hello");
			NUnit.Framework.Assert.AreEqual(false, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(true, matcher.CanAppendMatch());
			matcher.Append("world");
			NUnit.Framework.Assert.AreEqual(true, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, matcher.CanAppendMatch());
			matcher.Append("to much");
			NUnit.Framework.Assert.AreEqual(false, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, matcher.CanAppendMatch());
			matcher.Reset();
			matcher.Append("helloworld");
			NUnit.Framework.Assert.AreEqual(true, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, matcher.CanAppendMatch());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCreateMatcherForSuffix()
		{
			string pattern = "helloworld";
			FileNameMatcher matcher = new FileNameMatcher(pattern, null);
			matcher.Append("hello");
			FileNameMatcher childMatcher = matcher.CreateMatcherForSuffix();
			NUnit.Framework.Assert.AreEqual(false, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(true, matcher.CanAppendMatch());
			NUnit.Framework.Assert.AreEqual(false, childMatcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(true, childMatcher.CanAppendMatch());
			matcher.Append("world");
			NUnit.Framework.Assert.AreEqual(true, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, matcher.CanAppendMatch());
			NUnit.Framework.Assert.AreEqual(false, childMatcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(true, childMatcher.CanAppendMatch());
			childMatcher.Append("world");
			NUnit.Framework.Assert.AreEqual(true, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, matcher.CanAppendMatch());
			NUnit.Framework.Assert.AreEqual(true, childMatcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, childMatcher.CanAppendMatch());
			childMatcher.Reset();
			NUnit.Framework.Assert.AreEqual(true, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, matcher.CanAppendMatch());
			NUnit.Framework.Assert.AreEqual(false, childMatcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(true, childMatcher.CanAppendMatch());
			childMatcher.Append("world");
			NUnit.Framework.Assert.AreEqual(true, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, matcher.CanAppendMatch());
			NUnit.Framework.Assert.AreEqual(true, childMatcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, childMatcher.CanAppendMatch());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestCopyConstructor()
		{
			string pattern = "helloworld";
			FileNameMatcher matcher = new FileNameMatcher(pattern, null);
			matcher.Append("hello");
			FileNameMatcher copy = new FileNameMatcher(matcher);
			NUnit.Framework.Assert.AreEqual(false, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(true, matcher.CanAppendMatch());
			NUnit.Framework.Assert.AreEqual(false, copy.IsMatch());
			NUnit.Framework.Assert.AreEqual(true, copy.CanAppendMatch());
			matcher.Append("world");
			NUnit.Framework.Assert.AreEqual(true, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, matcher.CanAppendMatch());
			NUnit.Framework.Assert.AreEqual(false, copy.IsMatch());
			NUnit.Framework.Assert.AreEqual(true, copy.CanAppendMatch());
			copy.Append("world");
			NUnit.Framework.Assert.AreEqual(true, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, matcher.CanAppendMatch());
			NUnit.Framework.Assert.AreEqual(true, copy.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, copy.CanAppendMatch());
			copy.Reset();
			NUnit.Framework.Assert.AreEqual(true, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, matcher.CanAppendMatch());
			NUnit.Framework.Assert.AreEqual(false, copy.IsMatch());
			NUnit.Framework.Assert.AreEqual(true, copy.CanAppendMatch());
			copy.Append("helloworld");
			NUnit.Framework.Assert.AreEqual(true, matcher.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, matcher.CanAppendMatch());
			NUnit.Framework.Assert.AreEqual(true, copy.IsMatch());
			NUnit.Framework.Assert.AreEqual(false, copy.CanAppendMatch());
		}
	}
}
