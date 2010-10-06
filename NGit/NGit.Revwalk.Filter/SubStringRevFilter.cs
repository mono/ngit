using NGit.Revwalk;
using NGit.Revwalk.Filter;
using NGit.Util;
using Sharpen;

namespace NGit.Revwalk.Filter
{
	/// <summary>Abstract filter that searches text using only substring search.</summary>
	/// <remarks>Abstract filter that searches text using only substring search.</remarks>
	public abstract class SubStringRevFilter : RevFilter
	{
		/// <summary>Can this string be safely handled by a substring filter?</summary>
		/// <param name="pattern">the pattern text proposed by the user.</param>
		/// <returns>
		/// true if a substring filter can perform this pattern match; false
		/// if
		/// <see cref="PatternMatchRevFilter">PatternMatchRevFilter</see>
		/// must be used instead.
		/// </returns>
		public static bool Safe(string pattern)
		{
			for (int i = 0; i < pattern.Length; i++)
			{
				char c = pattern[i];
				switch (c)
				{
					case '.':
					case '?':
					case '*':
					case '+':
					case '{':
					case '}':
					case '(':
					case ')':
					case '[':
					case ']':
					case '\\':
					{
						return false;
					}
				}
			}
			return true;
		}

		private readonly RawSubStringPattern pattern;

		/// <summary>Construct a new matching filter.</summary>
		/// <remarks>Construct a new matching filter.</remarks>
		/// <param name="patternText">
		/// text to locate. This should be a safe string as described by
		/// the
		/// <see cref="Safe(string)">Safe(string)</see>
		/// as regular expression meta
		/// characters are treated as literals.
		/// </param>
		protected internal SubStringRevFilter(string patternText)
		{
			pattern = new RawSubStringPattern(patternText);
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override bool Include(RevWalk walker, RevCommit cmit)
		{
			return pattern.Match(Text(cmit)) >= 0;
		}

		/// <summary>Obtain the raw text to match against.</summary>
		/// <remarks>Obtain the raw text to match against.</remarks>
		/// <param name="cmit">current commit being evaluated.</param>
		/// <returns>sequence for the commit's content that we need to match on.</returns>
		protected internal abstract RawCharSequence Text(RevCommit cmit);

		public override RevFilter Clone()
		{
			return this;
		}

		// Typically we are actually thread-safe.
		public override string ToString()
		{
			return base.ToString() + "(\"" + pattern.Pattern() + "\")";
		}
	}
}
