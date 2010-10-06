using System;
using NGit;
using NGit.Revwalk;
using NGit.Revwalk.Filter;
using NGit.Util;
using Sharpen;

namespace NGit.Revwalk.Filter
{
	/// <summary>Matches only commits whose committer name matches the pattern.</summary>
	/// <remarks>Matches only commits whose committer name matches the pattern.</remarks>
	public class CommitterRevFilter
	{
		/// <summary>Create a new committer filter.</summary>
		/// <remarks>
		/// Create a new committer filter.
		/// <p>
		/// An optimized substring search may be automatically selected if the
		/// pattern does not contain any regular expression meta-characters.
		/// <p>
		/// The search is performed using a case-insensitive comparison. The
		/// character encoding of the commit message itself is not respected. The
		/// filter matches on raw UTF-8 byte sequences.
		/// </remarks>
		/// <param name="pattern">regular expression pattern to match.</param>
		/// <returns>
		/// a new filter that matches the given expression against the author
		/// name and address of a commit.
		/// </returns>
		public static RevFilter Create(string pattern)
		{
			if (pattern.Length == 0)
			{
				throw new ArgumentException(JGitText.Get().cannotMatchOnEmptyString);
			}
			if (SubStringRevFilter.Safe(pattern))
			{
				return new CommitterRevFilter.SubStringSearch(pattern);
			}
			return new CommitterRevFilter.PatternSearch(pattern);
		}

		public CommitterRevFilter()
		{
		}

		// Don't permit us to be created.
		internal static RawCharSequence TextFor(RevCommit cmit)
		{
			byte[] raw = cmit.RawBuffer;
			int b = RawParseUtils.Committer(raw, 0);
			if (b < 0)
			{
				return RawCharSequence.EMPTY;
			}
			int e = RawParseUtils.NextLF(raw, b, '>');
			return new RawCharSequence(raw, b, e);
		}

		private class PatternSearch : PatternMatchRevFilter
		{
			internal PatternSearch(string patternText) : base(patternText, true, true, Sharpen.Pattern
				.CASE_INSENSITIVE)
			{
			}

			protected internal override CharSequence Text(RevCommit cmit)
			{
				return TextFor(cmit);
			}

			public override RevFilter Clone()
			{
				return new CommitterRevFilter.PatternSearch(Pattern());
			}
		}

		private class SubStringSearch : SubStringRevFilter
		{
			protected internal SubStringSearch(string patternText) : base(patternText)
			{
			}

			protected internal override RawCharSequence Text(RevCommit cmit)
			{
				return TextFor(cmit);
			}
		}
	}
}
