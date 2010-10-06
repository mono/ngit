using System;
using System.Text;
using NGit;
using NGit.Revwalk;
using NGit.Revwalk.Filter;
using Sharpen;

namespace NGit.Revwalk.Filter
{
	/// <summary>Abstract filter that searches text using extended regular expressions.</summary>
	/// <remarks>Abstract filter that searches text using extended regular expressions.</remarks>
	public abstract class PatternMatchRevFilter : RevFilter
	{
		/// <summary>Encode a string pattern for faster matching on byte arrays.</summary>
		/// <remarks>
		/// Encode a string pattern for faster matching on byte arrays.
		/// <p>
		/// Force the characters to our funny UTF-8 only convention that we use on
		/// raw buffers. This avoids needing to perform character set decodes on the
		/// individual commit buffers.
		/// </remarks>
		/// <param name="patternText">
		/// original pattern string supplied by the user or the
		/// application.
		/// </param>
		/// <returns>
		/// same pattern, but re-encoded to match our funny raw UTF-8
		/// character sequence
		/// <see cref="NGit.Util.RawCharSequence">NGit.Util.RawCharSequence</see>
		/// .
		/// </returns>
		protected internal static string ForceToRaw(string patternText)
		{
			byte[] b = Constants.Encode(patternText);
			StringBuilder needle = new StringBuilder(b.Length);
			for (int i = 0; i < b.Length; i++)
			{
				needle.Append((char)(b[i] & unchecked((int)(0xff))));
			}
			return needle.ToString();
		}

		private readonly string patternText;

		private readonly Matcher compiledPattern;

		/// <summary>Construct a new pattern matching filter.</summary>
		/// <remarks>Construct a new pattern matching filter.</remarks>
		/// <param name="pattern">
		/// text of the pattern. Callers may want to surround their
		/// pattern with ".*" on either end to allow matching in the
		/// middle of the string.
		/// </param>
		/// <param name="innerString">
		/// should .* be wrapped around the pattern of ^ and $ are
		/// missing? Most users will want this set.
		/// </param>
		/// <param name="rawEncoding">
		/// should
		/// <see cref="ForceToRaw(string)">ForceToRaw(string)</see>
		/// be applied to the pattern
		/// before compiling it?
		/// </param>
		/// <param name="flags">
		/// flags from
		/// <see cref="Sharpen.Pattern">Sharpen.Pattern</see>
		/// to control how matching performs.
		/// </param>
		protected internal PatternMatchRevFilter(string pattern, bool innerString, bool rawEncoding
			, int flags)
		{
			if (pattern.Length == 0)
			{
				throw new ArgumentException(JGitText.Get().cannotMatchOnEmptyString);
			}
			patternText = pattern;
			if (innerString)
			{
				if (!pattern.StartsWith("^") && !pattern.StartsWith(".*"))
				{
					pattern = ".*" + pattern;
				}
				if (!pattern.EndsWith("$") && !pattern.EndsWith(".*"))
				{
					pattern = pattern + ".*";
				}
			}
			string p = rawEncoding ? ForceToRaw(pattern) : pattern;
			compiledPattern = Sharpen.Pattern.Compile(p, flags).Matcher(string.Empty);
		}

		/// <summary>Get the pattern this filter uses.</summary>
		/// <remarks>Get the pattern this filter uses.</remarks>
		/// <returns>the pattern this filter is applying to candidate strings.</returns>
		public virtual string Pattern()
		{
			return patternText;
		}

		/// <exception cref="NGit.Errors.MissingObjectException"></exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public override bool Include(RevWalk walker, RevCommit cmit)
		{
			return compiledPattern.Reset(Text(cmit)).Matches();
		}

		/// <summary>Obtain the raw text to match against.</summary>
		/// <remarks>Obtain the raw text to match against.</remarks>
		/// <param name="cmit">current commit being evaluated.</param>
		/// <returns>sequence for the commit's content that we need to match on.</returns>
		protected internal abstract CharSequence Text(RevCommit cmit);

		public override string ToString()
		{
			return base.ToString() + "(\"" + patternText + "\")";
		}
	}
}
