using System;
using NGit;
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	/// <summary>Searches text using only substring search.</summary>
	/// <remarks>
	/// Searches text using only substring search.
	/// <p>
	/// Instances are thread-safe. Multiple concurrent threads may perform matches on
	/// different character sequences at the same time.
	/// </remarks>
	public class RawSubStringPattern
	{
		private readonly string needleString;

		private readonly byte[] needle;

		/// <summary>Construct a new substring pattern.</summary>
		/// <remarks>Construct a new substring pattern.</remarks>
		/// <param name="patternText">
		/// text to locate. This should be a literal string, as no
		/// meta-characters are supported by this implementation. The
		/// string may not be the empty string.
		/// </param>
		public RawSubStringPattern(string patternText)
		{
			if (patternText.Length == 0)
			{
				throw new ArgumentException(JGitText.Get().cannotMatchOnEmptyString);
			}
			needleString = patternText;
			byte[] b = Constants.Encode(patternText);
			needle = new byte[b.Length];
			for (int i = 0; i < b.Length; i++)
			{
				needle[i] = Lc(b[i]);
			}
		}

		/// <summary>Match a character sequence against this pattern.</summary>
		/// <remarks>Match a character sequence against this pattern.</remarks>
		/// <param name="rcs">
		/// the sequence to match. Must not be null but the length of the
		/// sequence is permitted to be 0.
		/// </param>
		/// <returns>
		/// offset within <code>rcs</code> of the first occurrence of this
		/// pattern; -1 if this pattern does not appear at any position of
		/// <code>rcs</code>.
		/// </returns>
		public virtual int Match(RawCharSequence rcs)
		{
			int needleLen = needle.Length;
			byte first = needle[0];
			byte[] text = rcs.buffer;
			int matchPos = rcs.startPtr;
			int maxPos = rcs.endPtr - needleLen;
			for (; matchPos < maxPos; matchPos++)
			{
				if (Neq(first, text[matchPos]))
				{
					while (++matchPos < maxPos && Neq(first, text[matchPos]))
					{
					}
					if (matchPos == maxPos)
					{
						return -1;
					}
				}
				int si = ++matchPos;
				for (int j = 1; j < needleLen; j++, si++)
				{
					if (Neq(needle[j], text[si]))
					{
						goto OUTER_continue;
					}
				}
				return matchPos - 1;
OUTER_continue: ;
			}
OUTER_break: ;
			return -1;
		}

		private static bool Neq(byte a, byte b)
		{
			return a != b && a != Lc(b);
		}

		private static byte Lc(byte q)
		{
			return unchecked((byte)StringUtils.ToLowerCase((char)(q & unchecked((int)(0xff)))
				));
		}

		/// <summary>Get the literal pattern string this instance searches for.</summary>
		/// <remarks>Get the literal pattern string this instance searches for.</remarks>
		/// <returns>the pattern string given to our constructor.</returns>
		public virtual string Pattern()
		{
			return needleString;
		}

		public override string ToString()
		{
			return Pattern();
		}
	}
}
