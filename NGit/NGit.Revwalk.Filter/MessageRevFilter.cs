using System;
using NGit;
using NGit.Revwalk;
using NGit.Revwalk.Filter;
using NGit.Util;
using Sharpen;

namespace NGit.Revwalk.Filter
{
	/// <summary>Matches only commits whose message matches the pattern.</summary>
	/// <remarks>Matches only commits whose message matches the pattern.</remarks>
	public class MessageRevFilter
	{
		/// <summary>Create a message filter.</summary>
		/// <remarks>
		/// Create a message filter.
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
		/// a new filter that matches the given expression against the
		/// message body of the commit.
		/// </returns>
		public static RevFilter Create(string pattern)
		{
			if (pattern.Length == 0)
			{
				throw new ArgumentException(JGitText.Get().cannotMatchOnEmptyString);
			}
			if (SubStringRevFilter.Safe(pattern))
			{
				return new MessageRevFilter.SubStringSearch(pattern);
			}
			return new MessageRevFilter.PatternSearch(pattern);
		}

		public MessageRevFilter()
		{
		}

		// Don't permit us to be created.
		internal static RawCharSequence TextFor(RevCommit cmit)
		{
			byte[] raw = cmit.RawBuffer;
			int b = RawParseUtils.CommitMessage(raw, 0);
			if (b < 0)
			{
				return RawCharSequence.EMPTY;
			}
			return new RawCharSequence(raw, b, raw.Length);
		}

		private class PatternSearch : PatternMatchRevFilter
		{
			internal PatternSearch(string patternText) : base(patternText, true, true, Sharpen.Pattern
				.CASE_INSENSITIVE | Sharpen.Pattern.DOTALL)
			{
			}

			protected internal override CharSequence Text(RevCommit cmit)
			{
				return TextFor(cmit);
			}

			public override RevFilter Clone()
			{
				return new MessageRevFilter.PatternSearch(Pattern());
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
